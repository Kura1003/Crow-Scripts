using R3;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Taki.RubiksCube.System;
using Taki.Utility;
using UnityEngine;
using VContainer;

namespace Taki.Main.View
{
    public class TextLineAnimationManager : MonoBehaviour
    {
        [SerializeField] private GameObject _textPrefab;
        [SerializeField] private Transform _origin;
        [SerializeField] private int _baseCount = 5;
        [SerializeField] private float _baseY = -30.0f;
        [SerializeField] private float _heightAdjustmentFactor = 1.5f;
        [SerializeField] private float _spacing = 2.0f;
        [SerializeField] private int _maxPoolSize = 300;
        [SerializeField] private float _randomRadius = 5.0f;
        [SerializeField] private float _intervalSeconds = 0.2f;
        [SerializeField] private float _visibleDuration = 2.0f;
        [SerializeField] private float _minAngleDifference = 30.0f;

        private readonly Stack<GameObject> _poolStack = new();
        private readonly List<GameObject> _instantiatedObjects = new();

        private CancellationTokenSource _loopCts;
        private int _currentCount;
        private float _lastYRotation;

        [Inject] private readonly ICubeSizeManager _cubeSizeManager;
        [Inject] private readonly ICubeFactory _cubeFactory;

        private void Start()
        {
            _cubeFactory.OnCubeCreated
                .Subscribe(_ =>
                {
                    AdjustParametersBySize();
                }).AddTo(this);
        }

        public async UniTask PrewarmPoolAsync(CancellationToken token)
        {
            while (_instantiatedObjects.Count < _maxPoolSize)
            {
                if (token.IsCancellationRequested) return;

                int targetBatch = Mathf.Min(100, _maxPoolSize - _instantiatedObjects.Count);

                for (int i = 0; i < targetBatch; i++)
                {
                    CreateNewInstance();
                }

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }

        private GameObject CreateNewInstance()
        {
            GameObject go = Instantiate(_textPrefab, _origin);
            go.SetActive(false);
            _instantiatedObjects.Add(go);
            _poolStack.Push(go);
            return go;
        }

        private GameObject GetFromPool()
        {
            if (_poolStack.Count > 0)
            {
                return _poolStack.Pop();
            }

            if (_instantiatedObjects.Count < _maxPoolSize)
            {
                return CreateNewInstance();
            }

            return null;
        }

        private void ReturnToPool(GameObject go)
        {
            if (go is null) return;

            go.SetActive(false);
            _poolStack.Push(go);
        }

        private void AdjustParametersBySize()
        {
            float factor = _cubeSizeManager.GetSizeScaleFactor();
            _currentCount = Mathf.Max(1, Mathf.RoundToInt(_baseCount * factor));

            Vector3 pos = _origin.localPosition;
            pos.y = _baseY * factor * _heightAdjustmentFactor;
            _origin.localPosition = pos;
        }

        public async UniTask PlayInfiniteSequence(float loopIntervalSeconds)
        {
            StopInfiniteSequence();
            _loopCts = new CancellationTokenSource();
            var token = _loopCts.Token;

            while (!token.IsCancellationRequested)
            {
                PlaySequence(token).Forget();
                await UniTask.WaitForSeconds(loopIntervalSeconds, cancellationToken: token);
            }
        }

        public void StopLoopOnly()
        {
            if (_loopCts != null)
            {
                _loopCts.Cancel();
                _loopCts.Dispose();
                _loopCts = null;
            }
        }

        public void StopInfiniteSequence()
        {
            StopLoopOnly();
            Cleanup();
        }

        public async UniTask PlaySequence(CancellationToken token)
        {
            var activeObjects = PrepareLineFromPool();

            foreach (var obj in activeObjects)
            {
                if (token.IsCancellationRequested) return;

                obj.SetActive(true);
                await UniTask.WaitForSeconds(_intervalSeconds, cancellationToken: token);
            }

            if (token.IsCancellationRequested) return;

            await UniTask.WaitForSeconds(_visibleDuration, cancellationToken: token);

            foreach (var obj in activeObjects)
            {
                if (token.IsCancellationRequested) return;

                ReturnToPool(obj);
                await UniTask.WaitForSeconds(_intervalSeconds, cancellationToken: token);
            }
        }

        private List<GameObject> PrepareLineFromPool()
        {
            if (_origin is null) return new List<GameObject>();

            float randomX = (float)RandomUtility.Range(-_randomRadius, _randomRadius);
            float randomZ = (float)RandomUtility.Range(-_randomRadius, _randomRadius);

            Vector3 centerPosition = _origin.position + (_origin.rotation * new Vector3(randomX, 0, randomZ));
            Quaternion finalRotation = CalculateFinalRotation();
            Vector3[] points = LinePointCalculator.GenerateLinePoints(Vector3.zero, _currentCount, _spacing, GridPlane.XZ);

            var activeSelection = new List<GameObject>();

            for (int i = 0; i < _currentCount; i++)
            {
                GameObject go = GetFromPool();
                if (go is null) break;

                go.transform.SetPositionAndRotation(centerPosition + (finalRotation * points[i]), finalRotation);
                activeSelection.Add(go);
            }

            return activeSelection;
        }

        private Quaternion CalculateFinalRotation()
        {
            float randomY = GetRandomYRotation();
            return _origin.rotation * Quaternion.Euler(90f, randomY, 0f);
        }

        private float GetRandomYRotation()
        {
            float newAngle;
            int attempts = 0;
            int maxAttempts = 10;

            do
            {
                newAngle = (float)RandomUtility.Range(0.0, 360.0);
                attempts++;
            }
            while (CanRetry(attempts, maxAttempts) && !IsAngleDifferenceValid(newAngle));

            _lastYRotation = newAngle;
            return newAngle;
        }

        private bool IsAngleDifferenceValid(float newAngle)
        {
            return Mathf.Abs(Mathf.DeltaAngle(newAngle, _lastYRotation)) >= _minAngleDifference;
        }

        private bool CanRetry(int attempts, int maxAttempts)
        {
            return attempts < maxAttempts;
        }

        private void Cleanup()
        {
            _poolStack.Clear();
            foreach (var obj in _instantiatedObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                    _poolStack.Push(obj);
                }
            }
        }
    }
}