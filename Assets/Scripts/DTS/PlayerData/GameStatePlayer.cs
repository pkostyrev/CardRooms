
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
            public ItemStates items;
        }

        public Inventory[] inventories;
    }
}
