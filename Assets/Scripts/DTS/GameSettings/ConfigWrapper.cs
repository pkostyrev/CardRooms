
using System;

namespace CardRooms.DTS.GameSettings
{
    [Serializable]
    public struct ConfigWrapper
    {
        [Serializable]
        public struct Item
        {
            public string id;
            public LinkTargets.Item.Data data;
        }

        [Serializable]
        public struct Enemy
        {
            public string id;
            public LinkTargets.Enemy.Data data;
        }

        [Serializable]
        public struct RoomStatic
        {
            public string id;
            public LinkTargets.RoomStatic.Data data;
        }

        [Serializable]
        public struct RoomGenerator
        {
            public string id;
            public LinkTargets.RoomGenerator.Data data;
        }

        public GameSettings.Data gameSettings;
        public Item[] items;
        public Enemy[] enemys;
        public RoomStatic[] roomStatics;
        public RoomGenerator[] roomGenerators;
    }
}
