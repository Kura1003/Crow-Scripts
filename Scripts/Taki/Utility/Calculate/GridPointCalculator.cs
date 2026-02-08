using UnityEngine;

namespace Taki.Utility
{
    internal static class GridPointCalculator
    {
        internal static Vector3[,] GenerateGridPoints(
            Vector3 center,
            int rows,
            int columns,
            float spacing,
            GridPlane plane)
        {
            Vector3[,] grid = new Vector3[rows, columns];
            float width = (columns - 1) * spacing;
            float height = (rows - 1) * spacing;
            Vector2 start2D = new Vector2(-width / 2f, -height / 2f);

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    float u = start2D.x + col * spacing;
                    float v = start2D.y + row * spacing;
                    grid[row, col] = MapTo3DPlane(center, u, v, plane);
                }
            }
            return grid;
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