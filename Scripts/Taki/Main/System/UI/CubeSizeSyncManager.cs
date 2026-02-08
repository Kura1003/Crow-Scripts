using R3;
using System;
using Taki.Main.View;
using Taki.RubiksCube.Data;
using UnityEngine;

namespace Taki.Main.System
{
    public class CubeSizeSyncManager : IDisposable
    {
        private readonly CircleTextRotator _rotator;
        private readonly CubeSettings _settings;
        private readonly CompositeDisposable _disposables = new();

        private int _previousCubeSize;

        public bool IsSizeChanged => _settings.CubeSize != _previousCubeSize;

        public CubeSizeSyncManager(
            CircleTextRotator rotator, 
            CubeSettings settings)
        {
            _rotator = rotator;
            _settings = settings;

            _previousCubeSize = _settings.CubeSize;

            _rotator.OnRotationComplete
                .Subscribe(_ => SyncSize())
                .AddTo(_disposables);
        }

        private void SyncSize()
        {
            int newSize = _rotator.GetSelectedNode().CubeSize;

            if (_settings.CubeSize != newSize)
            {
                _settings.CubeSize = newSize;
                Debug.Log($"キューブサイズを同期しました: {newSize}");
            }
        }

        public void MarkAsProcessed()
        {
            _previousCubeSize = _settings.CubeSize;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}