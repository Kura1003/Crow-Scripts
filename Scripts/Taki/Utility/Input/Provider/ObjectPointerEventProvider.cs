using UnityEngine;

namespace Taki.Utility
{
    [RequireComponent(typeof(Collider))]
    public class ObjectPointerEventProvider : BasePointerEventProvider
    {
        private void OnMouseDown()
        {
            ExecuteOnClicked();
        }

        private void OnMouseEnter()
        {
            ExecuteOnPointerEntered();
        }

        private void OnMouseExit()
        {
            ExecuteOnPointerExited();
        }
    }
}