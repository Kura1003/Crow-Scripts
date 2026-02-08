using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Taki.Utility;

namespace Taki.Main.System
{
    public class UILineGenerator : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _prefabObjects = new();
        [SerializeField] private Color _hoverColor = Color.white;
        [SerializeField] private int _lineCount = 8;
        [SerializeField] private float _spacing = 10f;
        [SerializeField] private GridPlane _plane = GridPlane.XZ;

        public List<GraphicColorEntry> GenerateUIElements()
        {
            Vector3[] points = LinePointCalculator.GenerateLinePoints(
                Vector3.zero,
                _lineCount,
                _spacing,
                _plane);

            List<GraphicColorEntry> newEntries = new();

            for (int i = 0; i < points.Length; i++)
            {
                GameObject prefab = _prefabObjects[i % _prefabObjects.Count];
                GameObject newObject = Instantiate(prefab, transform);

                RectTransform rectTransform = newObject.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = points[i];

                Graphic graphic = newObject.GetComponent<Graphic>();
                newEntries.Add(new GraphicColorEntry
                {
                    Graphic = graphic,
                    HoverColor = _hoverColor
                });
            }

            return newEntries;
        }
    }
}