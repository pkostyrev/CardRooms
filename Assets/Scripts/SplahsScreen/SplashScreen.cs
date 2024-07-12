
using CardRooms.Common;
using CardRooms.Interfaces.Modules;
using UnityEngine;

namespace CardRooms.SplahsScreen
{
    public class SplashScreen : MonoBehaviour, ISplashScreen
    {
        [SerializeField] private GameObject root;
        [SerializeField] private GameObject cameraRoot;
        [SerializeField] private AnimatedBar animatedBar;
        [SerializeField] private CanvasGroup splashCanvasGroup;
        [SerializeField] private AnimationCurve fadeOutCurve;
        [SerializeField] private float disableCooldown;

        private float? hideCommandTimeUnscaled = null;

        public void Init(IConfigManager configManager)
        {
            configManager.DownloadProgress.OnValueChanged += OnDownloadProgressChanged;
            OnDownloadProgressChanged(configManager.DownloadProgress.Value);
        }

        private void OnDownloadProgressChanged(float normalized)
        {
            const long max = 100;
            long current = (long)(max * normalized);
            animatedBar.Set(current, max);
        }

        private void Update()
        {
            if (hideCommandTimeUnscaled.HasValue == true)
            {
                float timeSinceHideCommand = Time.unscaledTime - hideCommandTimeUnscaled.Value;

                if (timeSinceHideCommand > disableCooldown)
                {
                    root.SetActive(false);
                    this.enabled = false;
                }
                else
                {
                    splashCanvasGroup.alpha = fadeOutCurve.Evaluate(timeSinceHideCommand);
                }
            }
        }

        public void Hide()
        {
            cameraRoot.SetActive(false);

            hideCommandTimeUnscaled = Time.unscaledTime;
        }

        public void Show()
        {
            hideCommandTimeUnscaled = null;
            cameraRoot.SetActive(true);
            this.enabled = true;
            splashCanvasGroup.alpha = 1f;

            root.SetActive(true);
        }

        public GameObject ShowImmediatelyAndDetachFromRoot()
        {
            Show();
            root.transform.SetParent(null);
            return root;
        }
    }
}
