
using CardRooms.Common.Promises;
using UnityEngine;

namespace CardRooms.Windows.Elements
{
    [RequireComponent(typeof(ElementBase))]
    public class WindowsOptionalElement : MonoBehaviour
    {
        private ElementBase elementBase;

        protected bool windowsVisibility;

        internal bool IsVisible => windowsVisibility;

        private void Awake()
        {
            elementBase = GetComponent<ElementBase>();
        }

        public IPromise SetWindowsVisibility(bool visibility)
        {
            windowsVisibility = visibility;
            return elementBase.SetVisible(visibility);
        }
    }
}
