using Taki.Utility.Core;
using UnityEngine;

namespace Taki.RubiksCube.Data
{
    internal class FaceCoordinateRegistry
    {
        private readonly PieceInfo[,] PiecesInfo;
        private readonly int _cachedSize;

        private readonly Vector3[,] _localPositions;
        private readonly Quaternion[,] _localRotations;

        internal FaceCoordinateRegistry(
            PieceInfo[,] piecesInfo,
            int cubeSize)
        {
            Thrower.IfNull(piecesInfo, nameof(piecesInfo));

            PiecesInfo = piecesInfo;
            _cachedSize = cubeSize;

            _localPositions = new Vector3[_cachedSize, _cachedSize];
            _localRotations = new Quaternion[_cachedSize, _cachedSize];

            Initialize();
        }

        private void Initialize()
        {
            for (int row = 0; row < _cachedSize; row++)
            {
                for (int col = 0; col < _cachedSize; col++)
                {
                    _localPositions[row, col] = PiecesInfo[row, col].Transform.localPosition;
                    _localRotations[row, col] = PiecesInfo[row, col].Transform.localRotation;
                }
            }
        }

        internal void SavePositions(RotationLineInfo lineInfo)
        {
            for (int i = 0; i < _cachedSize; i++)
            {
                Vector2Int index = lineInfo.GetIndex(i);
                _localPositions[index.x, index.y] = PiecesInfo[index.x, index.y].Transform.localPosition;
            }
        }

        internal void SaveRotations(RotationLineInfo lineInfo)
        {
            for (int i = 0; i < _cachedSize; i++)
            {
                Vector2Int index = lineInfo.GetIndex(i);
                _localRotations[index.x, index.y] = PiecesInfo[index.x, index.y].Transform.localRotation;
            }
        }

        internal void SaveAllPositions()
        {
            for (int row = 0; row < _cachedSize; row++)
            {
                for (int col = 0; col < _cachedSize; col++)
                {
                    _localPositions[row, col] = PiecesInfo[row, col].Transform.localPosition;
                }
            }
        }

        internal void SaveAllRotations()
        {
            for (int row = 0; row < _cachedSize; row++)
            {
                for (int col = 0; col < _cachedSize; col++)
                {
                    _localRotations[row, col] = PiecesInfo[row, col].Transform.localRotation;
                }
            }
        }

        internal void RestorePositions(RotationLineInfo lineInfo)
        {
            for (int i = 0; i < _cachedSize; i++)
            {
                Vector2Int index = lineInfo.GetIndex(i);
                PiecesInfo[index.x, index.y].Transform.localPosition = _localPositions[index.x, index.y];
            }
        }

        internal void RestoreRotations(RotationLineInfo lineInfo)
        {
            for (int i = 0; i < _cachedSize; i++)
            {
                Vector2Int index = lineInfo.GetIndex(i);
                PiecesInfo[index.x, index.y].Transform.localRotation = _localRotations[index.x, index.y];
            }
        }

        internal void RestoreAllPositions()
        {
            for (int row = 0; row < _cachedSize; row++)
            {
                for (int col = 0; col < _cachedSize; col++)
                {
                    PiecesInfo[row, col].Transform.localPosition = _localPositions[row, col];
                }
            }
        }

        internal void RestoreAllRotations()
        {
            for (int row = 0; row < _cachedSize; row++)
            {
                for (int col = 0; col < _cachedSize; col++)
                {
                    PiecesInfo[row, col].Transform.localRotation = _localRotations[row, col];
                }
            }
        }
    }
}