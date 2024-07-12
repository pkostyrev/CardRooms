
using CardRooms.Common.Promises;
using UnityEngine;

namespace CardRooms.Windows.Helpers.ElementAnimation
{
    [RequireComponent(typeof(ElementAnimator))]
    public abstract class AnimationBase : MonoBehaviour
    {
        private bool lastVisible;
        private IPromise lastAnimation;
        private float animationDuration;

        public void Init(bool visible, float animationSpeed)
        {
            animationDuration = 1f / animationSpeed;

            lastVisible = visible;
            lastAnimation = Deferred.Resolved();
            Init();
            ApplyVisibility(visible ? 1f : 0f);
        }

        public IPromise SetVisible(bool visible)
        {
            if (lastVisible != visible)
            {
                lastVisible = visible;
                lastAnimation = lastAnimation.Then(() => { return StartAnimation(visible); });
            }

            return lastAnimation;
        }

        private IPromise StartAnimation(bool newState)
        {
            return Timers.Instance.WaitUnscaled(animationDuration,
                progress =>
                {
                    if (this != null)
                    {
                        if (newState == true)
                        {
                            ApplyVisibility(progress);
                        }
                        else
                        {
                            ApplyVisibility(1f - progress);
                        }
                    }
                });
        }

        protected abstract void Init();

        internal abstract bool CanBeDisabledWhenInvisible { get; }

        protected abstract void ApplyVisibility(float visibility);
    }
}
