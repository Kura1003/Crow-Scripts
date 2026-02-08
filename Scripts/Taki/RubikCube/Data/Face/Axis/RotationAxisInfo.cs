using System.Collections.Generic;
using UnityEngine;

namespace Taki.RubiksCube.Data
{
    internal readonly struct RotationAxisInfo
    {
        internal Vector3 Normal { get; }
        internal List<Transform> RotationAxes { get; }

        internal RotationAxisInfo(
            Vector3 normal, 
            List<Transform> rotationAxes)
        {
            Normal = normal;
            RotationAxes = rotationAxes;
        }
    }
}