using System;

namespace Taki.RubiksCube.Data
{
    [Flags]
    internal enum Face
    {
        Front = 1 << 0,
        Back = 1 << 1,
        Left = 1 << 2,
        Right = 1 << 3,
        Top = 1 << 4,
        Bottom = 1 << 5
    }
}