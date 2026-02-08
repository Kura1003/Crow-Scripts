using AnnulusGames.LucidTools.Audio;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using System;
using UnityEngine;
using VContainer;

namespace Taki.Test
{
    public interface IDummyService
    {
        void LogMessage();
    }

    public class DummyService : IDummyService
    {
        public void LogMessage()
        {
        }
    }

    public class TestMonoBehaviour : MonoBehaviour
    {
        [Inject] private readonly IDummyService _dummyService;

        [SerializeField]

        private void Start()
        {
            TestUniTask().Forget();
            TestDOTween();
            TestUniTaskDOTween().Forget();
            TestVContainerInject();
            TestR3();
        }

        private async UniTask TestUniTask()
        {
            await UniTask.WaitForSeconds(0.1f, cancellationToken: destroyCancellationToken);
        }

        private void TestDOTween()
        {
            transform.DOMove(Vector3.one, 0.1f);
            Tween myTween = DOTween.To(() => 0f, x => { }, 1f, 0.1f);
            Sequence mySequence = DOTween.Sequence();
        }

        private async UniTask TestUniTaskDOTween()
        {
            await transform.DOScale(Vector3.one * 1.1f, 0.1f)
                .WithCancellation(destroyCancellationToken);

            await transform.DOScale(Vector3.one, 0.05f);
        }

        private void TestVContainerInject()
        {
            if (_dummyService != null)
            {
                _dummyService.LogMessage();
            }
        }

        private void TestR3()
        {
            var rp = new ReactiveProperty<int>(0);

            IDisposable disposable = rp.Subscribe(x =>
            {
            });

            disposable.Dispose();
        }
    }
}