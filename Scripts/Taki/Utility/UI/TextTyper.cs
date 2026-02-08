using Cysharp.Threading.Tasks;
using System.Text;
using System.Threading;
using Taki.Audio;
using TMPro;
using UnityEngine;

namespace Taki.Utility
{
    public class TextTyper : MonoBehaviour
    {
        [SerializeField] private TMP_Text _targetText;
        [SerializeField] private float _characterInterval = 0.05f;
        [SerializeField] private bool _ignoreTimeScale = false;
        [SerializeField, TextArea(1, 3)]
        private string _textToType;

        private StringBuilder _stringBuilder = new StringBuilder();
        private string _fullTextCache;

        private const string _warmUpCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        private void Awake()
        {
            if (_targetText != null)
            {
                WarmUpTextMeshPro(destroyCancellationToken).Forget();
                _targetText.text = string.Empty;
            }
        }

        private async UniTask WarmUpTextMeshPro(CancellationToken token)
        {
            Color originalColor = _targetText.color;
            _targetText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

            _targetText.text = _warmUpCharacters;
            _targetText.ForceMeshUpdate(true, true);

            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, token);

            _targetText.color = originalColor;
            _targetText.text = string.Empty;
            _targetText.ForceMeshUpdate(true, true);
        }

        public async UniTask Type(CancellationToken token)
        {
            _stringBuilder.Clear();
            _stringBuilder.Append(_textToType);
            _fullTextCache = _stringBuilder.ToString();

            _targetText.text = _fullTextCache;
            _targetText.maxVisibleCharacters = 0;

            for (int i = 0; i < _textToType.Length; i++)
            {
                _targetText.maxVisibleCharacters = i + 1;
                AudioManager.Instance.Play("TextType", gameObject).SetVolume(0.5f);

                await UniTask
                    .WaitForSeconds(
                    _characterInterval,
                    _ignoreTimeScale,
                    cancellationToken: token);

                if (token.IsCancellationRequested) return;
            }
        }

        public async UniTask Clear(CancellationToken token)
        {
            int currentVisibleCharacters = _targetText.maxVisibleCharacters;

            while (currentVisibleCharacters > 0)
            {
                _targetText.maxVisibleCharacters = --currentVisibleCharacters;
                AudioManager.Instance.Play("TextType", gameObject).SetVolume(0.5f);

                await UniTask
                    .WaitForSeconds(
                    _characterInterval,
                    _ignoreTimeScale,
                    cancellationToken: token);

                if (token.IsCancellationRequested) return;
            }

            _targetText.text = string.Empty;
        }

        public void ClearImmediately()
        {
            _targetText.text = string.Empty;
            _targetText.maxVisibleCharacters = 0;
        }
    }
}
