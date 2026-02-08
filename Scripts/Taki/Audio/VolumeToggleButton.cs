using Taki.Utility;
using UnityEngine;

namespace Taki.Audio
{
    public class VolumeToggleButton : PointerEventHandler
    {
        [SerializeField] private AudioMuteLineVisualizer _muteLineVisualizer;

        protected override void OnClicked()
        {
            var audioManager = AudioManager.Instance;
            audioManager.ToggleMasterVolume();

            if (audioManager.IsVolumeMuted)
            {
                OnMuted();
            }

            else
            {
                OnUnmuted();
            }
        }

        protected override void OnPointerEntered() { }
        protected override void OnPointerExited() { }

        private void OnMuted()
        {
            _muteLineVisualizer.ShowLine();
        }

        private void OnUnmuted()
        {
            _muteLineVisualizer.HideLine();
        }
    }
}