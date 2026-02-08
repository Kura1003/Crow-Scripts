using Cysharp.Threading.Tasks;
using Taki.RubiksCube.Data;

namespace Taki.RubiksCube.System
{
    internal class CubeRebuilder : ICubeActionHandler
    {
        private readonly CubeSettings _cubeSettings;

        private readonly ICubeFactory _cubeFactory;
        private readonly ICubeDataProvider _cubeDataProvider;
        private readonly IRotationAxisProvider _rotationAxisProvider;
        private readonly ICubeCancellationToken _cubeCancellationToken;

        public CubeRebuilder(
            CubeSettings cubeSettings,
            ICubeFactory cubeFactory,
            ICubeDataProvider cubeDataProvider,
            IRotationAxisProvider rotationAxisProvider,
            ICubeCancellationToken cubeCancellationToken)
        {
            _cubeSettings = cubeSettings;

            _cubeFactory = cubeFactory;
            _cubeDataProvider = cubeDataProvider;
            _rotationAxisProvider = rotationAxisProvider;
            _cubeCancellationToken = cubeCancellationToken;
        }

        public async UniTask Execute()
        {
            _cubeFactory.Destroy();

            var cubeSize = _cubeSettings.CubeSize;
            var pieceSpacing = _cubeSettings.PieceSpacing;

            var generationInfo = await _cubeFactory.GenerateCube(
                pieceSpacing,
                cubeSize,
                false,
                true);

            if (generationInfo is null)
            {
                return;
            }

            _cubeDataProvider.Setup(
                generationInfo.Value.FaceManagersMap,
                cubeSize);

            _rotationAxisProvider.SetUp(
                generationInfo.Value.AxisInfoMap);

            await _cubeFactory.AnimateAllFacesSimultaneously(
                -1,
                _cubeCancellationToken.GetToken());
        }

        public void Dispose() { }
    }
}