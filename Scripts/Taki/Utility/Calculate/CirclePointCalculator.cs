using UnityEngine;

namespace Taki.Utility
{
    internal static class CirclePointCalculator
    {
        internal static Vector3[] GenerateCirclePoints(
            Vector3 center,
            float radius,
            int pointCount,
            GridPlane plane,
            float startAngle = 0f) 
        {
            var points = new Vector3[pointCount];
            float angleStep = 360f / pointCount;

            for (int i = 0; i < pointCount; i++)
            {
                float angle = (i * angleStep) + startAngle;
                float radian = angle * Mathf.Deg2Rad;

                float u = radius * Mathf.Cos(radian);
                float v = radius * Mathf.Sin(radian);

                points[i] = MapTo3DPlane(center, u, v, plane);
            }

            return points;
        }

        private static Vector3 MapTo3DPlane(
            Vector3 center,
            float u,
            float v,
            GridPlane plane)
        {
            return plane switch
            {
                GridPlane.XY => new Vector3(center.x + u, center.y + v, center.z),
                GridPlane.YZ => new Vector3(center.x, center.y + u, center.z + v),
                GridPlane.XZ => new Vector3(center.x + u, center.y, center.z + v),
                _ => Vector3.zero
            };
        }
    }
}