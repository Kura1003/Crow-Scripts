using Taki.Utility;
using VContainer;
using XPostProcessing;

namespace Taki.Main.System
{
    public class GlitchEffectToggleButton : PointerEventHandler
    {
        [Inject] private readonly IPostProcessEffectProvider _postProcessProvider;

        private GlitchImageBlockV2 _glitchImageBlockV2;
        private bool _isEffectActive = true;

        private void Start()
        {
            _glitchImageBlockV2 = _postProcessProvider.GetEffect<GlitchImageBlockV2>();
        }

        protected override void OnClicked()
        {
            _isEffectActive = !_isEffectActive;

            if (_isEffectActive)
            {
                OnEffectEnabled();
            }

            else
            {
                OnEffectDisabled();
            }
        }

        protected override void OnPointerEntered() { }
        protected override void OnPointerExited() { }

        private void OnEffectEnabled()
        {
            _glitchImageBlockV2.active = true;
        }

        private void OnEffectDisabled()
        {
            _glitchImageBlockV2.active = false;
        }
    }
}