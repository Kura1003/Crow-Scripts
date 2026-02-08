using UnityEngine;
using UnityEngine.UI;

namespace Taki.Utility
{
    [RequireComponent(typeof(Image))]
    public class RectImageHitDetector : MonoBehaviour, ICanvasRaycastFilter
    {
        [SerializeField, Range(0f, 1f)]
        private float _hitWidthRatio = 1.0f;
        [SerializeField, Range(0f, 1f)]
        private float _hitHeightRatio = 1.0f;

        private Image _image;
        private RectTransform _rectTransform;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _rectTransform = GetComponent<RectTransform>();

            if (!_image.raycastTarget)
            {
                _image.raycastTarget = true;
            }
        }

        public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            if (!_image.raycastTarget) return false;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rectTransform,
                screenPoint,
                eventCamera,
                out Vector2 localPoint
            );

            Rect rect = _rectTransform.rect;

            float halfHitWidth = (rect.width * _hitWidthRatio) / 2f;
            float halfHitHeight = (rect.height * _hitHeightRatio) / 2f;

            float centerX = rect.x + rect.width / 2f;
            float centerY = rect.y + rect.height / 2f;

            return Mathf.Abs(localPoint.x - centerX) <= halfHitWidth &&
                   Mathf.Abs(localPoint.y - centerY) <= halfHitHeight;
        }
    }
}