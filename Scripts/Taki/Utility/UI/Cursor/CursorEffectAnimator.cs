using DG.Tweening;
using UnityEngine;

namespace Taki.Utility
{
    public class CursorEffectAnimator : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _targetTransform;
        [SerializeField] private Vector3 _rotationAxis = Vector3.forward;
        [SerializeField] private float _rotationDegrees = 360f;
        [SerializeField] private float _scaleMultiplier = 1.5f;
        [SerializeField] private float _animationDuration = 1.0f;

        [SerializeField] private Ease _scaleEaseType = Ease.OutExpo;
        [SerializeField] private Ease _fadeEaseType = Ease.InQuad;
        [SerializeField] private bool _ignoreTimeScale = false;

        private Vector3 _initialScale;

        private void Awake()
        {
            _initialScale = _targetTransform.localScale;
            _targetTransform.localScale = Vector3.zero;

            PlayAnimation();
        }

        private void PlayAnimation()
        {
            _canvasGroup.alpha = 1f;

            Sequence sequence = DOTween.Sequence();

            sequence.Append(
                _targetTransform.DOLocalRotate(
                    _rotationAxis.normalized * _rotationDegrees,
                    _animationDuration,
                    RotateMode.LocalAxisAdd)
                .SetEase(Ease.Linear)
            );

            sequence.Join(
                _targetTransform.DOScale(
                    _initialScale * _scaleMultiplier,
                    _animationDuration)
                .SetEase(_scaleEaseType)
            );

            sequence.Join(
                _canvasGroup.DOFade(0f, _animationDuration)
                .SetEase(_fadeEaseType)
            );

            sequence.SetUpdate(_ignoreTimeScale)
                .SetLink(gameObject) 
                .OnComplete(() => {
                    if (this != null && gameObject != null) 
                    {
                        Destroy(gameObject);
                    }
                });
        }
    }
}