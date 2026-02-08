using Taki.Utility;
using VContainer;

namespace Taki.Main.System
{
    public class PauseButtonHandler : PointerEventHandler
    {
        [Inject] private readonly IPauseEvents _pauseEvents;

        protected override void OnClicked()
        {
            _pauseEvents.Pause();
        }

        protected override void OnPointerEntered() { }
        protected override void OnPointerExited() { }
    }
}