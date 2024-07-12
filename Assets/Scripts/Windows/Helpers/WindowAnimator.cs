
using CardRooms.Common.Promises;
using System;
using UnityEngine;

namespace CardRooms.Windows.Helpers
{
    [RequireComponent(typeof(RectTransform))]
    public class WindowAnimator : MonoBehaviour
    {
        [Serializable]
        private struct State
        {
            public float alpha;
            public float scale;
        }

        [SerializeField] private bool animateScale = true;
        [SerializeField] private bool animateAlpha = true;

        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private float animationSpeed;

        [SerializeField] private State visibleState = new State { alpha = 1f, scale = 1f };
        [SerializeField] private State hiddenState = new State { alpha = 0f, scale = 0.8f };

        private float currentState = 0f;
        private float goalState = 0f;

        private Deferred onCompleteDeferred;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            animationSpeed = Root.Root.ConfigManager.GameSettings.userInterface.animationSpeed;

            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            ApplyState();
        }

        public IPromise Show()
        {
            ResolveAndStop(true);

            enabled = true;
            onCompleteDeferred = Deferred.GetFromPool();
            goalState = 1f;

            return onCompleteDeferred;
        }

        public IPromise Hide()
        {
            ResolveAndStop(true);

            enabled = true;
            onCompleteDeferred = Deferred.GetFromPool();
            goalState = 0f;

            return onCompleteDeferred;
        }

        private void Update()
        {
            if (Mathf.Approximately(currentState, goalState) == true)
            {
                ResolveAndStop(false);
            }
            else
            {
                currentState = Mathf.MoveTowards(currentState, goalState, Time.unscaledDeltaTime * animationSpeed);
                ApplyState();
            }
        }

        private void ResolveAndStop(bool interrupted)
        {
            enabled = false;

            Deferred deferred = onCompleteDeferred;
            onCompleteDeferred = null;

            if (deferred != null)
            {
                if (interrupted)
                {
                    deferred.Reject(new System.Exception("Animation interrupted by another command"));
                }
                else
                {
                    deferred.Resolve();
                }
            }
        }

        private void ApplyState()
        {
            State state = GetCurrentState();

            if (animateAlpha == true)
            {
                canvasGroup.alpha = state.alpha;
            }

            if (animateScale == true)
            {
                rectTransform.localScale = Vector3.one * state.scale;
            }
        }

        private State GetCurrentState()
        {
            return new State
            {
                alpha = Mathf.Lerp(hiddenState.alpha, visibleState.alpha, currentState),
                scale = Mathf.Lerp(hiddenState.scale, visibleState.scale, currentState),
            };
        }
    }
}
