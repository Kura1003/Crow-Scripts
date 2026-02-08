using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Taki.Utility
{
    public class FadeScreenOnPointerHandler : PointerEventHandler
    {
        [SerializeField] private float _fadeDuration = 0.5f;
        [SerializeField] private float _fadeInTargetAlpha = 1.0f;

        [Inject] private IScreenFader _screenFader;

        protected override void OnClicked() { }

        protected override void OnPointerEntered()
        {
            _screenFader.SetFadeDuration(_fadeDuration);
            _screenFader.SetFadeInTargetAlpha(_fadeInTargetAlpha);

            _screenFader
                .FadeIn(destroyCancellationToken, true)
                .SuppressCancellationThrow()
                .Forget();
        }

        protected override void OnPointerExited()
        {
            _screenFader
                .FadeOut(destroyCancellationToken)
                .SuppressCancellationThrow()
                .Forget();
        }
    }
}