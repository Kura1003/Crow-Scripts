using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Collections.Generic;
using System.Threading;
using Taki.Audio;
using Taki.RubiksCube.Data;
using Taki.Utility;
using Taki.Utility.Core;
using UnityEngine;

namespace Taki.RubiksCube.System
{
    public class RubiksCubeFactory : MonoBehaviour, ICubeFactory, ICubeCancellationToken
    {
        [SerializeField] private GameObject _cubePiecePrefab;
        [SerializeField] private Color[] _faceColors;
        [SerializeField] private Transform _parentTransform;

        private const int MAX_CUBE_SIZE = 10;

        private readonly Subject<Unit> _onCubeCreated = new();
        private readonly Subject<Unit> _onCubeDestroyed = new();

        public Observable<Unit> OnCubeCreated => _onCubeCreated;
        public Observable<Unit> OnCubeDestroyed => _onCubeDestroyed;

        private CancellationTokenSource _cancellationTokenSource;

        private readonly Dictionary<Face, SpriteRenderer[,]> _instantiatedFaceRenderers = new();
        private readonly Dictionary<Face, Color> _faceColorMap = new();
        private readonly List<GameObject> _instantiatedRotationAxes = new();
        private readonly Stack<SpriteRenderer[,]> _pool = new();

        private int _cachedSize;

        public CancellationToken GetToken() =>
            _cancellationTokenSource != null
                ? _cancellationTokenSource.Token
                : CancellationToken.None;

        private void OnDestroy()
        {
            _onCubeCreated.Dispose();
            _onCubeDestroyed.Dispose();
            CancelAndDispose();
        }

        public void CancelAndDispose()
        {
            if (_cancellationTokenSource is null) return;
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }

        public async UniTask PrewarmPoolAsync(CancellationToken token)
        {
            if (_cubePiecePrefab is null) return;

            int faceCount = FaceUtility.GetFaceCount();

            for (int i = 0; i < faceCount; i++)
            {
                _pool.Push(CreateNewFaceMatrix());
                await UniTask.Yield(cancellationToken: token);

                if (token.IsCancellationRequested) return;
            }
        }

        private SpriteRenderer[,] CreateNewFaceMatrix()
        {
            var matrix = new SpriteRenderer[MAX_CUBE_SIZE, MAX_CUBE_SIZE];

            for (int row = 0; row < MAX_CUBE_SIZE; row++)
            {
                for (int col = 0; col < MAX_CUBE_SIZE; col++)
                {
                    var instance = Instantiate(_cubePiecePrefab, _parentTransform);
                    var renderer = instance.GetComponent<SpriteRenderer>();
                    renderer.color = Color.clear;
                    matrix[row, col] = renderer;
                }
            }

            return matrix;
        }

        private SpriteRenderer[,] GetFaceMatrixFromPool()
        {
            return _pool.Count > 0
                ? _pool.Pop()
                : CreateNewFaceMatrix();
        }

        public async UniTask<CubeGenerationInfo?> GenerateCube(
            float pieceSpacing,
            int cubeSize,
            bool skipIntermediateYields = false)
        {
            try
            {
                CancelAndDispose();
                _cancellationTokenSource = new CancellationTokenSource();
                var token = _cancellationTokenSource.Token;

                _cachedSize = cubeSize;

                Thrower.IfNull(_parentTransform, nameof(_parentTransform));
                Thrower.IfNull(_cubePiecePrefab, nameof(_cubePiecePrefab));

                var faceSpawnDataList = new List<FaceSpawnDataBase>
                {
                    new FrontFaceSpawnData(), new BackFaceSpawnData(),
                    new LeftFaceSpawnData(),  new RightFaceSpawnData(),
                    new TopFaceSpawnData(),   new BottomFaceSpawnData()
                };

                var faceManagersMap = new Dictionary<Face, FaceManagers>();
                var axisInfoMap = new Dictionary<Face, RotationAxisInfo>();

                _instantiatedFaceRenderers.Clear();
                _faceColorMap.Clear();
                ClearRotationAxes();

                int faceIndex = 0;

                foreach (var config in faceSpawnDataList)
                {
                    Face face = config.Face;
                    FaceSpawnInfo spawnInfo = config.SpawnInfo;

                    Color faceColor =
                        (_faceColors != null && faceIndex < _faceColors.Length)
                            ? _faceColors[faceIndex]
                            : Color.white;

                    faceIndex++;
                    _faceColorMap[face] = faceColor;

                    Vector3 faceNormal =
                        _parentTransform.TransformDirection(spawnInfo.Normal);

                    Vector3 faceCenter =
                        _parentTransform.position +
                        faceNormal * (_cachedSize * pieceSpacing / 2f);

                    var piecesMatrix = new PieceInfo[_cachedSize, _cachedSize];
                    var renderers = GetFaceMatrixFromPool();

                    Vector3[,] gridPoints =
                        GridPointCalculator.GenerateGridPoints(
                            Vector3.zero,
                            _cachedSize,
                            _cachedSize,
                            pieceSpacing,
                            spawnInfo.Plane);

                    for (int row = 0; row < _cachedSize; row++)
                    {
                        for (int col = 0; col < _cachedSize; col++)
                        {
                            var renderer = renderers[row, col];

                            renderer.transform.SetPositionAndRotation(
                                faceCenter + gridPoints[row, col],
                                Quaternion.LookRotation(faceNormal) *
                                Quaternion.Euler(spawnInfo.RotationOffset));

                            renderer.color = Color.clear;

                            string pieceId =
                                face.ToString()[0] +
                                (row * _cachedSize + col + 1).ToString();

                            piecesMatrix[row, col] =
                                new PieceInfo(renderer.transform, pieceId, face);
                        }

                        if (!skipIntermediateYields)
                            await UniTask.Yield(token);
                    }


                    _instantiatedFaceRenderers.Add(face, renderers);

                    SetupRotationAxes(
                        face,
                        piecesMatrix,
                        spawnInfo,
                        faceManagersMap,
                        axisInfoMap);
                }

                _onCubeCreated.OnNext(Unit.Default);
                return new CubeGenerationInfo(faceManagersMap, axisInfoMap);
            }
            catch (OperationCanceledException)
            {
                return default;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return default;
            }
        }

