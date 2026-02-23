using System;
using System.Collections.Generic;
using Taki.Utility;
using UnityEngine;

namespace Taki.Main.System
{
    [Serializable]
    public struct UILineAnimationSet
    {
        public UILineGenerator Generator;
        public PointerColorAnimator Animator;
    }

    public class UILineAnimationMediator : MonoBehaviour
    {
        [SerializeField] private List<UILineAnimationSet> _animationSets = new();

        private readonly Dictionary<PointerColorAnimator, List<GraphicColorEntry>> 
            _cachedEntriesMap = new();

        public void ExecuteGeneration()
        {
            _cachedEntriesMap.Clear();

            foreach (var set in _animationSets)
            {
                var entries = set.Generator.GenerateUIElements();
                _cachedEntriesMap[set.Animator] = entries;
            }
        }

        public void ExecuteSetup()
        {
            foreach (var set in _animationSets)
            {
                if (_cachedEntriesMap.TryGetValue(set.Animator, out var entries))
                {
                    set.Animator.SetGraphicEntries(entries, true);
                }
            }
        }
    }
}