
using System;

namespace CardRooms.CacheManager
{
    public interface IUnityInvocations
    {
        event Action OnUpdateEvent;
        event Action OnApplicationPauseEvent;
        event Action OnDestroyEvent;
    }
}
