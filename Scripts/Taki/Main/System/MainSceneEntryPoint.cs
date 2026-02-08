using AnnulusGames.LucidTools.Audio;
using Cysharp.Threading.Tasks;
using System.Threading;
using Taki.Audio;
using Taki.Main.View;
using Taki.RubiksCube.Data;
using Taki.RubiksCube.System;
using Taki.Utility;
using VContainer.Unity;

namespace Taki.Main.System
{
    internal class MainSceneEntryPoint : IInitializable
    {
        private readonly ImageSequenceFader _imageSequenceFader;
        private readonly CubeSettings _cubeSettings;
        private readonly CameraRotator _cameraRotator;
        private readonly TextLineAnimationManager _lineAnimationManager;

        private readonly TextBurstRadiator _textBurstRadiator;
        private readonly UILineAnimationMediator _animationMediator;
        private readonly ButtonEntrypoint _buttonEntrypoint;
        private readonly PauseLifecycleManager _pauseLifecycleManager;
        private AudioManager _audioManager;

        private readonly IScreenFader _screenFader;
        private readonly ICubeFactory _cubeFactory;
        private readonly ICubeDataProvider _cubeDataProvider;
        private readonly IRotationAxisProvider _rotationAxisProvider;
        private readonly ICubeInteractionHandler _cubeInteractionHandler;

        private readonly CancellationToken _token;

        internal MainSceneEntryPoint(
            ImageSequenceFader imageSequenceFader,
            CubeSettings cubeSettings,
            CameraRotator cameraRotator,
            TextLineAnimationManager lineAnimationManager,
            TextBurstRadiator textBurstRadiator,
            UILineAnimationMediator animationMediator,
            ButtonEntrypoint buttonEntrypoint,
            PauseLifecycleManager pauseLifecycleManager,
            IScreenFader screenFader,
            ICubeFactory cubeFactory,
            ICubeDataProvider cubeDataProvider,
            IRotationAxisProvider rotationAxisProvider,
            ICubeInteractionHandler cubeInteractionHandler,
            LifetimeScope scope)
        {
            _imageSequenceFader = imageSequenceFader;
            _cubeSettings = cubeSettings;
            _cameraRotator = cameraRotator;
            _lineAnimationManager = lineAnimationManager;
            _textBurstRadiator = textBurstRadiator;
            _animationMediator = animationMediator;
            _buttonEntrypoint = buttonEntrypoint;
            _pauseLifecycleManager = pauseLifecycleManager;

            _screenFader = screenFader;
            _cubeFactory = cubeFactory;
            _cubeDataProvider = cubeDataProvider;
            _rotationAxisProvider = rotationAxisProvider;
            _cubeInteractionHandler = cubeInteractionHandler;

            _token = scope.destroyCancellationToken;
        }

        public async void Initialize()
        {
            bool canceled = await RunSequenceAsync().SuppressCancellationThrow();
            if (canceled) return;

            FinalizePresentation();

            await StartPostPresentationEffects(_token).SuppressCancellationThrow();
        }

        private async UniTask StartPostPresentationEffects(CancellationToken token)
        {
            await UniTask.WaitForSeconds(1.5f, ignoreTimeScale: true, cancellationToken: token);

            _lineAnimationManager
                .PlayInfiniteSequence(0.5f)
                .SuppressCancellationThrow()
                .Forget();

            _audioManager.CurrentBgmPlayer = _audioManager
                .Play("Yuyake_Koyake-MB02-1(Reverb-Slow)", _buttonEntrypoint.gameObject)
                .SetLoop(true);
        }

        private async UniTask RunSequenceAsync()
        {
            await SetupSystemAsync();
            await PlayPresentationSequenceAsync();
        }

        private async UniTask SetupSystemAsync()
        {
            LucidAudio.BGMVolume = 0f;
            LucidAudio.SEVolume = 0f;
            _audioManager = AudioManager.Instance;

            await _lineAnimationManager.PrewarmPoolAsync(_token);
            await _textBurstRadiator.Initialize(_token);
            await _cubeFactory.PrewarmPoolAsync(_token);

            _screenFader.SetFadeInTargetAlpha(1f);
            _cameraRotator.IsLocked = true;
            _animationMediator.ExecuteGeneration();

            await UniTask.Yield(cancellationToken: _token);

            var generationInfo = await _cubeFactory.GenerateCube(
                _cubeSettings.PieceSpacing,
                _cubeSettings.CubeSize,
                false,
                true);

            _cubeDataProvider.Setup(
                generationInfo.Value.FaceManagersMap,
                _cubeSettings.CubeSize);

            _buttonEntrypoint.CollectHandlers();
            _imageSequenceFader.SetImagesTransparent();

            await UniTask.Yield(cancellationToken: _token);

            _rotationAxisProvider.SetUp(
                generationInfo.Value.AxisInfoMap);
        }

        private async UniTask PlayPresentationSequenceAsync()
        {
            await _screenFader.FadeOut(_token);

            await UniTask.WhenAll(
                _imageSequenceFader.PlayFadeSequenceAsync(_token),
                _cubeFactory.AnimateSpiralActivation(0.05f, _token));
        }

        private void FinalizePresentation()
        {
            _cameraRotator.IsLocked = false;
            _animationMediator.ExecuteSetup();

            _buttonEntrypoint.InitializeHandlers();
            _cubeInteractionHandler.RegisterEvents();
            _pauseLifecycleManager.Initialize();

            LucidAudio.BGMVolume = 1f;
            LucidAudio.SEVolume = 1f;
        }
    }
}