
namespace Taki.RubiksCube.Data
{
    internal readonly struct RotationLayerInfo
    {
        internal bool IsFrontLayer { get; }
        internal bool IsOppositeLayer { get; }
        internal bool IsMiddleLayer { get; }

        internal RotationLayerInfo(int layerIndex, int size)
        {
            IsFrontLayer = layerIndex == 0;
            IsOppositeLayer = layerIndex == size - 1;
            IsMiddleLayer = !IsFrontLayer && !IsOppositeLayer;
        }
    }
}