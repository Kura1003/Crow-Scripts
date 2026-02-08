using UnityEngine;

namespace Taki.RubiksCube.Data
{
    internal readonly struct RotationLineInfo
    {
        internal Face Face { get; }

        private readonly Line _lineType;

        private readonly int _fixedIndex;

        internal RotationLineInfo(
            Face face, 
            Line lineType, 
            int fixedIndex)
        {
            Face = face;
            _lineType = lineType;
            _fixedIndex = fixedIndex;
        }

        internal Vector2Int GetIndex(int lineIndex)
        {
            if (IsRow())
            {
                return new Vector2Int(_fixedIndex, lineIndex);
            }
            else
            {
                return new Vector2Int(lineIndex, _fixedIndex);
            }
        }

        private bool IsRow() => _lineType == Line.Row;
    }
}