using DG.Tweening;
using UnityEngine;

namespace Taki.Utility
{
    public class PointerMoveAnimator : PointerEventHandler
    {
        [SerializeField] private Vector3 _moveDirection = Vector3.up;
        [SerializeField] private float _moveDistance = 0.5f;
        [SerializeField] private float _animationDuration = 0.2f;
        [SerializeField] private Ease _easeType = Ease.OutQuad;
        [SerializeField] private bool _ignoreTimeScale = false;

        private Vector3 _originalPosition;
        private Vector3 _targetPosition;
        private Tween _currentTween;

        protected override void Awake()
        {
            base.Awake();
            _originalPosition = transform.localPosition;
            _targetPosition
                = _originalPosition + (_moveDirection.normalized * _moveDistance);
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

            _currentTween = transform.DOLocalMove(
                _targetPosition,
                _animationDuration)
                .SetEase(_easeType)
                .SetUpdate(_ignoreTimeScale);
        }

        protected override void OnPointerExited()
        {
            _currentTween?.Kill();

            _currentTween = transform.DOLocalMove(
                _originalPosition,
                _animationDuration)
                .SetEase(_easeType)
                .SetUpdate(_ignoreTimeScale);
        }
    }
}