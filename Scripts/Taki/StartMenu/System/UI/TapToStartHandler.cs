using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using TMPro;
using UnityEngine;

namespace Taki.StartMenu.System
{
    public class TapToStartHandler : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textMesh;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _fadeDuration = 0.5f;

        private bool _isLocked;

        public bool IsLocked
        {
            get => _isLocked;
            set => _isLocked = value;
        }

        private void Awake()
        {
            if (_textMesh != null) _textMesh.text = "Tap";
            if (_canvasGroup != null) _canvasGroup.alpha = 0f;
        }

        public async UniTask WaitTapAsync(CancellationToken token)
        {
            await _canvasGroup.DOFade(1f, _fadeDuration)
                .SetUpdate(true)
                .ToUniTask(cancellationToken: token);

            await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0) && !_isLocked, cancellationToken: token);

            _canvasGroup.DOFade(0f, _fadeDuration)
                .SetUpdate(true)
                .ToUniTask(cancellationToken: token)
                .Forget();
        }
    }
}