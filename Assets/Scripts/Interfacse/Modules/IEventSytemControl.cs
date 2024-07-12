
using CardRooms.Common.StatefulEvent;
using CardRooms.DTS;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace CardRooms.Interfaces.Modules
{
    public interface IEventSystemControl
    {
        IStatefulEvent<bool> HasSomeUserInput { get; }
        IStatefulEvent<bool> BlockedState { get; }
        void Init();
        void Block(EventSystemClient client);
        void Release(EventSystemClient client);
        void RaycastAll(PointerEventData pointerEventData, List<RaycastResult> results);
    }
}
