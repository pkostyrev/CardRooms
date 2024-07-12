
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CardRooms.Windows.Helpers
{
    public class ButtonFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform animatedTransform;
        [SerializeField] private DTS.CardRoomsResources cardRoomsResources;

        private Vector2? defaultOffset;
        private DTS.CardRoomsResources.ButtonFeedbackSettings buttonFeedbackSettings;
        private Coroutine playingAnimation;

        private void Awake()
        {
            buttonFeedbackSettings = cardRoomsResources.buttonFeedbackSettings;

            if (animatedTransform == null)
                animatedTransform = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            animatedTransform.localScale = Vector3.one;

            if (defaultOffset.HasValue == true)
            {
                animatedTransform.anchoredPosition = defaultOffset.Value;
                defaultOffset = null;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (playingAnimation != null)
            {
                StopCoroutine(playingAnimation);
            }

            if (gameObject.activeInHierarchy == true)
            {
                playingAnimation = StartCoroutine(Animate(buttonFeedbackSettings.pointerDown, buttonFeedbackSettings.pointerDownDuration, false));
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (playingAnimation != null)
            {
                StopCoroutine(playingAnimation);
            }

            if (gameObject.activeInHierarchy == true)
            {
                playingAnimation = StartCoroutine(Animate(buttonFeedbackSettings.pointerUp, buttonFeedbackSettings.pointerUpDuration, true));
            }
        }

        private IEnumerator Animate(AnimationCurve animation, float duration, bool isBackToDefault)
        {
            float startTime = Time.unscaledTime;
            float normalizedProgress = 0f;

            float offsetMultiplerSizeDependent = buttonFeedbackSettings.offsetMultiplier * animatedTransform.rect.height;

            Vector2 toPivotDefault = animatedTransform.rect.size * (animatedTransform.pivot - Vector2.one * 0.5f);

            if (defaultOffset.HasValue == false)
            {
                defaultOffset = animatedTransform.anchoredPosition;
            }

            while (normalizedProgress < 1f)
            {
                normalizedProgress = (Time.unscaledTime - startTime) / duration;

                float effect = animation.Evaluate(Mathf.Clamp01(normalizedProgress));

                float scale = 1f - effect * buttonFeedbackSettings.scaleMultiplier;

                animatedTransform.localScale = Vector3.one * scale;

                animatedTransform.anchoredPosition = defaultOffset.Value + Vector2.down * effect * offsetMultiplerSizeDependent + (toPivotDefault * (scale - 1f));

                yield return null;
            }

            if (isBackToDefault == true)
            {
                animatedTransform.anchoredPosition = defaultOffset.Value;
                animatedTransform.localScale = Vector3.one;
                defaultOffset = null;
            }
        }
    }
}
