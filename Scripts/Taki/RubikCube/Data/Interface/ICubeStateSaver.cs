
namespace Taki.RubiksCube.Data
{
    internal interface ICubeStateSaver
    {
        void SaveAllPiecePositions();
        void SaveAllPieceRotations();
        void SaveBufferedPiecePositions(int layerIndex);
        void SaveBufferedPieceRotations(int layerIndex);
    }
}