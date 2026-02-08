using Cysharp.Threading.Tasks;

namespace Taki.RubiksCube.System
{
    internal interface ICubeInteractionHandler
    {
        bool IsTaskRunning { get; }
        void RegisterEvents();
        void UnregisterEvents();
        UniTask ExecuteRebuild();
    }
}