        public async UniTask AnimateSpiralActivation(
            float intervalSeconds,
            CancellationToken token)
        {
            var indices = GridNavigationPattern.GetSpiral(_cachedSize);

            await UniTask.WhenAll(
                AnimateFaceSpiral(Face.Front, indices, intervalSeconds, token),
                AnimateFaceSpiral(Face.Back, indices, intervalSeconds, token),
                AnimateFaceSpiral(Face.Left, indices, intervalSeconds, token),
                AnimateFaceSpiral(Face.Right, indices, intervalSeconds, token),
                AnimateFaceSpiral(Face.Bottom, indices, intervalSeconds, token),
                DelayedAnimateFaceSpiral(Face.Top, indices, intervalSeconds, 0.3f, token)
            );
        }

        public async UniTask AnimateAllFacesSimultaneously(
            float intervalSeconds,
            CancellationToken token)
        {
            var indices = GridNavigationPattern.GetSpiral(_cachedSize);
            var tasks = new List<UniTask>();

            foreach (Face face in FaceUtility.GetAllFaces())
            {
                tasks.Add(
                    AnimateFaceSpiral(
                        face,
                        indices,
                        intervalSeconds,
                        token));
            }

            await UniTask.WhenAll(tasks);
        }

        private async UniTask DelayedAnimateFaceSpiral(
            Face face,
            IReadOnlyList<(int row, int col)> indices,
            float intervalSeconds,
            float delaySeconds,
            CancellationToken token)
        {
            await UniTask.WaitForSeconds(delaySeconds, cancellationToken: token);
            await AnimateFaceSpiral(face, indices, intervalSeconds, token);
        }

        private async UniTask AnimateFaceSpiral(
            Face face,
            IReadOnlyList<(int row, int col)> indices,
            float intervalSeconds,
            CancellationToken token)
        {
            if (!_instantiatedFaceRenderers.TryGetValue(face, out var matrix))
                return;

            if (!_faceColorMap.TryGetValue(face, out var faceColor))
                return;

            foreach (var (row, col) in indices)
            {
                if (token.IsCancellationRequested)
                    return;

                matrix[row, col].color = faceColor;

                if (intervalSeconds <= 0)
                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                else
                    await UniTask.WaitForSeconds(intervalSeconds, cancellationToken: token);
            }
        }

        private void SetupRotationAxes(
            Face face,
            PieceInfo[,] piecesMatrix,
            FaceSpawnInfo spawnInfo,
            Dictionary<Face, FaceManagers> faceManagersMap,
            Dictionary<Face, RotationAxisInfo> axisInfoMap)
        {
            Vector3 axisPosition =
                (piecesMatrix[0, 0].Transform.position +
                 piecesMatrix[_cachedSize - 1, _cachedSize - 1].Transform.position) / 2f;

            Quaternion axisRotation =
                Quaternion.LookRotation(_parentTransform.position - axisPosition);

            var axisTransforms = new List<Transform>();

            for (int i = 0; i < _cachedSize; i++)
            {
                var axis = new GameObject($"{face}Axis{i + 1}");
                axis.transform.SetPositionAndRotation(axisPosition, axisRotation);
                axis.transform.SetParent(_parentTransform);

                axisTransforms.Add(axis.transform);
                _instantiatedRotationAxes.Add(axis);
            }

            faceManagersMap.Add(face, new FaceManagers(piecesMatrix, _cachedSize));
            axisInfoMap.Add(face, new RotationAxisInfo(spawnInfo.Normal, axisTransforms));
        }

        private void ClearRotationAxes()
        {
            foreach (var axis in _instantiatedRotationAxes)
                if (axis != null)
                    Destroy(axis);

            _instantiatedRotationAxes.Clear();
        }

        public void Destroy()
        {
            CancelAndDispose();
            AudioManager.Instance.Play("Destroy", gameObject).SetVolume(0.5f);

            foreach (var kvp in _instantiatedFaceRenderers)
            {
                var matrix = kvp.Value;

                for (int row = 0; row < _cachedSize; row++)
                {
                    for (int col = 0; col < _cachedSize; col++)
                        matrix[row, col].color = Color.clear;
                }

                _pool.Push(matrix);
            }

            _instantiatedFaceRenderers.Clear();
            _faceColorMap.Clear();
            ClearRotationAxes();
            _cachedSize = 0;

            _onCubeDestroyed.OnNext(Unit.Default);
        }
    }
}
