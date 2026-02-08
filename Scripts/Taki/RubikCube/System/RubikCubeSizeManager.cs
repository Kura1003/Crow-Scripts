using R3;
using Taki.RubiksCube.Data;
using UnityEngine;
using VContainer;

namespace Taki.RubiksCube.System
{
    internal class RubikCubeSizeManager : ICubeSizeManager
    {
        private const int INITIAL_CUBE_SIZE = 4;

        private readonly CubeSettings _cubeSettings;

        private CompositeDisposable _disposables = new();

        [Inject]
        internal RubikCubeSizeManager(
            CubeSettings cubeSettings,
            ICubeInteractionHandler interactionHandler)
        {
            _cubeSettings = cubeSettings;

            SetCubeSize(INITIAL_CUBE_SIZE);
        }

        public float GetSizeScaleFactor()
        {
            float t = _cubeSettings.CubeSize * 0.1f;

            const float MinFactor = 0f;
            const float MaxFactor = 2.0f;

            float factor = Mathf.Lerp(MinFactor, MaxFactor, t);
            return factor;
        }

        public void SetCubeSize(int cubeSize)
        {
            _cubeSettings.CubeSize = cubeSize;
        }

        public void Dispose()
        {
            SetCubeSize(INITIAL_CUBE_SIZE);
            _disposables.Dispose();
        }
    }
}