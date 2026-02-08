using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Taki.Main.View
{
    public class SelectedNodeAnimator : MonoBehaviour
    {
        [SerializeField] private CircleTextRotator _circleTextRotator;

        [Header("Position Sync Settings")]
        [SerializeField] private List<RectTransform> _attachmentsToSyncPosition;

        [Header("Rotation Settings")]
        [SerializeField] private List<RectTransform> _attachmentsToRotateClockwise;
        [SerializeField] private List<RectTransform> _attachmentsToRotateCounterClockwise;
        [SerializeField] private float _rotateAngleZ = 360f;
        [SerializeField] private float _rotateDuration = 0.5f;
        [SerializeField] private Ease _rotateEase = Ease.OutQuart;

        [Header("Flash Settings")]
        [SerializeField] private List<Graphic> _graphicsToFlash;
        [SerializeField] private Color _originalColor = Color.white;
        [SerializeField] private Color _flashColor = Color.yellow;
        [SerializeField] private float _flashDuration = 0.3f;
        [SerializeField] private Ease _flashEase = Ease.Linear;

        [SerializeField] private bool _ignoreTimeScale = false;

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            _circleTextRotator.Initialize();
            SyncToSelectedNodePosition();

            _circleTextRotator.OnRotationComplete
                .Subscribe(_ =>
                {
                    FlashSelectedElements(destroyCancellationToken)
                        .SuppressCancellationThrow()
                        .Forget();
                })
                .AddTo(this);
        }

        private void SyncToSelectedNodePosition()
        {
            var selectedNode = _circleTextRotator.GetSelectedNode();
            var targetPosition = selectedNode.RectTransform.position;

            foreach (var attachment in _attachmentsToSyncPosition)
            {
                if (attachment != null)
                {
                    attachment.position = targetPosition;
                }
            }
        }

        private UniTask FlashSelectedElements(CancellationToken token)
        {
            var selectedNode = _circleTextRotator.GetSelectedNode();
            var textComponent = selectedNode.Text;

            if (textComponent is null) return UniTask.CompletedTask;

            var sequence = DOTween.Sequence();

            textComponent.color = _flashColor;
            sequence.Join(textComponent.DOColor(_originalColor, _flashDuration).SetEase(_flashEase));

            foreach (var graphic in _graphicsToFlash)
            {
                if (graphic is null) continue;

                graphic.color = _flashColor;
                sequence.Join(graphic.DOColor(_originalColor, _flashDuration).SetEase(_flashEase));
            }

            foreach (var attachment in _attachmentsToRotateClockwise)
            {
                if (attachment is null) continue;

                var targetRotation = attachment.localEulerAngles + new Vector3(0, 0, _rotateAngleZ);
                sequence.Join(attachment.DOLocalRotate(targetRotation, _rotateDuration, RotateMode.FastBeyond360)
                    .SetEase(_rotateEase));
            }

            foreach (var attachment in _attachmentsToRotateCounterClockwise)
            {
                if (attachment is null) continue;

                var targetRotation = attachment.localEulerAngles + new Vector3(0, 0, -_rotateAngleZ);
                sequence.Join(attachment.DOLocalRotate(targetRotation, _rotateDuration, RotateMode.FastBeyond360)
                    .SetEase(_rotateEase));
            }

            return sequence
                .SetUpdate(_ignoreTimeScale)
                .WithCancellation(token);
        }
    }
}