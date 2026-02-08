using System;

namespace Taki.Utility
{
    internal static class SeedGenerator
    {
        private static readonly Random _random = new Random();

        internal static Random GetRandom() => _random;
    }
}