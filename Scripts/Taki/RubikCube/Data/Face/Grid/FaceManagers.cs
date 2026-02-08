
namespace Taki.RubiksCube.Data
{
    internal readonly struct FaceManagers
    {
        private readonly FaceCoordinateRegistry _coordinateRegistry;
        private readonly FaceSwapper _swapper;
        private readonly FaceTransformManipulator _manipulator;
        private readonly FaceValidator _validator;

        internal FaceCoordinateRegistry CoordinateRegistry => _coordinateRegistry;
        internal FaceSwapper Swapper => _swapper;
        internal FaceTransformManipulator Manipulator => _manipulator;
        internal FaceValidator Validator => _validator;

        internal FaceManagers(
            PieceInfo[,] piecesInfo,
            int cubeSize)
        {
            _validator = new FaceValidator(piecesInfo, cubeSize);
            _coordinateRegistry = new FaceCoordinateRegistry(piecesInfo, cubeSize);
            _swapper = new FaceSwapper(piecesInfo, cubeSize);
            _manipulator = new FaceTransformManipulator(piecesInfo, cubeSize);
        }
    }
}