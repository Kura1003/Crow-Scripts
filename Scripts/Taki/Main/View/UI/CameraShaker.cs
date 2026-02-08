using DG.Tweening;
using Taki.Main.System;
using Taki.Utility;
using UnityEngine;
using VContainer;
using XPostProcessing;

namespace Taki.Main.View
{
    public class CameraShaker : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private float _shakeDuration = 0.5f;
        [SerializeField] private float _shakeStrength = 0.5f;
        [SerializeField] private int _shakeVibrato = 10;
        [SerializeField] private float _shakeRandomness = 90f;
        [SerializeField] private Ease _glitchEase = Ease.OutQuad;

        [Inject] private readonly IPauseEvents _pauseEvents;
        [Inject] private readonly IPostProcessEffectProvider _postProcessEffectProvider;

        private Transform _cameraTransform;
        private Vector3 _originalPosition;
        private Tween _currentShakeTween;
        private Tween _currentGlitchTween;
        private GlitchWaveJitter _glitchWaveJitter;

        private void Start()
        {
            _cameraTransform = Camera.main.transform;
            SetOriginalPosition();

            _glitchWaveJitter = _postProcessEffectProvider.GetEffect<GlitchWaveJitter>();

            if (_glitchWaveJitter != null)
            {
                _glitchWaveJitter.amount.value = 0f;
            }
        }

        public void SetOriginalPosition()
        {
            _originalPosition = _cameraTransform.localPosition;
        }

        public void ShakeCamera()
        {
            StopCameraShake();
            HandleCameraShake();
            HandleGlitchShake();
        }

        private void HandleCameraShake()
        {
            _canvas.renderMode = RenderMode.ScreenSpaceCamera;
            _currentShakeTween = _cameraTransform.DOShakePosition(
                    _shakeDuration,
                    _shakeStrength,
                    _shakeVibrato,
                    _shakeRandomness,
                    false,
                    true)
                .SetUpdate(true)
                .OnKill(() =>
                {
                    if (!_pauseEvents.IsPaused)
                    {
                        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                        _cameraTransform.localPosition = _originalPosition;
                    }
                });
        }

        private void HandleGlitchShake()
        {
            _currentGlitchTween?.Kill();

            _glitchWaveJitter.amount.value = 1f;

            _currentGlitchTween = DOTween.To(
                () => _glitchWaveJitter.amount.value,
                x => _glitchWaveJitter.amount.value = x,
                0f,
                _shakeDuration
            ).SetEase(_glitchEase).SetUpdate(true);
        }

        private void StopCameraShake()
        {
            _currentShakeTween?.Kill();
            _currentShakeTween = null;

            _currentGlitchTween?.Kill();
            _currentGlitchTween = null;

            if (_glitchWaveJitter != null)
            {
                _glitchWaveJitter.amount.value = 0f;
            }
        }
    }
}