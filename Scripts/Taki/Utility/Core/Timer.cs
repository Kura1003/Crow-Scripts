using System;
using System.Diagnostics;

namespace Taki.Utility.Core
{
    internal static class Timer
    {
        internal static IDisposable Measure(string message)
        {
            return new PerformanceTimer(message);
        }

        private sealed class PerformanceTimer : IDisposable
        {
            private readonly Stopwatch _stopwatch;
            private readonly string _message;

            internal PerformanceTimer(string message)
            {
                _message = message;
                _stopwatch = Stopwatch.StartNew();
            }

            public void Dispose()
            {
                _stopwatch.Stop();
                UnityEngine.Debug.Log(
                    $"{_message} の実行時間: " +
                    $"{_stopwatch.ElapsedMilliseconds} ms");
            }
        }
    }
}