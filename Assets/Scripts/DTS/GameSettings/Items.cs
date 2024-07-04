
using CardRooms.DTS.Links;
using System;

namespace CardRooms.DTS.GameSettings
{
    [Serializable]
    public class Items
    {
        [Serializable]
        public struct Inventory
        {
            public InventoryType inventoryType;
            public ItemType[] allowedItems;
            public long itemPlacesCapacity;
        }

        public LinkToItem[] itemsCatalog;
        public Inventory[] inventories;

        public Inventory GetInventoryByType(InventoryType inventoryType)
        {
            foreach (Inventory inventory in inventories)
            {
                if (inventory.inventoryType == inventoryType)
                {
                    return inventory;
                }
            }

            return default;
        }
    }
}
