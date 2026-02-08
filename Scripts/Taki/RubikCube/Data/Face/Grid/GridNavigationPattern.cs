using System.Collections.Generic;

namespace Taki.RubiksCube.Data
{
    internal static class GridNavigationPattern
    {
        private static readonly Dictionary<int, List<(int row, int col)>> _spiralCache = new();

        public static IReadOnlyList<(int row, int col)> GetSpiral(int n)
        {
            if (_spiralCache.TryGetValue(n, out var cached)) return cached;

            var indices = new List<(int row, int col)>();
            int top = 0, bottom = n - 1;
            int left = 0, right = n - 1;

            while (top <= bottom && left <= right)
            {
                for (int i = left; i <= right; i++) indices.Add((top, i));
                top++;

                for (int i = top; i <= bottom; i++) indices.Add((i, right));
                right--;

                if (top <= bottom)
                {
                    for (int i = right; i >= left; i--) indices.Add((bottom, i));
                    bottom--;
                }

                if (left <= right)
                {
                    for (int i = bottom; i >= top; i--) indices.Add((i, left));
                    left++;
                }
            }

            _spiralCache[n] = indices;
            return indices;
        }
    }
}