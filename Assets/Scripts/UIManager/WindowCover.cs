
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CardRooms.UIManager
{
    public class WindowCover : MonoBehaviour
    {
        public event Action OnCoverClick = () => { };

        [SerializeField] private Graphic cover;
        [SerializeField] private float fadeSpeed = 2f;
        [SerializeField] private Color tintColor = new Color(0f, 0f, 0f, 0.86f);

        public bool Enabled => goalStateEnabled;

        private float currentTint = 0f;
        private bool goalStateEnabled = false;
        private bool goalStateTint = false;

        private void Awake()
        {
            if (cover.GetComponent<Button>())
            {
                cover.GetComponent<Button>().onClick.AddListener(() => { OnCoverClick(); });
            }
        }

        private void Update()
        {
            currentTint = Mathf.MoveTowards(currentTint, goalStateTint ? 1f : 0f, Time.unscaledDeltaTime * fadeSpeed);

            Color c = tintColor;
            c.a = Mathf.Lerp(0, c.a, currentTint);
            cover.color = c;

            if (currentTint <= 0.01f && goalStateEnabled == false)
            {
                gameObject.SetActive(false);
            }
        }

        public void SetMode(bool enabled, bool showTint)
        {
            gameObject.SetActive(true);
            goalStateEnabled = enabled;
            goalStateTint = showTint;
            cover.raycastTarget = enabled;
        }
    }
}
