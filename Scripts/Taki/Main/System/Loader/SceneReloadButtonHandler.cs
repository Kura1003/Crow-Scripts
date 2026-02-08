using AnnulusGames.LucidTools.Audio;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Taki.RubiksCube.System;
using Taki.Utility;
using VContainer;
using Taki.Main.View;

namespace Taki.Main.System
{
    public class SceneReloadButtonHandler : PointerEventHandler
    {
        [SerializeField] private TextLineAnimationManager _lineAnimationManager;

        [Inject] private readonly ButtonEntrypoint _buttonEntrypoint;

        [Inject] private readonly ICubeInteractionHandler _cubeInteractionHandler;
        [Inject] private readonly ICubeCancellationToken _cubeCancellationToken;
        [Inject] private readonly ISceneReloader _sceneReloader;

        protected override void OnClicked()
        {
            Time.timeScale = 1.0f;

            _buttonEntrypoint.DisposeHandlers();
            _cubeInteractionHandler.UnregisterEvents();

            _lineAnimationManager.StopLoopOnly();
            _cubeCancellationToken.CancelAndDispose();

            LucidAudio.StopAll();

            _sceneReloader
                .ReloadCurrentScene(destroyCancellationToken)
                .SuppressCancellationThrow()
                .Forget();
        }

        protected override void OnPointerEntered() { }

        protected override void OnPointerExited() { }
    }
}