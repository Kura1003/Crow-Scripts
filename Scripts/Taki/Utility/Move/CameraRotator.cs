using System;
using System.Collections.Generic;
using Taki.Audio;
using UnityEngine;

namespace Taki.Utility
{
    public class CameraRotator : MonoBehaviour
    {
        [SerializeField] private Transform _rotationAxis;
        [SerializeField] private float _rotationSpeed = 90.0f;
        [SerializeField] private float _angleThreshold = 30.0f;
        [SerializeField, Range(0f, 1f)] private float _decayRate = 0.9f;

        private float _totalAngleMoved = 0f;
        private Vector3 _currentRotationDirection = Vector3.zero;
        private float _currentRotationMagnitude = 0f;
        private bool _isInputActive = false;

        public bool IsLocked { get; set; } = false;

        private static readonly Dictionary<KeyCode, Vector3> _keyDirectionMap =
            new Dictionary<KeyCode, Vector3>
        {
            { KeyCode.A, Vector3.forward },
            { KeyCode.D, Vector3.back },
            { KeyCode.W, Vector3.left },
            { KeyCode.S, Vector3.right }
        };

        private void LateUpdate()
        {
            if (IsLocked) return;

            HandleInput();
            ApplyRotation();

            if (Input.GetKeyDown(KeyCode.Q))
            {
                ResetCameraAngle();
            }
        }

        private void HandleInput()
        {
            var inputDirection = Vector3.zero;

            foreach (var keyPair in _keyDirectionMap)
            {
                if (Input.GetKey(keyPair.Key))
                {
                    inputDirection += keyPair.Value;
                }
            }

            _isInputActive = inputDirection.sqrMagnitude > 0.001f;

            if (_isInputActive)
            {
                _currentRotationDirection = inputDirection.normalized;
                _currentRotationMagnitude = _rotationSpeed;
            }
            else
            {
                ApplyDecay();
            }
        }

        private void ApplyRotation()
        {
            if (_currentRotationMagnitude <= 0f) return;

            var deltaTime = Time.deltaTime;
            var angleThisFrame = _currentRotationMagnitude * deltaTime;

            _rotationAxis.rotation *= Quaternion.AngleAxis(
                angleThisFrame,
                _currentRotationDirection);

            _totalAngleMoved += angleThisFrame;

            if (_totalAngleMoved >= _angleThreshold)
            {
                OnAngleThresholdExceeded();
                _totalAngleMoved = 0f;
            }
        }

        private void ApplyDecay()
        {
            _currentRotationMagnitude *= _decayRate;

            if (_currentRotationMagnitude < 0.01f)
            {
                _currentRotationMagnitude = 0f;
                _currentRotationDirection = Vector3.zero;
            }
        }

        public void ResetCameraAngle()
        {
            _rotationAxis.rotation = Quaternion.identity;
            _totalAngleMoved = 0f;
            _currentRotationDirection = Vector3.zero;
            _currentRotationMagnitude = 0f;
            _isInputActive = false;

            AudioManager.Instance.Play("鳩時計", gameObject).SetVolume(0.5f);
        }

        private void OnAngleThresholdExceeded()
        {
            AudioManager.Instance.Play("GearTurn", gameObject).SetVolume(0.5f);
        }
    }
}