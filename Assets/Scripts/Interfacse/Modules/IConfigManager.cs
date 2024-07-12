
using CardRooms.Common.Promises;
using CardRooms.Common.StatefulEvent;
using CardRooms.DTS.Links;
using CardRooms.DTS.LinkTargets;

namespace CardRooms.Interfaces.Modules
{
    public interface IConfigManager
    {
        IStatefulEvent<float> DownloadProgress { get; }

        void Init();

#if UNITY_EDITOR
        void InitEditor();
#endif

        DTS.GameSettings.GameSettings.Data GameSettings { get; }

        IPromise LoadConfigs();

        Item GetByLink(LinkToItem link);
        Enemy GetByLink(LinkToEnemy link);
        RoomStatic GetByLink(LinkToRoomStatic link);
        RoomGenerator GetByLink(LinkToRoomGenerator link);
    }
}
