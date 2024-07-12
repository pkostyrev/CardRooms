
//#define SPLASH_MANAGER_DEBUG

using CardRooms.DTS;
using CardRooms.Common.Promises;
using System;
using UnityEngine;

namespace CardRooms.UIManager
{
    public class SplashManager : MonoBehaviour
    {
        private enum Mode { Nothing, Cover, World }

        [SerializeField] private CanvasGroup coverGroup;

        private IPromise transitionQueue = Deferred.Resolved();
        private Mode transitionQueueFinalState;

        private void Awake()
        {
            coverGroup.gameObject.SetActive(true);
            coverGroup.alpha = 1f;
            transitionQueueFinalState = Mode.Cover;
        }

        public IPromise ApplyBackground(WindowBackgroundType bgType, Action onHalfWay, float animationSpeed)
        {
            Mode newMode = Mode.Cover;

            switch (bgType)
            {
                case WindowBackgroundType.Empty:
                case WindowBackgroundType.DefaultCover:
                    newMode = Mode.Cover;
                    break;

                case WindowBackgroundType.RaycastBlocker:
                case WindowBackgroundType.Hud:
                case WindowBackgroundType.HudNonInteractable:
                    newMode = Mode.World;
                    break;

                case WindowBackgroundType.NothingIsVisible:
                case WindowBackgroundType.Skip:
                    newMode = Mode.Nothing;
                    break;

                default:
                    Debug.LogError($"WindowBackgroundType {bgType} not supported in CameraController");
                    break;
            }

            return Show(newMode, animationSpeed, onHalfWay);
        }

        private IPromise Show(Mode newMode, float animationSpeed, Action onHalfWay)
        {
            Log($"fade command to {newMode} received");
            bool modeChanged = newMode != transitionQueueFinalState;
            if (modeChanged)
            {
                bool needShowCover = transitionQueueFinalState != Mode.Cover;
                bool needHideCover = newMode != Mode.Cover;

                transitionQueue = transitionQueue
                    .Done(() => { Log($"+ fading camera to {newMode} started"); })
                    .Then(() => needShowCover ? AnimateCover(1.0f, animationSpeed) : Deferred.Resolved())
                    .Done(() =>
                    {
                        onHalfWay?.Invoke();
                        Log($"+ fading camera to {newMode} half way");
                    })
                    .Then(() => needHideCover ? AnimateCover(0.0f, animationSpeed) : Deferred.Resolved())
                    .Done(() => { Log($"+ fading camera to {newMode} completed"); })
                    ;

                transitionQueueFinalState = newMode;
            }
            else
            {
                transitionQueue.Done(() =>
                {
                    onHalfWay?.Invoke();
                });
            }

            return transitionQueue;
        }

        private IPromise AnimateCover(float targetAlphaValue, float animationSpeed)
        {
            float sourceValue = 1f - targetAlphaValue;

            coverGroup.alpha = sourceValue;
            coverGroup.gameObject.SetActive(true);

            IPromise timer = Timers.Instance.WaitUnscaled(1f / animationSpeed, SetAlphaProgress);

            timer.Done(() => SetAlphaProgress(1f));

            return timer;

            void SetAlphaProgress(float progress)
            {
                float alpha = Mathf.Lerp(sourceValue, targetAlphaValue, progress);
                coverGroup.alpha = alpha;
                coverGroup.gameObject.SetActive(alpha > 0.01f);
            }
        }

        [System.Diagnostics.Conditional("SPLASH_MANAGER_DEBUG")]
        private void Log(string message)
        {
            Debug.Log($"<color=yellow>{message}</color>");
        }
    }
}
