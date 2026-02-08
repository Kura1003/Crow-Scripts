using Cysharp.Threading.Tasks;
using Taki.Utility;
using VContainer;

namespace Taki.Main.View
{
    public class TextRadiatePointerHandler : PointerEventHandler
    {
        [Inject] private readonly TextBurstRadiator _textBurstRadiator;

        protected override void OnClicked() { }

        protected override void OnPointerEntered()
        {
            _textBurstRadiator
                .Burst(destroyCancellationToken)
                .SuppressCancellationThrow()
                .Forget();
        }

        protected override void OnPointerExited()
        {
            _textBurstRadiator
                .Implode(destroyCancellationToken)
                .SuppressCancellationThrow()
                .Forget();
        }
    }
}