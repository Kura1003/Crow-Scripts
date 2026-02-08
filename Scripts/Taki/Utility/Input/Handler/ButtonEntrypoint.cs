using System.Collections.Generic;
using Taki.Utility.Core;
using UnityEngine;

namespace Taki.Utility
{
    public class ButtonEntrypoint : MonoBehaviour
    {
        [SerializeField]
        private List<Transform> _targetParents = new();

        private List<PointerEventHandler> _handlers;

        public void CollectHandlers()
        {
            _handlers = new List<PointerEventHandler>();

            foreach (var parent in _targetParents)
            {
                if (parent is null)
                {
                    continue;
                }

                var components = parent.GetComponentsInChildren<PointerEventHandler>(true);

                foreach (var handler in components)
                {
                    _handlers.Add(handler);
                }
            }
        }

        public void InitializeHandlers()
        {
            Thrower.IfNull(_handlers, nameof(_handlers));

            foreach (var handler in _handlers)
            {
                handler.Initialize();
            }
        }

        public void DisposeHandlers()
        {
            Thrower.IfNull(_handlers, nameof(_handlers));

            foreach (var handler in _handlers)
            {
                handler.Dispose();
            }
        }
    }
}