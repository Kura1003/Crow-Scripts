using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Taki.Utility
{
    public class ImageSequenceFader : MonoBehaviour
    {
        [SerializeField] private List<Graphic> _graphics = new();
        [SerializeField] private List<CanvasGroup> _canvasGroups = new();
        [SerializeField] private float _fadeDuration = 1.0f;
        [SerializeField] private float _interval = 0.5f;

        public void SetImagesTransparent()
        {
            foreach (var group in _canvasGroups)
            {
                if (group is null) continue;

                group.alpha = 0f;
            }

            foreach (var graphic in _graphics)
            {
                if (graphic is null) continue;

                var color = graphic.color;
                color.a = 0f;
                graphic.color = color;
            }
        }

        public async UniTask PlayFadeSequenceAsync(CancellationToken token)
        {
            foreach (var group in _canvasGroups)
            {
                if (group is null) continue;

                FadeInCanvasGroup(group, token).Forget();
                await UniTask.WaitForSeconds(_interval, cancellationToken: token);
            }

            foreach (var graphic in _graphics)
            {
                if (graphic is null) continue;

                FadeInGraphic(graphic, token).Forget();
                await UniTask.WaitForSeconds(_interval, cancellationToken: token);
            }
        }

        private UniTask FadeInCanvasGroup(CanvasGroup group, CancellationToken token)
        {
            return group.DOFade(1f, _fadeDuration)
                .SetUpdate(true)
                .ToUniTask(cancellationToken: token);
        }

        private UniTask FadeInGraphic(Graphic graphic, CancellationToken token)
        {
            return graphic.DOFade(1f, _fadeDuration)
                .SetUpdate(true)
                .ToUniTask(cancellationToken: token);
        }
    }
}