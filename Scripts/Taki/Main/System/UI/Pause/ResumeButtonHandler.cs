using Taki.Main.View;
using Taki.Utility;
using UnityEngine;
using VContainer;

namespace Taki.Main.System
{
    public class ResumeButtonHandler : PointerEventHandler
    {
        [SerializeField] private CircleTextRotator _circleTextRotator;

        [Inject] private readonly IPauseEvents _pauseEvents;

        protected override void OnClicked()
        {
            if (!_pauseEvents.IsFullyPaused ||
                _circleTextRotator.IsTaskExecuting) return;

            _pauseEvents.Resume();
        }

        protected override void OnPointerEntered() { }

        protected override void OnPointerExited() { }
    }
}