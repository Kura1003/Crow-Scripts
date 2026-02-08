using System;
using UnityEngine;
using UnityEngine.UI;

namespace Taki.Utility
{
    [RequireComponent(typeof(Image))]
    public class CircularImageHitDetector : MonoBehaviour, ICanvasRaycastFilter
    {
        [SerializeField, Range(0f, 1f)] 
        private float _hitRadiusRatio = 1.0f;

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

        public bool IsRaycastLocationValid(
            Vector2 screenPoint, 
            Camera eventCamera)
        {
            if (!_image.raycastTarget)
            {
                return false;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rectTransform,
                screenPoint,
                eventCamera,
                out Vector2 localPoint
            );

            float width = _rectTransform.rect.width;
            float height = _rectTransform.rect.height;

            float baseRadius = Mathf.Min(width, height) / 2f;
            float actualRadius = baseRadius * _hitRadiusRatio;

            float distanceFromCenter = localPoint.magnitude;

            return distanceFromCenter <= actualRadius;
        }
    }
}