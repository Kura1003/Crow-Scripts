using System.Collections.Generic;

namespace Taki.Utility
{
    internal static class ShuffleExtensions
    {
        internal static void Shuffle<T>(this T[] array)
        {
            var random = SeedGenerator.GetRandom();
            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                (array[n], array[k]) = (array[k], array[n]);
            }
        }

        internal static void Shuffle<T>(this List<T> list)
        {
            var random = SeedGenerator.GetRandom();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }
    }
}