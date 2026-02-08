using Cysharp.Threading.Tasks;
using Taki.RubiksCube.Data;

namespace Taki.RubiksCube.System
{
    internal class CubeRestorer : ICubeActionHandler
    {
        private const float DEFAULT_ROTATION_DURATION = 0.3f;

        private readonly CubeSettings _cubeSettings;

        private readonly float _fastRotationDuration;

        private readonly IRubiksCubeRotator _cubeRotator;
        private readonly ICubeDataProvider _cubeDataProvider;
        private readonly ICubeCancellationToken _cubeCancellationToken;

        public CubeRestorer(
            CubeSettings cubeSettings,
            float fastRotationDuration,
            IRubiksCubeRotator cubeRotator,
            ICubeDataProvider cubeDataProvider,
            ICubeCancellationToken cubeCancellationToken)
        {
            _cubeSettings = cubeSettings;
            _fastRotationDuration = fastRotationDuration;
            _cubeRotator = cubeRotator;
            _cubeDataProvider = cubeDataProvider;
            _cubeCancellationToken = cubeCancellationToken;
        }

        public async UniTask Execute()
        {
            _cubeSettings.RotationDuration = _fastRotationDuration;
            await _cubeRotator.RestoreToInitialState();

            _cubeSettings.RotationDuration = DEFAULT_ROTATION_DURATION;

            if (_cubeCancellationToken.GetToken().IsCancellationRequested)
            {
                return;
            }

            _cubeDataProvider.ValidateAllFacesInitialState();
        }

        public void Dispose()
        {
            _cubeSettings.RotationDuration = DEFAULT_ROTATION_DURATION;
        }
    }
}