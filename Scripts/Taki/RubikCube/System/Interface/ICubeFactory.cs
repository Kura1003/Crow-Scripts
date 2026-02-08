using Cysharp.Threading.Tasks;
using R3;
using System.Threading;
using Taki.RubiksCube.Data;

namespace Taki.RubiksCube.System
{
    internal interface ICubeFactory
    {
        Observable<Unit> OnCubeCreated { get; }
        Observable<Unit> OnCubeDestroyed { get; }

        UniTask PrewarmPoolAsync(CancellationToken token);

        UniTask<CubeGenerationInfo?> GenerateCube(
            float pieceSpacing,
            int cubeSize,
            bool skipIntermediateYields = false);

        UniTask AnimateSpiralActivation(
            float intervalSeconds,
            CancellationToken token);

        UniTask AnimateAllFacesSimultaneously(
            float intervalSeconds,
            CancellationToken token);

        void Destroy();
    }
}