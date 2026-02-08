using R3;
using UnityEngine;

namespace Taki.Main.System
{
    internal class PauseService : IPauseEvents
    {
        public Subject<Unit> OnPauseRequested { get; } = new();
        public Subject<Unit> OnResumeRequested { get; } = new();

        private bool _isPaused = false;
        private bool _isFullyPaused = false;

        public bool IsPaused => _isPaused;
        public bool IsFullyPaused => _isFullyPaused;

        public void Pause()
        {
            if (_isPaused)
            {
                Debug.Log($"ゲームは既にポーズ状態です。");
                return;
            }

            _isPaused = true;
            _isFullyPaused = false;
            OnPauseRequested.OnNext(Unit.Default);
            Debug.Log($"ゲームをポーズしました。");
        }

        public void Resume()
        {
            if (!_isPaused)
            {
                Debug.Log($"ゲームは既に再開されています。");
                return;
            }

            _isPaused = false;
            _isFullyPaused = false;
            OnResumeRequested.OnNext(Unit.Default);
            Debug.Log($"ゲームを再開しました。");
        }

        public void SetFullyPaused(bool isFullyPaused)
        {
            _isFullyPaused = isFullyPaused;
            Debug.Log($"完全停止フラグを {isFullyPaused} に更新しました。");
        }

        public void Dispose()
        {
            OnPauseRequested?.Dispose();
            OnResumeRequested?.Dispose();
        }
    }
}