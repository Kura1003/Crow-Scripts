using Taki.Utility.Core;

namespace Taki.Utility
{
    public class ExitButtonHandler : PointerEventHandler
    {
        protected override void OnClicked()
        {
            App.Exit();
        }

        protected override void OnPointerEntered() { }
        protected override void OnPointerExited() { }
    }
}