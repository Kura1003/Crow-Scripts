using Cysharp.Threading.Tasks;
using R3;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Taki.Main.View;
using Taki.RubiksCube.Data;
using Taki.RubiksCube.System;
using Taki.Utility;
using UnityEngine;
using VContainer;
using XPostProcessing;

namespace Taki.Main.System
{
    public class PauseLifecycleManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _targetObjects = new();
        [SerializeField] private Canvas _canvas;
        [SerializeField] private CircleTextRotator _circleTextRotator;
        [SerializeField] private DualCanvasGroupFader _dualCanvasGroupFader;
        [SerializeField] private TextLineAnimationManager _lineAnimationManager;
        [SerializeField] private TextBurstRadiator _textBurstRadiator;
        [SerializeField] private List<TextTyper> _textTypers;
        [SerializeField] private EdgeEffectController _edgeEffectController;

        [Inject] private readonly CubeSettings _cubeSettings;

        [Inject] private readonly IPauseEvents _pauseEvents;
        [Inject] private readonly ICubeInteractionHandler _cubeInteractionHandler;
        [Inject] private readonly IPostProcessEffectProvider _postProcessProvider;

        private CubeSizeSyncManager _syncManager;
        private EdgeDetectionSobelNeonV2 _edgeDetectionSobelNeonV2;
        private EdgeDetectionRobertsNeon _edgeDetectionRobertsNeon;

        public void Initialize()
        {
            _pauseEvents.OnPauseRequested
                .Subscribe(_ =>
                HandlePauseAsync(destroyCancellationToken)
                .SuppressCancellationThrow()
                .Forget())
                .AddTo(this);

            _pauseEvents.OnResumeRequested
                .Subscribe(_ =>
                HandleResumeAsync(destroyCancellationToken)
                .SuppressCancellationThrow()
                .Forget())
                .AddTo(this);

            _syncManager = new CubeSizeSyncManager(_circleTextRotator, _cubeSettings);
            _syncManager.MarkAsProcessed();

            _edgeDetectionSobelNeonV2 = _postProcessProvider
                .GetEffect<EdgeDetectionSobelNeonV2>();

            _edgeDetectionRobertsNeon = _postProcessProvider
                .GetEffect<EdgeDetectionRobertsNeon>();
        }

        private async UniTask HandlePauseAsync(CancellationToken token)
        {
            Time.timeScale = 0f;

            _canvas.renderMode = RenderMode.ScreenSpaceCamera;
            _edgeDetectionSobelNeonV2.active = false;
            _edgeDetectionRobertsNeon.active = true;
            _edgeDetectionRobertsNeon.BackgroundFade.value = 0f;

            _targetObjects.Where(obj => obj != null).ToList().ForEach(obj => obj.SetActive(false));

            var tasks = _textTypers
                .Select(typer => typer.Type(token))
                .Append(_dualCanvasGroupFader.FadeOut(token))
                .ToList();

            await UniTask.WhenAll(tasks);

            _pauseEvents.SetFullyPaused(true);
        }

        private async UniTask HandleResumeAsync(CancellationToken token)
        {
            Time.timeScale = 1f;

            if (!_syncManager.IsSizeChanged)
            {
                _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }

            _edgeDetectionSobelNeonV2.active = true;
            _edgeDetectionRobertsNeon.active = false;
            _edgeDetectionRobertsNeon.BackgroundFade.value = 0f;
            _edgeEffectController.CancelAnimation();

            _targetObjects.Where(obj => obj != null).ToList().ForEach(obj => obj.SetActive(true));
            _textTypers.ForEach(typer => typer.ClearImmediately());

            var tasks = new List<UniTask>
            {
                _dualCanvasGroupFader.FadeIn(token),
                _textBurstRadiator.Overdrive(token)
            };

            if (_syncManager.IsSizeChanged)
            {
                tasks.Add(HandleSizeChangedRebuildAsync());
            }

            await UniTask.WhenAll(tasks);
        }

        private async UniTask HandleSizeChangedRebuildAsync()
        {
            _syncManager.MarkAsProcessed();
            _lineAnimationManager.StopInfiniteSequence();

            await _cubeInteractionHandler.ExecuteRebuild();

            _lineAnimationManager
                .PlayInfiniteSequence(0.5f)
                .SuppressCancellationThrow()
                .Forget();
        }

        private void OnDestroy()
        {
            _syncManager?.Dispose();
        }
    }
}