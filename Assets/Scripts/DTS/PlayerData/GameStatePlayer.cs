
using System;

namespace CardRooms.DTS.PlayerData
{
    [Serializable]
    public struct GameStatePlayer
    {
        [Serializable]
        public struct Inventory
        {
            public InventoryType inventoryType;
            public SlotItemState[] items;
        }

        [Serializable]
        public struct Rooms
        {
            public RoomState[] roomStates;
        }

        [Serializable]
        public struct RoomGeneration
        {
        }

        public Inventory[] inventories;
        public Rooms rooms;
    }
}
