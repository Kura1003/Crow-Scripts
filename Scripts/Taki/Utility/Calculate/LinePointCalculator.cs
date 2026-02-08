using UnityEngine;

namespace Taki.Utility
{
    internal static class LinePointCalculator
    {
        internal static Vector3[] GenerateLinePoints(
            Vector3 center,
            int count,
            float spacing,
            GridPlane plane)
        {
            if (count <= 0) return new Vector3[0];

            Vector3[] points = new Vector3[count];

            float length = (count - 1) * spacing;
            float start = -length / 2f;

            for (int i = 0; i < count; i++)
            {
                float u = start + i * spacing;
                points[i] = MapTo3DPlane(center, u, plane);
            }

            return points;
        }

        private static Vector3 MapTo3DPlane(
            Vector3 center,
            float u,
            GridPlane plane)
        {
            return plane switch
            {
                GridPlane.XY => new Vector3(center.x + u, center.y, center.z),
                GridPlane.YZ => new Vector3(center.x, center.y + u, center.z),
                GridPlane.XZ => new Vector3(center.x + u, center.y, center.z),
                _ => Vector3.zero
            };
        }
    }
}