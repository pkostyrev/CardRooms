
using CardRooms.Windows.Helpers.ElementAnimation;
using CardRooms.Common.Promises;
using UnityEngine;

namespace CardRooms.Windows.Elements
{
    public abstract class ElementBase : MonoBehaviour
    {
        private ElementAnimator elementAnimator;
        private ElementAnimator ElementAnimator
        {
            get
            {
                if (elementAnimator == null)
                {
                    elementAnimator = GetComponent<ElementAnimator>();
                }
                return elementAnimator;
            }
        }

        public bool GetVisibleState()
        {
            if (ElementAnimator != null)
            {
                return ElementAnimator.IsShown && gameObject.activeInHierarchy;
            }
            else
            {
                return false;
            }
        }

        public virtual IPromise SetVisible(bool visible)
        {
            if (ElementAnimator != null)
            {
                return ElementAnimator.SetVisible(visible);
            }
            else
            {
                gameObject.SetActive(visible);
                return Deferred.Resolved();
            }
        }
    }
}
