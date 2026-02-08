using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Taki.Audio;
using Taki.Main.System;
using Taki.Utility;
using TMPro;
using UnityEngine;
using VContainer;

namespace Taki.Main.View
{
    [Serializable]
    public struct OrbiterNode
    {
        public RectTransform RectTransform;
        public TextMeshProUGUI Text;
        public int CubeSize;
    }

    public class CircleTextRotator : MonoBehaviour
    {
        [SerializeField] private List<OrbiterNode> _rotatorObjects;
        [SerializeField] private float _radius = 5.0f;
        [SerializeField] private RectTransform _rotationAxis;
        [SerializeField] private float _snapDuration = 0.5f;
        [SerializeField] private Ease _snapEase = Ease.OutQuad;
        [SerializeField] private bool _ignoreTimeScale = false;

        [Inject] private readonly IPauseEvents _pauseEvents;

        public OrbiterNode GetSelectedNode() => _rotatorObjects[1];

        private readonly Subject<Unit> _onRotationComplete = new Subject<Unit>();
        public Observable<Unit> OnRotationComplete => _onRotationComplete;

        private bool _isTaskExecuting = false;

        public bool IsTaskExecuting => _isTaskExecuting;

        private Dictionary<KeyCode, Func<CancellationToken, UniTask>> _keyActionMap = new();

        private void Update()
        {
            if (_isTaskExecuting || !_pauseEvents.IsFullyPaused) return;

            foreach (var pair in _keyActionMap)
            {
                if (Input.GetKeyDown(pair.Key))
                {
                    pair.Value.Invoke(destroyCancellationToken)
                        .SuppressCancellationThrow()
                        .Forget();
                }
            }
        }
        public void OnDestroy() => _onRotationComplete.Dispose();

        public void Initialize()
        {
            InitializeKeyActionMap();

            var circlePoints = CirclePointCalculator.GenerateCirclePoints(
                Vector3.zero,
                _radius,
                _rotatorObjects.Count,
                GridPlane.XY,
                45.0f);

            for (int i = 0; i < _rotatorObjects.Count; i++)
            {
                var node = _rotatorObjects[i];
                node.RectTransform.anchoredPosition = circlePoints[i];
                node.RectTransform.SetParent(_rotationAxis);

                if (node.Text != null) node.Text.text = node.CubeSize.ToString();
            }

            RotateInstantly(1);
        }

        private void InitializeKeyActionMap()
        {
            _keyActionMap = new Dictionary<KeyCode, Func<CancellationToken, UniTask>>()
            {
                { KeyCode.A, RotateCounterClockwise },
                { KeyCode.D, RotateClockwise }
            };
        }

        private UniTask RotateClockwise(CancellationToken token) => Rotate(-1, token);
        private UniTask RotateCounterClockwise(CancellationToken token) => Rotate(1, token);

        private UniTask Rotate(int direction, CancellationToken token)
        {
            _isTaskExecuting = true;

            float angleStep = 360f / _rotatorObjects.Count;
            float targetZ = _rotationAxis.localEulerAngles.z + angleStep * direction;

            var sequence = DOTween.Sequence();
            sequence.Append(_rotationAxis.DOLocalRotate(new Vector3(0, 0, targetZ), _snapDuration));

            _rotatorObjects.ForEach(node =>
            {
                float childTargetZ = node.RectTransform.localEulerAngles.z - angleStep * direction;
                sequence.Insert(0, node.RectTransform.DOLocalRotate(
                    new Vector3(node.RectTransform.localEulerAngles.x, node.RectTransform.localEulerAngles.y, childTargetZ),
                    _snapDuration));
            });

            sequence.OnComplete(() => OnRotationCompleted(direction));

            return sequence.SetEase(_snapEase).SetUpdate(_ignoreTimeScale).WithCancellation(token);
        }

        private void RotateInstantly(int direction)
        {
            float angleStep = 360f / _rotatorObjects.Count;
            float targetZ = _rotationAxis.localEulerAngles.z + angleStep * direction;
            _rotationAxis.localEulerAngles = new Vector3(0, 0, targetZ);

            _rotatorObjects.ForEach(node =>
            {
                float childTargetZ = node.RectTransform.localEulerAngles.z - angleStep * direction;
                node.RectTransform.localEulerAngles = new Vector3(
                    node.RectTransform.localEulerAngles.x,
                    node.RectTransform.localEulerAngles.y,
                    childTargetZ);
            });

            ShiftList(direction);
        }

        private void OnRotationCompleted(int direction)
        {
            AudioManager.Instance.Play("LockEngage", gameObject).SetVolume(0.5f);

            ShiftList(direction);
            _onRotationComplete.OnNext(Unit.Default);
            _isTaskExecuting = false;
        }

        private void ShiftList(int direction)
        {
            if (direction > 0)
            {
                var last = _rotatorObjects.Last();
                _rotatorObjects.RemoveAt(_rotatorObjects.Count - 1);
                _rotatorObjects.Insert(0, last);
            }

            else
            {
                var first = _rotatorObjects[0];
                _rotatorObjects.RemoveAt(0);
                _rotatorObjects.Add(first);
            }
        }
    }
}