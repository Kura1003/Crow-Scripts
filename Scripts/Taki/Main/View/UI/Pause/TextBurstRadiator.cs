using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading;
using Taki.Main.System;
using UnityEngine;

namespace Taki.Main.View
{
    public class TextBurstRadiator : MonoBehaviour
    {
        [SerializeField] private List<BurstEnergyGenerator> _energyGenerators;
        [SerializeField] private float _burstDuration = 0.5f;
        [SerializeField] private Ease _burstEaseType = Ease.OutQuad;
        [SerializeField] private Ease _implodeEaseType = Ease.InQuad;
        [SerializeField] private bool _ignoreTimeScale = false;

        private readonly List<Transform> _radiatedItems = new();
        private readonly List<Vector3> _corePositions = new();
        private readonly List<Vector3> _overdrivePositions = new();
        private readonly List<Tween> _activeBurstTweens = new();

        private bool _isOverdriveAnimating = false;

        public async UniTask Initialize(CancellationToken token)
        {
            _radiatedItems.Clear();
            _corePositions.Clear();
            _overdrivePositions.Clear();

            int cumulativeCreationCount = 0;

            foreach (var generator in _energyGenerators)
            {
                cumulativeCreationCount = await generator
                    .ChargeAndGenerate(cumulativeCreationCount, token);

                var items = generator.GeneratedTransforms;
                for (var i = 0; i < items.Count; i++)
                {
                    _radiatedItems.Add(items[i]);
                }

                var points = generator.BurstPoints;
                for (var i = 0; i < points.Count; i++)
                {
                    _corePositions.Add(points[i]);
                }

                var overdrivePoints = generator.OverdrivePoints;
                for (var i = 0; i < overdrivePoints.Count; i++)
                {
                    _overdrivePositions.Add(overdrivePoints[i]);
                }
            }

            for (var i = 0; i < _radiatedItems.Count; i++)
            {
                _radiatedItems[i].localPosition = Vector3.zero;
            }
        }

        public void Dispose()
        {
            StopAllBursts();

            foreach (var generator in _energyGenerators)
            {
                if (generator != null)
                {
                    generator.ReleaseEnergy();
                }
            }

            _radiatedItems.Clear();
            _corePositions.Clear();
            _overdrivePositions.Clear();
        }

        public async UniTask Burst(CancellationToken token)
        {
            if (_isOverdriveAnimating) return;

            StopAllBursts();
            SetGeneratorsActive(true);

            var tasks = new List<UniTask>(_radiatedItems.Count);

            for (var i = 0; i < _radiatedItems.Count; i++)
            {
                var targetPosition = _corePositions[i];
                var tween = _radiatedItems[i].DOLocalMove(targetPosition, _burstDuration)
                    .SetEase(_burstEaseType)
                    .SetUpdate(_ignoreTimeScale);

                _activeBurstTweens.Add(tween);
                tasks.Add(tween.ToUniTask(cancellationToken: token));
            }

            await UniTask.WhenAll(tasks);
            _activeBurstTweens.Clear();
        }

        public async UniTask Implode(CancellationToken token)
        {
            if (_isOverdriveAnimating) return;

            StopAllBursts();

            var tasks = new List<UniTask>(_radiatedItems.Count);

            foreach (var item in _radiatedItems)
            {
                var tween = item.DOLocalMove(Vector3.zero, _burstDuration)
                    .SetEase(_implodeEaseType)
                    .SetUpdate(_ignoreTimeScale);

                _activeBurstTweens.Add(tween);
                tasks.Add(tween.ToUniTask(cancellationToken: token));
            }

            await UniTask.WhenAll(tasks);

            _activeBurstTweens.Clear();
            SetGeneratorsActive(false);
        }

        public async UniTask Overdrive(CancellationToken token)
        {
            if (_isOverdriveAnimating) return;

            StopAllBursts();
            _isOverdriveAnimating = true;

            var tasks = new List<UniTask>(_radiatedItems.Count);

            for (var i = 0; i < _radiatedItems.Count; i++)
            {
                var targetPosition = _overdrivePositions[i];
                var tween = _radiatedItems[i].DOLocalMove(targetPosition, _burstDuration * 2)
                    .SetEase(_burstEaseType)
                    .SetUpdate(_ignoreTimeScale);

                _activeBurstTweens.Add(tween);
                tasks.Add(tween.ToUniTask(cancellationToken: token));
            }

            await UniTask.WhenAll(tasks);

            foreach (var item in _radiatedItems)
            {
                item.localPosition = Vector3.zero;
            }

            SetGeneratorsActive(false);
            _isOverdriveAnimating = false;
        }

        private void SetGeneratorsActive(bool isActive)
        {
            foreach (var generator in _energyGenerators)
            {
                generator.SetEnergyActive(isActive);
            }
        }

        private void StopAllBursts()
        {
            for (var i = _activeBurstTweens.Count - 1; i >= 0; i--)
            {
                var tween = _activeBurstTweens[i];
                if (tween != null && tween.IsActive())
                {
                    tween.Kill();
                }
            }

            _activeBurstTweens.Clear();
        }
    }
}