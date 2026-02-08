using System.Collections.Generic;
using Taki.Utility.Core;
using UnityEngine;

namespace Taki.RubiksCube.Data
{
    internal class RotationAxisManager : IRotationAxisProvider
    {
        private Transform _parentTransform;
        private Dictionary<Face, RotationAxisInfo> _axisInfoMap = new();
        private readonly Dictionary<Face, Vector3> _cachedFaceNormals = new();

        internal RotationAxisManager(
            Transform parentTransform)
        {
            Thrower.IfNull(parentTransform, nameof(parentTransform));

            _parentTransform = parentTransform;
        }

        public void SetUp(
            Dictionary<Face, RotationAxisInfo> axisInfoMap)
        {
            Thrower.IfNull(axisInfoMap, nameof(axisInfoMap));

            _axisInfoMap = axisInfoMap;
            _cachedFaceNormals.Clear();

            foreach (var pair in _axisInfoMap)
            {
                var face = pair.Key;
                var normal = pair.Value.Normal;
                _cachedFaceNormals[face] = _parentTransform.TransformDirection(normal);
            }
        }

        public Vector3 GetFaceNormal(Face face) => _cachedFaceNormals[face];

        public Transform GetRotationAxis(Face face, int layerIndex)
        {
            var axes = _axisInfoMap[face].RotationAxes;
            return axes[layerIndex];
        }

        public Transform GetCenterTransform() => _parentTransform;
    }
}