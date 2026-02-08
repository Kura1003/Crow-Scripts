using R3;
using System;

namespace Taki.Main.System
{
    internal interface IPauseEvents : IDisposable
    {
        Subject<Unit> OnPauseRequested { get; }
        Subject<Unit> OnResumeRequested { get; }

        bool IsPaused { get; }
        bool IsFullyPaused { get; }

        void Pause();
        void Resume();
        void SetFullyPaused(bool isFullyPaused);
    }
}