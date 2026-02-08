using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using Taki.RubiksCube.Data;
using Taki.Utility;
using UnityEngine;
using VContainer;

namespace Taki.RubiksCube.System
{
    public class CubeActionController : MonoBehaviour, ICubeInteractionHandler
    {
        [SerializeField] private float _fastRotationDuration = 0.1f;
        [SerializeField] private int _shuffleCount = 3;
        [SerializeField] private int _fastShuffleCount = 50;

        [Serializable]
        private struct TaggedProvider
        {
            public CubeActionTag ActionTag;
            public BasePointerEventProvider Provider;
        }

        [SerializeField] private List<TaggedProvider> _taggedProviders = new();

        [Inject] private readonly CubeSettings _cubeSettings;

        [Inject] private readonly IRubiksCubeRotator _cubeRotator;
        [Inject] private readonly ICubeFactory _cubeFactory;
        [Inject] private readonly ICubeDataProvider _cubeDataProvider;
        [Inject] private readonly IRotationAxisProvider _rotationAxisProvider;
        [Inject] private readonly ICubeStateRestorer _cubeStateRestorer;
        [Inject] private readonly ICubeCancellationToken _cubeCancellationToken;

        private Dictionary<CubeActionTag, ICubeActionHandler> _actionHandlers;

        private bool _isTaskRunning = false;
        public bool IsTaskRunning => _isTaskRunning;

        private CompositeDisposable _disposables = new();

        private void Awake()
        {
            CreateActionHandlers();
        }

        private void OnDestroy()
        {
            _disposables.Dispose();

            if (_actionHandlers != null)
            {
                _actionHandlers.Values
                    .ToList()
                    .ForEach(handler => handler.Dispose());

                _actionHandlers.Clear();
            }
        }

        private void CreateActionHandlers()
        {
            _actionHandlers = new Dictionary<CubeActionTag, ICubeActionHandler>
            {
                {
                    CubeActionTag.Shuffle,
                    new CubeShuffler(
                        _shuffleCount,
                        _cubeRotator)
                },
                {
                    CubeActionTag.FastShuffle,
                    new FastCubeShuffler(
                        _fastShuffleCount,
                        _cubeRotator,
                        _cubeStateRestorer)
                },
                {
                    CubeActionTag.SlideRotate,
                    new SlideRotator(
                        _cubeSettings,
                        _cubeRotator,
                        _cubeFactory,
                        _cubeCancellationToken)
                },
                {
                    CubeActionTag.Restore,
                    new CubeRestorer(
                        _cubeSettings,
                        _fastRotationDuration,
                        _cubeRotator,
                        _cubeDataProvider,
                        _cubeCancellationToken)
                },
                {
                    CubeActionTag.Rebuild,
                    new CubeRebuilder(
                        _cubeSettings,
                        _cubeFactory,
                        _cubeDataProvider,
                        _rotationAxisProvider,
                        _cubeCancellationToken)
                }
            };
        }

        public void RegisterEvents()
        {
            _taggedProviders
                .Where(tp => _actionHandlers.ContainsKey(tp.ActionTag))
                .ToList()
                .ForEach(tp =>
                {
                    _actionHandlers.TryGetValue(tp.ActionTag, out var handler);
                    tp.Provider.OnClicked
                        .Subscribe(_ =>
                        {
                            ExecuteActionTask(handler)
                            .SuppressCancellationThrow()
                            .Forget();
                        })
                        .AddTo(_disposables);

                    Debug.Log($"タグ: {tp.ActionTag} のアクションを " +
                              $"{tp.Provider.gameObject.name} に登録しました。");
                });
        }

        public void UnregisterEvents()
        {
            _disposables.Dispose();
            Debug.Log("すべてのキューブイベントの登録を解除しました。");
        }

        public UniTask ExecuteRebuild()
        {
            if (_actionHandlers.TryGetValue(CubeActionTag.Rebuild, out var handler))
            {
                return ExecuteActionTask(handler);
            }

            return UniTask.CompletedTask;
        }

        private async UniTask ExecuteActionTask(ICubeActionHandler handler)
        {
            if (_isTaskRunning && handler is not CubeRebuilder)
            {
                return;
            }

            _isTaskRunning = true;
            await handler.Execute();

            if (_cubeCancellationToken.GetToken().IsCancellationRequested) return;

            _isTaskRunning = false;
        }
    }
}