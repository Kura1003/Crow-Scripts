using System.Collections.Generic;
using UnityEngine;

namespace Taki.RubiksCube.Data
{
    internal interface IRotationAxisProvider
    {
        Vector3 GetFaceNormal(Face face);
        Transform GetRotationAxis(Face face, int layerIndex);
        Transform GetCenterTransform();
        void SetUp(
            Dictionary<Face, RotationAxisInfo> axisInfoMap);
    }
}