using DG.Tweening;
using UnityEngine;

namespace Taki.Utility
{
    public class PointerScaleAnimator : PointerEventHandler
    {
        [SerializeField] private float _animationDuration = 0.2f;
        [SerializeField] private float _scaleMultiplier = 1.1f;
        [SerializeField] private Ease _easeType = Ease.OutQuad;
        [SerializeField] private bool _ignoreTimeScale = false;

        private Vector3 _originalScale;
        private Tween _currentTween;

        protected override void Awake()
        {
            base.Awake();
            _originalScale = transform.localScale;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _currentTween?.Kill();
        }

        protected override void OnClicked() { }

        protected override void OnPointerEntered()
        {
            _currentTween?.Kill();

            _currentTween = transform.DOScale(
                    _originalScale * _scaleMultiplier,
                    _animationDuration)
                .SetEase(_easeType)
                .SetUpdate(_ignoreTimeScale);
        }

        protected override void OnPointerExited()
        {
            _currentTween?.Kill();

            _currentTween = transform.DOScale(
                    _originalScale,
                    _animationDuration)
                .SetEase(_easeType)
                .SetUpdate(_ignoreTimeScale);
        }
    }
}