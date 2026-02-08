using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using System.Collections.Generic;
using System.Linq;
using Taki.Audio;
using Taki.RubiksCube.Data;
using UnityEngine;
using VContainer;

namespace Taki.RubiksCube.System
{
    internal class RubiksCubeRotationHandler : IRubiksCubeRotator
    {
        private CubeSettings _settings;

        private readonly ICubeRotationLogic _cubeRotationLogic;
        private readonly ICubeTransformManipulator _transformManipulator;
        private readonly ICubeStateSaver _cubeStateSaver;
        private readonly ICubeStateRestorer _cubeStateRestorer;
        private readonly IRotationAxisProvider _rotationAxisProvider;

        private readonly ICubeFactory _factory;
        private readonly ICubeCancellationToken _cubeCancellationToken;

        private bool _isRotating = false;

        private int _cachedSize;
        private int _cachedAngle;

        private readonly struct UndoRotationCommand
        {
            public readonly Face Face;
            public readonly int LayerIndex;
            public readonly bool IsClockwise;

            public UndoRotationCommand(
                Face face, 
                int layerIndex, 
                bool isClockwise)
            {
                Face = face;
                LayerIndex = layerIndex;
                IsClockwise = isClockwise;
            }
        }

        private readonly Stack<UndoRotationCommand> animatedUndoStack = new();

        private bool CanRotateWithUndo()
            => !_settings.IsLimitExceeded(animatedUndoStack.Count);

        private CompositeDisposable _disposables = new();

        [Inject]
        internal RubiksCubeRotationHandler(
            CubeSettings settings,
            ICubeRotationLogic cubeRotationLogic,
            ICubeTransformManipulator transformManipulator,
            ICubeStateSaver cubeStateSaver,
            ICubeStateRestorer cubeStateRestorer,
            IRotationAxisProvider rotationAxisProvider,
            ICubeFactory factory,
            ICubeCancellationToken cubeCancellationToken)
        {
            SetSettings(settings);

            _cubeRotationLogic = cubeRotationLogic;
            _transformManipulator = transformManipulator;
            _cubeStateSaver = cubeStateSaver;
            _cubeStateRestorer = cubeStateRestorer;
            _rotationAxisProvider = rotationAxisProvider;

            _factory = factory;
            _cubeCancellationToken = cubeCancellationToken;

            _factory.OnCubeCreated
                .Subscribe(_ =>
                {
                    SetSettings(settings);
                })
                .AddTo(_disposables);

            _factory.OnCubeDestroyed
                .Subscribe(_ =>
                {
                    ClearRotationHistory();
                })
                .AddTo(_disposables);
        }

        private void SetSettings(CubeSettings settings)
        {
            _settings = settings;
            _cachedSize = _settings.CubeSize;
            _cachedAngle = _settings.RotationAngle;
        }

        public async UniTask ExecuteRotation(
            Face face,
            int layerIndex,
            bool isClockwise,
            bool recordRotation = true,
            bool useUnsafe = false)
        {
            var token = _cubeCancellationToken.GetToken();
            if (token.IsCancellationRequested) return;

            if (_isRotating && !useUnsafe) return;
            if (recordRotation && !CanRotateWithUndo()) return;

            if (!useUnsafe)
            {
                _isRotating = true;
            }

            layerIndex %= _cachedSize;
            _cubeRotationLogic.SetRotationBuffers(face, layerIndex);

            var parentTransform = _rotationAxisProvider.GetCenterTransform();

            var localAxis = Vector3.forward;
            var pivot = _rotationAxisProvider.GetRotationAxis(face, layerIndex);

            _transformManipulator.ParentBufferedPiecesTo(layerIndex, pivot);

            int signedAngle = isClockwise ? _cachedAngle : -_cachedAngle;
            Quaternion finalRotation 
                = pivot.rotation * Quaternion.AngleAxis(signedAngle, localAxis);

            await pivot.DORotate(
                    localAxis * signedAngle,
                    _settings.RotationDuration,
                    RotateMode.LocalAxisAdd)
                    .SetEase(_settings.EasingType)
                    .WithCancellation(token);

            pivot.rotation = finalRotation;

            _transformManipulator.ParentBufferedPiecesTo(layerIndex, parentTransform);

            _cubeRotationLogic.RotateSideLines(face, layerIndex, isClockwise);
            _cubeRotationLogic.RotateFaceSurface(layerIndex, isClockwise);

            _cubeStateSaver.SaveBufferedPiecePositions(layerIndex);
            _cubeStateSaver.SaveBufferedPieceRotations(layerIndex);

            if (!useUnsafe)
            {
                _cubeStateRestorer.RestoreBufferedPiecePositions(layerIndex);
                _cubeStateRestorer.RestoreBufferedPieceRotations(layerIndex);
                _isRotating = false;
            }

            if (recordRotation)
            {
                var undoCommand = 
                    new UndoRotationCommand(
                        face, 
                        layerIndex, 
                        !isClockwise);

                animatedUndoStack.Push(undoCommand);
            }

            AudioManager.Instance.Play("LockEngage", parentTransform.gameObject).SetVolume(0.5f);
        }

        public void ExecuteRotationWithoutAnimation(
            Face face,
            int layerIndex,
            bool isClockwise,
            bool recordRotation = true)
        {
            if (recordRotation && !CanRotateWithUndo()) return;

            layerIndex %= _cachedSize;
            _cubeRotationLogic.SetRotationBuffers(face, layerIndex);

            var localAxis = Vector3.forward;
            var worldAxis = -_rotationAxisProvider.GetFaceNormal(face);
            int signedAngle = isClockwise ? _cachedAngle : -_cachedAngle;

            _transformManipulator.RotateFaceSurfacePieces(layerIndex, signedAngle, localAxis);
            _transformManipulator.RotateSideLinePieces(layerIndex, signedAngle, worldAxis);

            _cubeRotationLogic.RotateSideLines(face, layerIndex, isClockwise);
            _cubeRotationLogic.RotateFaceSurface(layerIndex, isClockwise);

            _cubeStateSaver.SaveBufferedPieceRotations(layerIndex);

            if (recordRotation)
            {
                var undoCommand =
                    new UndoRotationCommand(
                        face,
                        layerIndex,
                        !isClockwise);

                animatedUndoStack.Push(undoCommand);
            }
        }

        public async UniTask RestoreCube(int count)
        {
            if (_isRotating) return;

            var token = _cubeCancellationToken.GetToken();
            var stack = animatedUndoStack;
            var undoCount = (count < 0) ? stack.Count : Mathf.Min(count, stack.Count);

            for (int i = 0; i < undoCount; i++)
            {
                if (stack.Count == 0) break;

                var undo = stack.Pop();

                await ExecuteRotation(
                    undo.Face,
                    undo.LayerIndex,
                    undo.IsClockwise,
                    recordRotation: false,
                    useUnsafe: false);

                if (token.IsCancellationRequested) break;
            }
        }

        public UniTask RestoreToInitialState()
        {
            return RestoreCube(-1);
        }

        private void ClearRotationHistory()
        {
            animatedUndoStack.Clear();
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}