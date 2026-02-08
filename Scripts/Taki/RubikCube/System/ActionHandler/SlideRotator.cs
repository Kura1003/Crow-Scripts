using Cysharp.Threading.Tasks;
using R3;
using System.Collections.Generic;
using System.Threading;
using Taki.RubiksCube.Data;
using Taki.Utility;
using UnityEngine;

namespace Taki.RubiksCube.System
{
    internal class SlideRotator : ICubeActionHandler
    {
        private readonly CubeSettings _cubeSettings;

        private int _slideCount;

        private readonly IRubiksCubeRotator _cubeRotator;
        private readonly ICubeFactory _cubeFactory;
        private readonly ICubeCancellationToken _cubeCancellationToken;

        private CompositeDisposable _disposables = new();

        public SlideRotator(
            CubeSettings cubeSettings,
            IRubiksCubeRotator cubeRotator,
            ICubeFactory cubeFactory,
            ICubeCancellationToken cubeCancellationToken)
        {
            _cubeSettings = cubeSettings;

            _cubeRotator = cubeRotator;
            _cubeFactory = cubeFactory;
            _cubeCancellationToken = cubeCancellationToken;

            SetSlideCount();

            _cubeFactory.OnCubeCreated
                .Subscribe(_ => SetSlideCount())
                .AddTo(_disposables);
        }

        private async UniTask ExecuteSingleTask(
            Face face,
            int layerIndex,
            bool isClockwise,
            CancellationToken token)
        {
            await UniTask.WaitForSeconds(
                layerIndex * 0.1f,
                cancellationToken: token);

            await _cubeRotator.ExecuteRotation(
                face,
                layerIndex,
                isClockwise,
                recordRotation: true,
                useUnsafe: true
            );
        }

        public UniTask Execute()
        {
            var isClockwise = RandomUtility.CoinToss();
            var face = FaceUtility.GetRandomFace();
            var token = _cubeCancellationToken.GetToken();

            var rotationTasks = new List<UniTask>();

            for (var i = 0; i < _slideCount; i++)
            {
                var task = ExecuteSingleTask(
                    face,
                    i,
                    isClockwise,
                    token
                );
                rotationTasks.Add(task);
            }

            return UniTask.WhenAll(rotationTasks);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void SetSlideCount()
        {
            _slideCount = _cubeSettings.CubeSize;
            Debug.Log($"スライド回転回数を {_slideCount} に設定しました。");
        }
    }
}