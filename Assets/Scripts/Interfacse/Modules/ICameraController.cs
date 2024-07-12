
using UnityEngine;

namespace CardRooms.Interfaces.Modules
{
    public interface ICameraController
    {
        void Init(IUIManager uiManager);

        Vector3 ViewToScreen(Vector3 viewPosition);
        bool TryScreenToView(Vector2 screen, out Vector3 viewPosition);
    }
}
