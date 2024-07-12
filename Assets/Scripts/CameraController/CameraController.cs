
using CardRooms.Interfaces.Modules;
using UnityEngine;

namespace CardRooms.CameraController
{
    public class CameraController : MonoBehaviour, ICameraController
    {
        [SerializeField] private new Camera camera;

        private IUIManager uiManager;

        private bool lastCameraActiveState = true;

        public void Init(IUIManager uiManager)
        {
            this.uiManager = uiManager;

            this.uiManager.OnTransitionHalfWay += OnHalfWay;
        }


        public Vector3 ViewToScreen(Vector3 viewPosition) => camera.WorldToScreenPoint(viewPosition);

        public bool TryScreenToView(Vector2 screen, out Vector3 viewPosition)
        {
            Ray ray = camera.ScreenPointToRay(screen);
            if (ray.direction.y < 0)
            {
                viewPosition = ray.origin - ray.direction * (ray.origin.y / ray.direction.y);
                return true;
            }

            viewPosition = default;
            return false;
        }

        private void OnHalfWay()
        {
            bool active = uiManager.IsFullscreenWindowShown() == false;

            if (lastCameraActiveState != active)
            {
                camera.gameObject.SetActive(active);
                camera.Render();

                lastCameraActiveState = active;
            }
        }
    }
}
