using UnityEngine;

namespace Taki.RubiksCube.Data
{
    internal interface ICubeTransformManipulator
    {
        void ParentBufferedPiecesTo(int layerIndex, Transform parent);
        void RotateFaceSurfacePieces(int layerIndex, int angle, Vector3 localAxis);
        void RotateSideLinePieces(int layerIndex, int angle, Vector3 worldAxis);
    }
}