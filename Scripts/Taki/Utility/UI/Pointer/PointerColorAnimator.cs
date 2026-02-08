using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Taki.Utility
{
    [Serializable]
    public struct GraphicColorEntry
    {
        public Graphic Graphic;
        public Color HoverColor;
    }

    public class PointerColorAnimator : PointerEventHandler
    {
        [SerializeField] private List<GraphicColorEntry> _graphicEntries = new();
        [SerializeField] private float _animationDuration = 0.2f;
        [SerializeField] private Ease _easeType = Ease.OutQuad;
        [SerializeField] private bool _ignoreTimeScale = false;

        private readonly Dictionary<Graphic, Color> _originalColors = new();
        private readonly List<Tween> _currentTweens = new();

        protected override void Awake()
        {
            base.Awake();
            InitializeGraphicEntries(_graphicEntries);
        }

        public void SetGraphicEntries(
            List<GraphicColorEntry> newGraphicEntries, 
            bool append = false)
        {
            KillCurrentTweens();

            if (!append)
            {
                _graphicEntries.Clear();
                _originalColors.Clear();
            }

            for (int i = 0; i < newGraphicEntries.Count; i++)
            {
                _graphicEntries.Add(newGraphicEntries[i]);
            }

            InitializeGraphicEntries(newGraphicEntries);
        }

        private void InitializeGraphicEntries(List<GraphicColorEntry> entries)
        {
            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                if (entry.Graphic != null)
                {
                    _originalColors[entry.Graphic] = entry.Graphic.color;
                }
            }
        }

        public void ResetColorsInstantly()
        {
            KillCurrentTweens();

            for (int i = 0; i < _graphicEntries.Count; i++)
            {
                var entry = _graphicEntries[i];
                if (entry.Graphic != null && 
                    _originalColors.TryGetValue(entry.Graphic, out var originalColor))
                {
                    entry.Graphic.color = originalColor;
                }
            }
        }

        protected override void OnClicked() { }

        protected override void OnPointerEntered()
        {
            KillCurrentTweens();

            for (int i = 0; i < _graphicEntries.Count; i++)
            {
                var entry = _graphicEntries[i];
                if (entry.Graphic != null && _originalColors.ContainsKey(entry.Graphic))
                {
                    Tween newTween = entry.Graphic.DOColor(entry.HoverColor, _animationDuration)
                        .SetEase(_easeType)
                        .SetUpdate(_ignoreTimeScale);

                    _currentTweens.Add(newTween);
                }
            }
        }

        protected override void OnPointerExited()
        {
            KillCurrentTweens();

            for (int i = 0; i < _graphicEntries.Count; i++)
            {
                var entry = _graphicEntries[i];
                if (entry.Graphic != null && 
                    _originalColors.TryGetValue(entry.Graphic, out var originalColor))
                {
                    Tween newTween = entry.Graphic.DOColor(originalColor, _animationDuration)
                        .SetEase(_easeType)
                        .SetUpdate(_ignoreTimeScale);

                    _currentTweens.Add(newTween);
                }
            }
        }

        private void KillCurrentTweens()
        {
            for (int i = _currentTweens.Count - 1; i >= 0; i--)
            {
                var tween = _currentTweens[i];
                if (tween != null && tween.IsActive())
                {
                    tween.Kill();
                }
            }
            _currentTweens.Clear();
        }
    }
}