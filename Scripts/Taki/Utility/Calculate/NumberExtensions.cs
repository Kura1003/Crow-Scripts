using System;

namespace Taki.Utility
{
    internal static class NumberExtensions
    {
        internal static bool IsEven(this int number) => number % 2 == 0;

        internal static bool IsOdd(this int number) => !number.IsEven();

        internal static T Negate<T>(this T value) where T : 
            struct, IComparable, IConvertible, IEquatable<T>
        {
            if (value is sbyte sbyteValue)
            {
                return (T)(object)(sbyte)-sbyteValue;
            }

            if (value is short shortValue)
            {
                return (T)(object)(short)-shortValue;
            }

            if (value is int intValue)
            {
                return (T)(object)-intValue;
            }

            if (value is long longValue)
            {
                return (T)(object)-longValue;
            }

            if (value is float floatValue)
            {
                return (T)(object)-floatValue;
            }

            if (value is double doubleValue)
            {
                return (T)(object)-doubleValue;
            }

            if (value is decimal decimalValue)
            {
                return (T)(object)-decimalValue;
            }

            throw new ArgumentException(
                $"型 {typeof(T).Name} は、" +
                $"Negate メソッドでサポートされていません、" +
                $"または符号なしの型です。", nameof(value));
        }
    }
}