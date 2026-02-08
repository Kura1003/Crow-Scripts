using System.Collections.Generic;

namespace Taki.RubiksCube.Data
{
    internal interface ICubeDataProvider
    {
        void Setup(Dictionary<Face, FaceManagers> faceManagersMap, int cubeSize);
        FaceManagers GetFaceManagers(Face face);
        bool ContainsPiece(Face face, string pieceId);
        bool ValidateAllFacesInitialState();
    }
}