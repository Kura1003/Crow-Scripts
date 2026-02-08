using R3;

namespace Taki.Utility
{
    internal interface IPointerEvents
    {
        ReactiveCommand<Unit> OnClicked { get; }
        ReactiveCommand<Unit> OnPointerEntered { get; }
        ReactiveCommand<Unit> OnPointerExited { get; }
    }
}