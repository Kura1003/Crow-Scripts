using System;

namespace Taki.Utility.Core
{
    internal static class ArrayExtensions
    {
        internal static T[] ReverseIf<T>(this T[] array, bool shouldReverse)
        {
            if (shouldReverse) Array.Reverse(array);

            return array;
        }

        internal static void Cycle<T>(this T[][] arrays, int[] cycleOrder)
        {
            var temp = arrays[cycleOrder[0]];

            for (int i = 0; i < cycleOrder.Length - 1; i++)
            {
                arrays[cycleOrder[i]] = arrays[cycleOrder[i + 1]];
            }

            arrays[cycleOrder[^1]] = temp;
        }
    }
}