using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Taki.Utility
{
    public class RandomColorApplier : MonoBehaviour
    {
        [SerializeField] private List<Renderer> _renderersToApply = new();
        [SerializeField] private List<Graphic> _graphicsToApply = new();

        [Header("Ignore Settings")]
        [SerializeField] private bool _useIgnoreSettings;
        [SerializeField] private Color _ignoreBaseColor = Color.white;
        [SerializeField] private float _threshold = 0.5f;

        private void Awake()
        {
            ApplyColor();
        }

        public void ApplyColor()
        {
            ColorIgnoreSettings? settings = _useIgnoreSettings
                ? new ColorIgnoreSettings(_ignoreBaseColor, _threshold)
                : null;

            Color colorToApply = RandomUtility.GetColor(1.0f, settings);

            foreach (var rend in _renderersToApply.Where(rend => rend != null))
            {
                rend.material.color = colorToApply;
            }

            foreach (var graphic in _graphicsToApply.Where(graphic => graphic != null))
            {
                graphic.color = colorToApply;
            }
        }
    }
}