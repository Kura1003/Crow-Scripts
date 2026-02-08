using Taki.Utility;

namespace Taki.RubiksCube.Data
{
    internal class FaceValidator
    {
        private readonly PieceInfo[,] _piecesInfo;
        private readonly int _cachedSize;

        internal FaceValidator(PieceInfo[,] piecesInfo, int cubeSize)
        {
            _piecesInfo = piecesInfo;
            _cachedSize = cubeSize;
        }

        internal bool AreAllFacesAligned(Face expectedFace)
        {
            for (int i = 0; i < _cachedSize; i++)
            {
                for (int j = 0; j < _cachedSize; j++)
                {
                    if (_piecesInfo[i, j].InitialFace != expectedFace)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        internal bool ContainsPieceId(string pieceId)
        {
            for (int row = 0; row < _cachedSize; row++)
            {
                for (int col = 0; col < _cachedSize; col++)
                {
                    if (_piecesInfo[row, col].IsSameAs(pieceId))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        internal int GetRandomIndex() => RandomUtility.Range(_cachedSize);
    }
}