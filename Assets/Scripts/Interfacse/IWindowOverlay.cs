
using CardRooms.DTS;
using CardRooms.Common.Promises;
using System;

namespace CardRooms.Interfaces
{
    public interface IWindowOverlay
    {
        event Action OnBackButtonClick;
        IPromise Show(IWindow window, WindowsSettings.Settings settings);
        void Die();
    }
}
