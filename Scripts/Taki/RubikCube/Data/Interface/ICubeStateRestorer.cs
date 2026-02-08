
namespace Taki.RubiksCube.Data
{
    internal interface ICubeStateRestorer
    {
        void RestoreAllPiecePositions();
        void RestoreAllPieceRotations();
        void RestoreBufferedPiecePositions(int layerIndex);
        void RestoreBufferedPieceRotations(int layerIndex);
    }
}