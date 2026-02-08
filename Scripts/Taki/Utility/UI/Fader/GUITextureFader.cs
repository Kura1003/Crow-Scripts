using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;

namespace Taki.Utility
{
    internal class GUITextureFader
    {
        private readonly Texture2D _texture;
        private Rect _positionRect;
        private Color _currentColor;
        private float _fadeDuration = 1.0f;
        private float _fadeInTargetAlpha = 1.0f;
        private bool _isDrawing = true;
        private Tween _fadeTween;

        internal GUITextureFader(Texture2D texture, Rect position)
        {
            _isDrawing = true;
            _texture = texture;
            _positionRect = position;
            _currentColor = new Color(1f, 1f, 1f, 0f);
        }

        internal void SetTextureColor(Color color)
        {
            _currentColor = color;
        }

        internal void SetFadeDuration(float duration)
        {
            _fadeDuration = Mathf.Max(0.01f, duration);
        }

        internal void SetFadeInTargetAlpha(float alpha)
        {
            _fadeInTargetAlpha = Mathf.Clamp01(alpha);
        }

        internal async UniTask FadeIn(CancellationToken token, bool keepDrawingAfterFade = false)
        {
            _fadeTween?.Kill();
            _isDrawing = true;

            _fadeTween = DOTween.To(() => _currentColor.a, x => _currentColor.a = x, _fadeInTargetAlpha, _fadeDuration)
                .SetUpdate(true)
                .SetEase(Ease.OutQuad);

            await _fadeTween.ToUniTask(cancellationToken: token);

            if (!keepDrawingAfterFade)
            {
                _isDrawing = false;
            }
        }

        internal async UniTask FadeOut(CancellationToken token)
        {
            _fadeTween?.Kill();
            _isDrawing = true;

            _fadeTween = DOTween.To(() => _currentColor.a, x => _currentColor.a = x, 0f, _fadeDuration)
                .SetUpdate(true)
                .SetEase(Ease.InQuad);

            await _fadeTween.ToUniTask(cancellationToken: token);

            _isDrawing = false;
        }

        internal void SetAlphaToZero()
        {
            _currentColor.a = 0;
        }

        internal void Draw()
        {
            if (!_isDrawing)
            {
                return;
            }

            _positionRect = new Rect(0, 0, Screen.width, Screen.height);
            Color originalGuiColor = GUI.color;
            GUI.color = _currentColor;
            GUI.DrawTexture(_positionRect, _texture);
            GUI.color = originalGuiColor;
        }
    }
}