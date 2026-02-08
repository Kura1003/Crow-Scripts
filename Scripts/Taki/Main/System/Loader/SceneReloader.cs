using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using Taki.Utility;
using UnityEngine.SceneManagement;
using VContainer;

namespace Taki.Main.System
{
    internal interface ISceneReloader : IDisposable
    {
        UniTask ReloadCurrentScene(CancellationToken token);
    }

    internal class SceneReloader : ISceneReloader
    {
        [Inject] private readonly IScreenFader _screenFader;

        public async UniTask ReloadCurrentScene(CancellationToken token)
        {
            _screenFader.SetFadeDuration(0.5f);
            _screenFader.SetFadeInTargetAlpha(1f);

            await _screenFader.FadeIn(token, true);

            string currentSceneName = SceneManager.GetActiveScene().name;
            await SceneManager.LoadSceneAsync(currentSceneName).ToUniTask(cancellationToken: token);
        }

        public void Dispose() { }
    }
}