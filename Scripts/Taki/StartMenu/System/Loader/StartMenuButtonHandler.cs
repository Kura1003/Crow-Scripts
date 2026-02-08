using Cysharp.Threading.Tasks;
using Taki.Utility;
using VContainer;

namespace Taki.StartMenu.System
{
    public class StartMenuButtonHandler : PointerEventHandler
    {
        [Inject] private readonly ButtonEntrypoint _buttonEntrypoint;

        [Inject] private readonly IMainSceneLoader _sceneLoader;

        protected override void OnClicked()
        {
            _buttonEntrypoint.DisposeHandlers();

            _sceneLoader
                .LoadMainScene(
                    destroyCancellationToken)
                .SuppressCancellationThrow()
                .Forget();
        }

        protected override void OnPointerEntered() { }

        protected override void OnPointerExited() { }
    }
}
