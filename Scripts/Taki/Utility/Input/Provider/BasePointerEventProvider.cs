using R3;
using UnityEngine;

namespace Taki.Utility
{
    public abstract class BasePointerEventProvider : MonoBehaviour, IPointerEvents
    {
        private readonly ReactiveCommand<Unit> _onClicked = new();
        private readonly ReactiveCommand<Unit> _onPointerEntered = new();
        private readonly ReactiveCommand<Unit> _onPointerExited = new();

        public ReactiveCommand<Unit> OnClicked => _onClicked;
        public ReactiveCommand<Unit> OnPointerEntered => _onPointerEntered;
        public ReactiveCommand<Unit> OnPointerExited => _onPointerExited;

        private void OnDestroy()
        {
            _onClicked.Dispose();
            _onPointerEntered.Dispose();
            _onPointerExited.Dispose();
        }

        protected void ExecuteOnClicked() => _onClicked.Execute(Unit.Default);
        protected void ExecuteOnPointerEntered() => _onPointerEntered.Execute(Unit.Default);
        protected void ExecuteOnPointerExited() => _onPointerExited.Execute(Unit.Default);
    }
}