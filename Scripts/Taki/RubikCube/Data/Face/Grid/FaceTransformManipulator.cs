using Taki.Utility.Core;
using UnityEngine;

namespace Taki.RubiksCube.Data
{
    internal class FaceTransformManipulator
    {
        private readonly PieceInfo[,] PiecesInfo;
        private readonly int _cachedSize;

        internal FaceTransformManipulator(
            PieceInfo[,] piecesInfo,
            int cubeSize)
        {
            Thrower.IfNull(piecesInfo, nameof(piecesInfo));

            PiecesInfo = piecesInfo;
            _cachedSize = cubeSize;
        }

        internal void ParentLine(RotationLineInfo lineInfo, Transform parent)
        {
            for (int i = 0; i < _cachedSize; i++)
            {
                Vector2Int index = lineInfo.GetIndex(i);
                PiecesInfo[index.x, index.y].Parent(parent);
            }
        }

        internal void UnparentAll(Transform parent)
        {
            for (int row = 0; row < _cachedSize; row++)
            {
                for (int col = 0; col < _cachedSize; col++)
                {
                    PiecesInfo[row, col].Parent(parent);
                }
            }
        }

        internal void RotateLine(
            RotationLineInfo lineInfo,
            float angle,
            Vector3 worldAxis)
        {
            for (int i = 0; i < _cachedSize; i++)
            {
                Vector2Int index = lineInfo.GetIndex(i);
                PiecesInfo[index.x, index.y].Rotate(angle, worldAxis);
            }
        }

        internal void RotateAll(float angle, Vector3 localAxis)
        {
            for (int row = 0; row < _cachedSize; row++)
            {
                for (int col = 0; col < _cachedSize; col++)
                {
                    PiecesInfo[row, col].RotateLocal(angle, localAxis);
                }
            }
        }
    }
}