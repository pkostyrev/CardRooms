
using CardRooms.DTS.GameSettings;
using CardRooms.DTS.LinkTargets;
using System;

namespace CardRooms.ConfigManager
{
    internal static class ConfigPostProcessors
    {
        internal static readonly Action<ConfigWrapper, string, RoomStatic> PostProcessOnLoadRoomStatic = (configWrapper, id, roomStatic) =>
        {
            foreach (ConfigWrapper.RoomStatic x in configWrapper.roomStatics)
            {
                if (x.id == id)
                {
                    roomStatic.data = x.data;
                    return;
                }
            }
        };

        internal static readonly Action<ConfigWrapper, string, RoomGenerator> PostProcessOnRoomGenerator = (configWrapper, id, roomGenerator) =>
        {
            foreach (ConfigWrapper.RoomGenerator x in configWrapper.roomGenerators)
            {
                if (x.id == id)
                {
                    roomGenerator.data = x.data;
                    return;
                }
            }
        };

        internal static readonly Action<ConfigWrapper, string, Item> PostProcessOnLoadItem = (configWrapper, id, item) =>
        {
            foreach (ConfigWrapper.Item x in configWrapper.items)
            {
                if (x.id == id)
                {
                    item.data = x.data;
                    return;
                }
            }
        };

        internal static readonly Action<ConfigWrapper, string, Enemy> PostProcessOnLoadEnemy = (configWrapper, id, enemy) =>
        {
            foreach (ConfigWrapper.Enemy x in configWrapper.enemys)
            {
                if (x.id == id)
                {
                    enemy.data = x.data;
                    return;
                }
            }
        };
    }
}
