using Taki.Utility;
using UnityEngine;

namespace Taki.RubiksCube.Data
{
    internal readonly struct FaceSpawnInfo
    {
        internal GridPlane Plane { get; }
        internal Vector3 Normal { get; }
        internal Vector3 RotationOffset { get; }

        internal FaceSpawnInfo(
            GridPlane plane,
            Vector3 normal,
            Vector3 rotationOffset)
        {
            Plane = plane;
            Normal = normal;
            RotationOffset = rotationOffset;
        }
    }
}