using System;
using UnityEngine;

namespace Taki.Utility.Core
{
    internal static class Thrower
    {
        internal static void IfTrue(bool condition, string message)
        {
            if (condition)
            {
                throw new InvalidOperationException(message);
            }
        }

        internal static void IfNull<T>(T data, string paramName) where T : class
        {
            if (data is null)
            {
                throw new ArgumentNullException(
                    paramName, 
                    $"パラメーターがnullです。");
            }
        }

        internal static void IfNotMatch<TExpected, TActual>(string message)
        {
            if (typeof(TExpected) != typeof(TActual))
            {
                throw new InvalidCastException(message);
            }
        }

        internal static void IfOutOfRange(int index, int length)
        {
            if (index < 0 || index >= length)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(index), 
                    $"インデックスは 0 から {length - 1} の範囲内である必要があります。");
            }
        }

        internal static void IfOutOfRange(Vector2Int index, Vector2Int size)
        {
            if (index.x < 0 || index.x >= size.x || index.y < 0 || index.y >= size.y)
            {
                throw new ArgumentOutOfRangeException(
                    $"xは 0 から {size.x - 1}、" +
                    $"yは 0 から {size.y - 1} の範囲内である必要があります。");
            }
        }

        internal static void IfOutOfRange(Vector3Int index, Vector3Int size)
        {
            if (index.x < 0 || index.x >= size.x || index.y < 0 || index.y >= size.y || index.z < 0 || index.z >= size.z)
            {
                throw new ArgumentOutOfRangeException(
                    $"xは 0 から {size.x - 1}、" +
                    $"yは 0 から {size.y - 1}、" +
                    $"zは 0 から {size.z - 1} の範囲内である必要があります。");
            }
        }

        internal static void IfOutOfRange<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value), 
                    $"値は {min} から {max} の範囲内である必要があります。");
            }
        }
    }
}