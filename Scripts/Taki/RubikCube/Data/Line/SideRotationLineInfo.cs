
namespace Taki.RubiksCube.Data
{
    internal readonly struct SideRotationLineInfo
    {
        internal Face LineFace { get; }

        internal Line LineType { get; }

        internal bool IsReversed { get; }

        internal SideRotationLineInfo(
            Face lineFace,
            Line lineType,
            bool isReversed)
        {
            LineFace = lineFace;
            LineType = lineType;
            IsReversed = isReversed;
        }

        internal RotationLineInfo GetLineInfo(
            int layerIndex,
            int size)
        {
            if (IsReversed)
            {
                int normalizedIndex = (size - 1 - layerIndex) % size;
                return new RotationLineInfo(LineFace, LineType, normalizedIndex);
            }

            else
            {
                int normalizedIndex = layerIndex % size;
                return new RotationLineInfo(LineFace, LineType, normalizedIndex);
            }
        }
    }
}