
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CardRooms.Common
{
    public class AnimatedBar : MonoBehaviour
    {
        public enum TextFormat : byte
        {
            Value = 0,
            ValueWithMax = 1,
            Percentage = 2,
            None = 100,
        }

        private struct Transition
        {
            public double finishTime;
            public float barFrom;
            public float barTo;
            public long valueFrom;
            public long valueTo;
            public long valueMax;
        }

        [SerializeField] private float animationDuration = 1f;
        [SerializeField] private RectTransform barGraphic;
        [SerializeField] private float barGraphicMinAnchor;
        [SerializeField] private TMP_Text barText;
        [SerializeField] private TextFormat format;

        [SerializeField] private Image fillProgessBar;
        [SerializeField] private Sprite spriteProgressBarFullyFilled;
        [SerializeField] private Sprite spriteProgressBarNotFullyFilled;

        private bool inited = false;

        private Transition transition;

        private void Update()
        {
            Refresh();
        }

        public void SetImmediately(long current, long max)
        {
            inited = false;
            Set(current, max);
        }

        public void Set(long current, long max)
        {
            float valueNormalized = max == 0 ? 0f : ((float)current / (float)max);

            enabled = true;

            fillProgessBar.sprite = current >= max ? spriteProgressBarFullyFilled : spriteProgressBarNotFullyFilled;

            if (inited == false)
            {
                transition = new Transition()
                {
                    barFrom = valueNormalized,
                    barTo = valueNormalized,
                    valueTo = current,
                    valueMax = max,
                    finishTime = 0
                };

                Refresh();
                inited = true;
                return;
            }
            else
            {
                float progress = GetTransitionProgress();

                transition = new Transition()
                {
                    barFrom = Mathf.Lerp(transition.barFrom, transition.barTo, progress),
                    valueFrom = (long)Mathf.Lerp(transition.valueFrom, transition.valueTo, progress),
                    barTo = valueNormalized,
                    valueTo = current,
                    valueMax = max,
                    finishTime = Time.unscaledTimeAsDouble + animationDuration
                };
            }
        }

        private void Refresh()
        {
            float progress = GetTransitionProgress();

            long currentValue;

            if (progress > 0.999f)
            {
                SetBarVisual(transition.barTo);
                currentValue = transition.valueTo;
                enabled = false;
            }
            else
            {
                SetBarVisual(Mathf.Lerp(transition.barFrom, transition.barTo, progress));
                currentValue = (long)Mathf.Lerp(transition.valueFrom, transition.valueTo, progress);
            }

            switch (format)
            {
                case TextFormat.Value:
                    barText.text = $"{currentValue}";
                    break;

                case TextFormat.ValueWithMax:
                    barText.text = $"{currentValue}/{transition.valueMax}";
                    break;

                case TextFormat.Percentage:
                    if (transition.valueMax > 0)
                    {
                        barText.text = $"{(float)currentValue / transition.valueMax:0%}";
                    }
                    break;

                case TextFormat.None:
                    barText.text = null;
                    break;

                default:
                    Debug.LogError("No text format selected");
                    break;
            }

            void SetBarVisual(float progress)
            {
                progress = Mathf.Clamp01(progress);
                barGraphic.anchorMin = Vector2.zero;
                barGraphic.anchorMax = new Vector2(barGraphicMinAnchor + (1f - barGraphicMinAnchor) * progress, 1f);
                barGraphic.anchoredPosition = Vector2.zero;
                barGraphic.sizeDelta = Vector2.zero;
            }
        }

        private float GetTransitionProgress()
        {
            float timeLeft = (float)(transition.finishTime - Time.unscaledTimeAsDouble);

            if (timeLeft < 0f) { return 1f; }

            float progressLeft = timeLeft / animationDuration;

            return 1f - progressLeft * progressLeft;
        }
    }
}
