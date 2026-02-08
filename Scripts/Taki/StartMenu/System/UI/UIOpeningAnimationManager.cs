using System.Collections.Generic;
using Taki.Utility;
using UnityEngine;

namespace Taki.StartMenu.System
{
    public class UIOpeningAnimationManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private GameObject _uiElementPrefab;
        [SerializeField] private RectTransform _container;
        [SerializeField] private GridPlane _plane = GridPlane.XY;

        [Header("Layout Settings")]
        [SerializeField] private float _startY = 0f;
        [SerializeField] private float _spacing = 100f;
        [SerializeField] private int _elementCount = 5;

        [Header("Animation Settings")]
        [SerializeField] private float _moveSpeed = 50f;
        [SerializeField] private Vector2 _moveDirection = Vector2.right;
        [SerializeField] private bool _isAnimationActive = false;

        private readonly List<RectTransform> _generatedElements = new();
        private float _accumulatedDistance = 0f;
        private Vector3[] _targetPositions;

        private void Start()
        {
            GenerateUIElements();
        }

        private void Update()
        {
            if (!_isAnimationActive) return;

            Vector2 movement = CalculateMovement();
            _accumulatedDistance += Mathf.Abs(movement.x);

            ApplyMovement(movement);

            if (_accumulatedDistance >= _spacing)
            {
                ShiftElements();
                _accumulatedDistance -= _spacing;
            }
        }

        private Vector2 CalculateMovement()
        {
            return _moveDirection.normalized * (_moveSpeed * Time.unscaledDeltaTime);
        }

        private void ApplyMovement(Vector2 movement)
        {
            foreach (var element in _generatedElements)
            {
                if (element is not null)
                {
                    element.anchoredPosition += movement;
                }
            }
        }

        private void ShiftElements()
        {
            if (_generatedElements.Count == 0) return;

            RectTransform first = _generatedElements[0];
            _generatedElements.RemoveAt(0);
            Destroy(first.gameObject);

            var go = Instantiate(_uiElementPrefab, _container);
            go.SetActive(true);

            if (go.TryGetComponent<RectTransform>(out var rect))
            {
                _generatedElements.Add(rect);
            }

            ApplyTargetPositions();
        }

        private void ApplyTargetPositions()
        {
            for (int i = 0; i < _generatedElements.Count; i++)
            {
                if (_generatedElements[i] is not null)
                {
                    _generatedElements[i].anchoredPosition =
                        new Vector2(_targetPositions[i].x, _targetPositions[i].y);
                }
            }
        }

        private void GenerateUIElements()
        {
            ClearElements();

            Vector2 center = new Vector2(0f, _startY);

            _targetPositions =
                LinePointCalculator.GenerateLinePoints(
                    center,
                    _elementCount,
                    _spacing,
                    _plane
                );

            foreach (var pos in _targetPositions)
            {
                var go = Instantiate(_uiElementPrefab, _container);
                go.SetActive(true);

                if (go.TryGetComponent<RectTransform>(out var rect))
                {
                    rect.anchoredPosition = new Vector2(pos.x, pos.y);
                    _generatedElements.Add(rect);
                }
            }
        }

        private void ClearElements()
        {
            foreach (var element in _generatedElements)
            {
                if (element is not null)
                {
                    Destroy(element.gameObject);
                }
            }

            _generatedElements.Clear();
            _accumulatedDistance = 0f;
        }
    }
}
