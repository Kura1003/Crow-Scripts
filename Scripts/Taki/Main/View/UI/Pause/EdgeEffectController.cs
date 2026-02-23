using Taki.Utility;
using VContainer;
using XPostProcessing;
using DG.Tweening;
using UnityEngine;

namespace Taki.Main.View
{
    public class EdgeEffectController : PointerEventHandler
    {
        [SerializeField] private float _duration = 0.5f;
        [SerializeField] private Ease _enterEase = Ease.OutCubic;
        [SerializeField] private Ease _exitEase = Ease.InCubic;

        [Inject] private readonly IPostProcessEffectProvider _postProcessProvider;

        private EdgeDetectionRobertsNeon _edgeEffect;
        private Tweener _currentTween;

        private void Start()
        {
            _edgeEffect = _postProcessProvider.GetEffect<EdgeDetectionRobertsNeon>();
            _edgeEffect.BackgroundFade.value = 0f;
            _edgeEffect.active = false;
        }

        protected override void OnPointerEntered()
        {
            AnimateFade(1f, _enterEase);
        }

        protected override void OnPointerExited()
        {
            AnimateFade(0f, _exitEase);
        }

        protected override void OnClicked() { }

        public void CancelAnimation(bool resetToZero = true)
        {
            _currentTween?.Kill();

            if (resetToZero)
            {
                _edgeEffect.BackgroundFade.value = 0f;
            }
        }

        private void AnimateFade(float endValue, Ease ease)
        {
            _currentTween?.Kill();

            _currentTween = DOTween.To(
                () => _edgeEffect.BackgroundFade.value,
                x => _edgeEffect.BackgroundFade.value = x,
                endValue,
                _duration
            ).SetEase(ease)
            .SetUpdate(true);
        }
    }
}