using Taki.Main.View;
using Taki.RubiksCube.Data;
using Taki.RubiksCube.System;
using Taki.Utility;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using VContainer;
using VContainer.Unity;

namespace Taki.Main.System
{
    public class MainSceneLifetimeScope : LifetimeScope
    {
        [SerializeField] private PostProcessVolume _postProcessVolume;

        [SerializeField] private ImageSequenceFader _imageSequenceFader;
        [SerializeField] private GUIScreenFader _guiScreenFader;
        [SerializeField] private CameraRotator _cameraRotator;
        [SerializeField] private TextLineAnimationManager _lineAnimationManager;
        [SerializeField] private TextBurstRadiator _textBurstRadiator;

        [SerializeField] private Transform _cubePivot;
        [SerializeField] private CubeSettings _cubeSettings;
        [SerializeField] private RubiksCubeFactory _cubeFactory;
        [SerializeField] private CubeActionController _cubeActionController;

        [SerializeField] private UILineAnimationMediator _animationMediator;
        [SerializeField] private ButtonEntrypoint _buttonEntrypoint;
        [SerializeField] private PauseLifecycleManager _pauseLifecycleManager;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_postProcessVolume);
            builder.Register<PostProcessEffectProvider>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.RegisterComponent<ImageSequenceFader>(_imageSequenceFader);
            builder.RegisterComponent<IScreenFader>(_guiScreenFader);
            builder.RegisterComponent<CameraRotator>(_cameraRotator);
            builder.RegisterComponent<TextLineAnimationManager>(_lineAnimationManager);
            builder.RegisterComponent<TextBurstRadiator>(_textBurstRadiator);

            builder.RegisterComponent(_cubeSettings);
            builder.RegisterComponent(_cubeFactory).AsImplementedInterfaces();

            builder.Register<CubeDataProcessor>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.Register<RotationAxisManager>(Lifetime.Singleton)
                .WithParameter(_cubePivot)
                .AsImplementedInterfaces();

            builder.Register<RubiksCubeRotationHandler>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<RubikCubeSizeManager>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.RegisterComponent(_cubeActionController).AsImplementedInterfaces();

            builder.RegisterComponent<UILineAnimationMediator>(_animationMediator);
            builder.RegisterComponent<ButtonEntrypoint>(_buttonEntrypoint);

            builder.Register<PauseService>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.RegisterComponent<PauseLifecycleManager>(_pauseLifecycleManager);

            builder.Register<SceneReloader>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<StartMenuSceneLoader>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.RegisterEntryPoint<GUIScreenFaderInitializer>();
            builder.RegisterEntryPoint<MainSceneEntryPoint>();
        }
    }
}