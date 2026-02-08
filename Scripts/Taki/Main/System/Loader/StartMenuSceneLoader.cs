using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using Taki.Utility;
using UnityEngine.SceneManagement;
using VContainer;

namespace Taki.Main.System
{
    internal interface IStartMenuSceneLoader : IDisposable
    {
        UniTask LoadStartMenuScene(CancellationToken token);
    }

    internal class StartMenuSceneLoader : IStartMenuSceneLoader
    {
        [Inject] private readonly IScreenFader _screenFader;
        private readonly string _sceneName = "StartMenu";

        public async UniTask LoadStartMenuScene(CancellationToken token)
        {
            _screenFader.SetFadeDuration(0.5f);
            _screenFader.SetFadeInTargetAlpha(1f);

            await _screenFader.FadeIn(token, true);

            await SceneManager.LoadSceneAsync(_sceneName).ToUniTask(cancellationToken: token);
        }

        public void Dispose() { }
    }
}