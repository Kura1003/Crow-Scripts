using Cysharp.Threading.Tasks;
using System.Threading;
using System;

namespace Taki.StartMenu.System
{
    internal interface IMainSceneLoader : IDisposable
    {
        UniTask LoadMainScene(CancellationToken token);
        void PreloadMainScene();
    }
}
