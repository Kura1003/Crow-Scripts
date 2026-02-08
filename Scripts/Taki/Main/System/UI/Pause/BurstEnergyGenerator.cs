using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Taki.Utility;
using UnityEngine;

namespace Taki.Main.System
{
    public class BurstEnergyGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject _burstItemPrefab;

        [Header("Radiator Settings")]
        [SerializeField] private int _energyCount = 5;
        [SerializeField] private float _radiateRadius = 5.0f;
        [SerializeField] private GridPlane _radiatePlane = GridPlane.XY;

        public IReadOnlyList<Transform> GeneratedTransforms => _generatedTransforms;
        public IReadOnlyList<Vector3> BurstPoints => _burstPoints;
        public IReadOnlyList<Vector3> OverdrivePoints => _overdrivePoints;

        private readonly List<Transform> _generatedTransforms = new();
        private readonly List<Vector3> _burstPoints = new();
        private readonly List<Vector3> _overdrivePoints = new();

        public async UniTask<int> ChargeAndGenerate(int currentTotalCount, CancellationToken token)
        {
            ReleaseEnergy();
            if (_burstItemPrefab is null) return currentTotalCount;

            if (_burstPoints is List<Vector3> bpList) bpList.Capacity = _energyCount;
            if (_overdrivePoints is List<Vector3> opList) opList.Capacity = _energyCount;
            if (_generatedTransforms is List<Transform> gtList) gtList.Capacity = _energyCount;

            var points = CirclePointCalculator.GenerateCirclePoints(
                Vector3.zero, _radiateRadius, _energyCount, _radiatePlane);

            var overdrivePoints = CirclePointCalculator.GenerateCirclePoints(
                Vector3.zero, _radiateRadius * 2, _energyCount, _radiatePlane);

            for (var i = 0; i < _energyCount; i++)
            {
                if (token.IsCancellationRequested) return currentTotalCount;

                _burstPoints.Add(points[i]);
                _overdrivePoints.Add(overdrivePoints[i]);

                GameObject newItem = Instantiate(_burstItemPrefab, transform);
                Transform itemTransform = newItem.transform;

                itemTransform.SetLocalPositionAndRotation(points[i], Quaternion.identity);
                _generatedTransforms.Add(itemTransform);

                currentTotalCount++;

                if (currentTotalCount > 100)
                {
                    currentTotalCount = 0;
                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }
            }

            return currentTotalCount;
        }

        public void SetEnergyActive(bool isActive)
        {
            for (var i = 0; i < _generatedTransforms.Count; i++)
            {
                if (_generatedTransforms[i] != null)
                {
                    _generatedTransforms[i].gameObject.SetActive(isActive);
                }
            }
        }

        public void ReleaseEnergy()
        {
            for (var i = 0; i < _generatedTransforms.Count; i++)
            {
                if (_generatedTransforms[i] != null)
                {
                    Destroy(_generatedTransforms[i].gameObject);
                }
            }

            _generatedTransforms.Clear();
            _burstPoints.Clear();
            _overdrivePoints.Clear();
        }
    }
}