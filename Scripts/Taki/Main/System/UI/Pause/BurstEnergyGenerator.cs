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

        public async UniTask<int> ChargeAndGenerate(int currentTotalCount, CancellationToken cancellationToken)
        {
            ReleaseEnergy();

            if (_burstItemPrefab == null)
                return currentTotalCount;

            _generatedTransforms.Capacity =
            _burstPoints.Capacity =
            _overdrivePoints.Capacity = _energyCount;

            var burstPoints = CirclePointCalculator.GenerateCirclePoints(
                Vector3.zero,
                _radiateRadius,
                _energyCount,
                _radiatePlane);

            var overdrivePoints = CirclePointCalculator.GenerateCirclePoints(
                Vector3.zero,
                _radiateRadius * 2f,
                _energyCount,
                _radiatePlane);

            for (int index = 0; index < _energyCount; index++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                _burstPoints.Add(burstPoints[index]);
                _overdrivePoints.Add(overdrivePoints[index]);

                Transform generatedTransform =
                    Instantiate(_burstItemPrefab, transform).transform;

                generatedTransform.SetLocalPositionAndRotation(
                    burstPoints[index],
                    Quaternion.identity);

                _generatedTransforms.Add(generatedTransform);

                if (++currentTotalCount > 100)
                {
                    currentTotalCount = 0;
                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                }
            }

            return currentTotalCount;
        }

        public void SetEnergyActive(bool isActive)
        {
            foreach (Transform generatedTransform in _generatedTransforms)
            {
                if (generatedTransform != null)
                {
                    generatedTransform.gameObject.SetActive(isActive);
                }
            }
        }

        public void ReleaseEnergy()
        {
            foreach (Transform generatedTransform in _generatedTransforms)
            {
                if (generatedTransform != null)
                {
                    Destroy(generatedTransform.gameObject);
                }
            }

            _generatedTransforms.Clear();
            _burstPoints.Clear();
            _overdrivePoints.Clear();
        }
    }
}