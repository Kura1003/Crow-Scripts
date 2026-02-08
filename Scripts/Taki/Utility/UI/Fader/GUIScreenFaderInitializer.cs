using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Taki.Utility
{
    public class GUIScreenFaderInitializer : IInitializable
    {
        private static readonly Texture2D FADE_TEXTURE = Texture2D.whiteTexture;
        private static readonly Rect FADE_POSITION = new Rect(0, 0, Screen.width, Screen.height);
        private static readonly Color FADE_COLOR = Color.black;
        private const float FADE_DURATION = 0.5f;
        private const float FADE_IN_TARGET_ALPHA = 1f;

        [Inject] private readonly IScreenFader _screenFader;

        public void Initialize()
        {
            _screenFader.Initialize(FADE_TEXTURE, FADE_POSITION);
            _screenFader.SetTextureColor(FADE_COLOR);
            _screenFader.SetFadeDuration(FADE_DURATION);
            _screenFader.SetFadeInTargetAlpha(FADE_IN_TARGET_ALPHA);
        }
    }
}