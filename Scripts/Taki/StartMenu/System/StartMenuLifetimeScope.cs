using UnityEngine;
using VContainer;
using VContainer.Unity;
using Taki.Utility;

namespace Taki.StartMenu.System
{
    public class StartMenuLifetimeScope : LifetimeScope
    {
        [SerializeField] private CursorEffectSpawner _cursorEffectSpawner;
        [SerializeField] private GUIScreenFader _guiScreenFader;
        [SerializeField] private TapToStartHandler _tapToStartHandler;
        [SerializeField] private ImageSequenceFader _imageSequenceFader;
        [SerializeField] private CircularImageSpawner _circularImageSpawner;
        [SerializeField] private ButtonEntrypoint _buttonEntrypoint;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_cursorEffectSpawner);
            builder.RegisterComponent<IScreenFader>(_guiScreenFader);
            builder.RegisterComponent(_tapToStartHandler);
            builder.RegisterComponent(_imageSequenceFader);
            builder.RegisterComponent(_circularImageSpawner);
            builder.RegisterComponent(_buttonEntrypoint);

            builder.Register<MainSceneLoader>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.RegisterEntryPoint<GUIScreenFaderInitializer>();
            builder.RegisterEntryPoint<StartMenuEntryPoint>();
        }
    }
}