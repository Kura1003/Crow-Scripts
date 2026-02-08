using UnityEngine;

namespace Taki.Utility
{
    public struct ColorIgnoreSettings
    {
        public Color BaseColor;
        public float Threshold;

        public ColorIgnoreSettings(Color baseColor, float threshold)
        {
            BaseColor = baseColor;
            Threshold = threshold;
        }
    }

    internal static class RandomUtility
    {
        internal static Color GetColor(
            float alpha = 1.0f, ColorIgnoreSettings? 
            ignoreSettings = null)
        {
            var random = SeedGenerator.GetRandom();
            Color result;

            while (true)
            {
                result = new Color(
                    (float)random.NextDouble(),
                    (float)random.NextDouble(),
                    (float)random.NextDouble(),
                    alpha);

                if (!ignoreSettings.HasValue) break;

                var settings = ignoreSettings.Value;
                float diff =
                    Mathf.Abs(result.r - settings.BaseColor.r) +
                    Mathf.Abs(result.g - settings.BaseColor.g) +
                    Mathf.Abs(result.b - settings.BaseColor.b);

                if (diff > settings.Threshold) break;
            }

            return result;
        }

        internal static int Range(int min, int max)
        {
            return SeedGenerator.GetRandom().Next(min, max);
        }

        internal static int Range(int max)
        {
            return SeedGenerator.GetRandom().Next(0, max);
        }

        internal static double Range(double min, double max)
        {
            return SeedGenerator.GetRandom().NextDouble() * (max - min) + min;
        }

        internal static bool CoinToss()
        {
            return SeedGenerator.GetRandom().Next(0, 2) == 1;
        }
    }
}