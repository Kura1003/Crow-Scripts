using UnityEngine;
using UnityEngine.UI;

namespace Taki.Audio
{
    public class AudioMuteLineVisualizer : MonoBehaviour
    {
        [SerializeField] private Image _muteLineImage;

        private void Awake()
        {
            HideLine();
        }

        public void ShowLine()
        {
            SetAlpha(1f);
        }

        public void HideLine()
        {
            SetAlpha(0f);
        }

        private void SetAlpha(float alpha)
        {
            if (_muteLineImage is null) return;

            Color color = _muteLineImage.color;
            color.a = alpha;
            _muteLineImage.color = color;
        }
    }
}