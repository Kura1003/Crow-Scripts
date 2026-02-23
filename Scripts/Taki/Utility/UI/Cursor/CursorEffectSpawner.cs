using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Taki.Utility
{
    [RequireComponent(typeof(Canvas), typeof(RectTransform))]
    public class CursorEffectSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _cursorEffectPrefab;
        [SerializeField] private float _spawnInterval = 0.1f;
        [SerializeField] private bool _canSpawn = true;

        private Canvas _canvas;
        private RectTransform _rectTransform;
        private bool _isSpawning;

        public bool CanSpawn
        {
            get => _canSpawn;
            set
            {
                _canSpawn = value;
            }
        }

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (!_canSpawn) return;

            if (Input.GetMouseButtonDown(0))
            {
                _isSpawning = true;
                SpawnEffectAsync(
                    Input.mousePosition, 
                    destroyCancellationToken)
                    .SuppressCancellationThrow()
                    .Forget();
            }

            if (Input.GetMouseButtonUp(0))
            {
                _isSpawning = false;
            }
        }

        private async UniTask SpawnEffectAsync(Vector2 initialPosition, CancellationToken token)
        {
            while (_isSpawning && _canSpawn && !token.IsCancellationRequested)
            {
                Vector2 localPoint;
                Camera cameraToUse = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;

                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _rectTransform,
                    initialPosition,
                    cameraToUse,
                    out localPoint);

                SpawnEffect(localPoint);

                await UniTask.WaitForSeconds(_spawnInterval, ignoreTimeScale: true, cancellationToken: token);

                initialPosition = Input.mousePosition;
            }
        }

        private void SpawnEffect(Vector2 localPoint)
        {
            GameObject instance = Instantiate(_cursorEffectPrefab, _rectTransform);
            RectTransform instanceRect = instance.GetComponent<RectTransform>();
            instanceRect.anchoredPosition = localPoint;
        }
    }
}