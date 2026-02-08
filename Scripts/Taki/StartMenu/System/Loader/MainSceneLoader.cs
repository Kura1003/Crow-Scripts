using Cysharp.Threading.Tasks;
using System.Threading;
using Taki.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace Taki.StartMenu.System
{
    internal class MainSceneLoader : IMainSceneLoader
    {
        [Inject] private readonly IScreenFader _screenFader;
        private readonly string _sceneName = "Main";

        private AsyncOperation _loadAsync;

        public void PreloadMainScene()
        {
            if (_loadAsync is null)
            {
                _loadAsync = SceneManager.LoadSceneAsync(_sceneName);
                _loadAsync.allowSceneActivation = false;
            }
        }

        public async UniTask LoadMainScene(CancellationToken token)
        {
            if (_loadAsync is null) return;

            _screenFader.SetFadeDuration(0.5f);
            _screenFader.SetFadeInTargetAlpha(1f);

            await _screenFader.FadeIn(token, true);
            _loadAsync.allowSceneActivation = true;
        }

        public void Dispose()
        {
            _loadAsync = null;
        }
    }
}
