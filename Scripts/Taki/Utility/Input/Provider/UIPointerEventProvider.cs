using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Taki.Utility
{
    [RequireComponent(typeof(Image))]
    public class UIPointerEventProvider : BasePointerEventProvider,
        IPointerClickHandler,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                ExecuteOnClicked();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            ExecuteOnPointerEntered();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ExecuteOnPointerExited();
        }
    }
}