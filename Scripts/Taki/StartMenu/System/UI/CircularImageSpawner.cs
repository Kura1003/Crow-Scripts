using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Taki.Audio;

namespace Taki.StartMenu.System
{
    public class CircularImageSpawner : MonoBehaviour
    {
        [SerializeField] private RectTransform _centerTransform;
        [SerializeField] private List<GameObject> _prefabs = new();
        [SerializeField] private float _radius = 200f;
        [SerializeField] private float _interval = 0.3f;

        private readonly List<GameObject> _spawnedObjects = new();

        public void SpawnAtThirdQuadrant()
        {
            if (_prefabs.Count == 0) return;

            float startAngle = 180f;
            float endAngle = 270f;
            float totalRange = endAngle - startAngle;

            float angleStep = _prefabs.Count > 1 ? totalRange / (_prefabs.Count - 1) : 0;

            for (int i = 0; i < _prefabs.Count; i++)
            {
                float angle = startAngle + (angleStep * i);
                float radian = angle * Mathf.Deg2Rad;

                float x = _radius * Mathf.Cos(radian);
                float y = _radius * Mathf.Sin(radian);

                GameObject obj = Instantiate(_prefabs[i], _centerTransform);
                RectTransform rect = obj.GetComponent<RectTransform>();

                if (rect != null)
                {
                    rect.anchoredPosition = new Vector2(x, y);
                }

                obj.SetActive(false);
                _spawnedObjects.Add(obj);
            }
        }

        public async UniTask PlayActivationSequenceAsync(CancellationToken token)
        {
            foreach (var obj in _spawnedObjects)
            {
                if (obj == null) continue;

                obj.SetActive(true);
                AudioManager.Instance.Play("TextType", gameObject).SetVolume(0.5f);

                await UniTask.WaitForSeconds(_interval, cancellationToken: token);
            }
        }

        private void OnDestroy()
        {
            foreach (var obj in _spawnedObjects)
            {
                if (obj != null) Destroy(obj);
            }

            _spawnedObjects.Clear();
        }
    }
}