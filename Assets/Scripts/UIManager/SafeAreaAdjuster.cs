
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CardRooms.UIManager
{
    public class SafeAreaAdjuster : MonoBehaviour
    {
        [SerializeField] private CanvasScaler canvasScaler;
        [SerializeField] private Vector2 minScreenSize;

#if UNITY_EDITOR
        [SerializeField] private Rect debugRect;
        [SerializeField] private KeyCode debugKey;
#endif

        private Rect lastSafeArea = new Rect(0, 0, 0, 0);

        private readonly List<RectTransform> adjustPanels = new List<RectTransform>();

        private void Update()
        {
            Refresh(false);
        }

        public void AddAdjustPanel(RectTransform rectTransform)
        {
            if (rectTransform.anchorMax != Vector2.one || rectTransform.anchorMin != Vector2.zero)
            {
                Debug.Log($"window {rectTransform.name} will not be adjusted to safe area - it's rect not linked to corners and it doesn't require adjustment");
                return;
            }

            for (int i = adjustPanels.Count - 1; i >= 0; i--)
            {
                if (adjustPanels[i] == null)
                {
                    adjustPanels.RemoveAt(i);
                }
            }

            adjustPanels.Add(rectTransform);

            Refresh(true);
        }

        private void Refresh(bool force)
        {
            Rect safeArea = Screen.safeArea;

#if UNITY_EDITOR
            if (Input.GetKey(debugKey) == true)
            {
                Vector2 screen = new Vector2(Screen.width, Screen.height);

                safeArea = new Rect(Vector2.Scale(debugRect.min, screen), Vector2.Scale(debugRect.size, screen));
            }
#endif

            if (safeArea != lastSafeArea)
            {
                lastSafeArea = safeArea;

                ApplySafeArea(safeArea);

                Debug.Log($"New safe area applied to {name}: x={safeArea.x}, y={safeArea.y}, w={safeArea.width}, h={safeArea.height} on full extents w={Screen.width}, h={Screen.height}");
            }
            else if (force == true)
            {
                ApplySafeArea(safeArea);
            }
        }

        private void ApplySafeArea(Rect safeArea)
        {
            Vector2 anchorMin = safeArea.position;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;

            Vector2 anchorMax = safeArea.position + safeArea.size;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            foreach (RectTransform adjustPanel in adjustPanels)
            {
                if (adjustPanel != null)
                {
                    adjustPanel.anchorMin = anchorMin;
                    adjustPanel.anchorMax = anchorMax;
                }
            }

            canvasScaler.referenceResolution = new Vector2(minScreenSize.x / (anchorMax.x - anchorMin.x), minScreenSize.y / (anchorMax.y - anchorMin.y));
        }
    }
}
