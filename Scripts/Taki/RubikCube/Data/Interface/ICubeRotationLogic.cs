
namespace Taki.RubiksCube.Data
{
    internal interface ICubeRotationLogic
    {
        void SetRotationBuffers(Face face, int layerIndex);

        void ClearRotationBuffers(int layerIndex);

        void RotateSideLines(
            Face face,
            int layerIndex,
            bool isClockwise);

        void RotateFaceSurface(int layerIndex, bool isClockwise);
    }
}