using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Taki.Main.View
{
    public class DualCanvasGroupFader : MonoBehaviour
    {
        [SerializeField] private List<CanvasGroup> _mainGroups = new();
        [SerializeField] private List<CanvasGroup> _oppositeGroups = new();

        [SerializeField] private float _fadeDuration = 1.0f;
        [SerializeField] private Ease _fadeEase = Ease.OutQuad;

        [SerializeField] private bool _ignoreTimeScale = false;

        private void Awake()
        {
            foreach (var canvasGroup in _oppositeGroups)
            {
                canvasGroup.alpha = 0;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
            }

            foreach (var canvasGroup in _mainGroups)
            {
                canvasGroup.alpha = 1;
                canvasGroup.blocksRaycasts = true;
                canvasGroup.interactable = true;
            }
        }

        public async UniTask FadeIn(CancellationToken token)
        {
            var fadeTasks = new List<UniTask>();

            foreach (var canvasGroup in _mainGroups)
            {
                var task = canvasGroup.DOFade(1.0f, _fadeDuration)
                    .SetEase(_fadeEase)
                    .SetUpdate(_ignoreTimeScale)
                    .ToUniTask(cancellationToken: token);

                fadeTasks.Add(task);
            }

            foreach (var canvasGroup in _oppositeGroups)
            {
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;

                var task = canvasGroup.DOFade(0.0f, _fadeDuration)
                    .SetEase(_fadeEase)
                    .SetUpdate(_ignoreTimeScale)
                    .ToUniTask(cancellationToken: token);

                fadeTasks.Add(task);
            }

            await UniTask.WhenAll(fadeTasks);

            foreach (var canvasGroup in _mainGroups)
            {
                canvasGroup.blocksRaycasts = true;
                canvasGroup.interactable = true;
            }
        }

        public async UniTask FadeOut(CancellationToken token)
        {
            var fadeTasks = new List<UniTask>();

            foreach (var canvasGroup in _oppositeGroups)
            {
                var task = canvasGroup.DOFade(1.0f, _fadeDuration)
                    .SetEase(_fadeEase)
                    .SetUpdate(_ignoreTimeScale)
                    .ToUniTask(cancellationToken: token);

                fadeTasks.Add(task);
            }

            foreach (var canvasGroup in _mainGroups)
            {
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;

                var task = canvasGroup.DOFade(0.0f, _fadeDuration)
                    .SetEase(_fadeEase)
                    .SetUpdate(_ignoreTimeScale)
                    .ToUniTask(cancellationToken: token);

                fadeTasks.Add(task);
            }

            await UniTask.WhenAll(fadeTasks);

            foreach (var canvasGroup in _oppositeGroups)
            {
                canvasGroup.blocksRaycasts = true;
                canvasGroup.interactable = true;
            }
        }
    }
}