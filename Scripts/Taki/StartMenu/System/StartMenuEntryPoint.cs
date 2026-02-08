using AnnulusGames.LucidTools.Audio;
using Cysharp.Threading.Tasks;
using Taki.Utility;
using System.Threading;
using VContainer.Unity;

namespace Taki.StartMenu.System
{
    internal class StartMenuEntryPoint : IInitializable
    {
        private readonly CursorEffectSpawner _spawner;
        private readonly TapToStartHandler _tapHandler;
        private readonly ImageSequenceFader _imageFader;
        private readonly CircularImageSpawner _circularSpawner;
        private readonly ButtonEntrypoint _buttonEntrypoint;

        private readonly IScreenFader _screenFader;
        private readonly IMainSceneLoader _sceneLoader;
        private readonly CancellationToken _token;

        internal StartMenuEntryPoint(
            CursorEffectSpawner spawner,
            TapToStartHandler tapHandler,
            ImageSequenceFader imageFader,
            CircularImageSpawner circularSpawner,
            ButtonEntrypoint buttonEntrypoint,
            IScreenFader screenFader,
            IMainSceneLoader sceneLoader,
            LifetimeScope scope)
        {
            _spawner = spawner;
            _tapHandler = tapHandler;
            _imageFader = imageFader;
            _circularSpawner = circularSpawner;
            _buttonEntrypoint = buttonEntrypoint;

            _screenFader = screenFader;
            _sceneLoader = sceneLoader;

            _token = scope.destroyCancellationToken;
        }

        public async void Initialize()
        {
            bool canceled = await RunSequenceAsync().SuppressCancellationThrow();

            if (!canceled)
            {
                FinalizePresentation();
            }
        }

        private async UniTask RunSequenceAsync()
        {
            await SetupSystemAsync();
            await PlayPresentationSequenceAsync();
        }

        private async UniTask SetupSystemAsync()
        {
            LucidAudio.SEVolume = 1f;
            LucidAudio.BGMVolume = 1f;

            _spawner.CanSpawn = false;
            _screenFader.SetFadeInTargetAlpha(0.5f);
            _circularSpawner.SpawnAtThirdQuadrant();
            _buttonEntrypoint.CollectHandlers();

            await UniTask.DelayFrame(1, cancellationToken: _token);

            _imageFader.SetImagesTransparent();
            _sceneLoader.PreloadMainScene();
        }

        private async UniTask PlayPresentationSequenceAsync()
        {
            await _screenFader.FadeIn(_token, true);

            await _tapHandler.WaitTapAsync(_token);

            _screenFader.SetFadeInTargetAlpha(1f);
            await _screenFader.FadeOut(_token);

            await _imageFader.PlayFadeSequenceAsync(_token);
            await _circularSpawner.PlayActivationSequenceAsync(_token);
        }

        private void FinalizePresentation()
        {
            _buttonEntrypoint.InitializeHandlers();
            _spawner.CanSpawn = true;
        }
    }
}