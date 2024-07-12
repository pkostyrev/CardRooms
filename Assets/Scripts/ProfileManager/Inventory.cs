
using CardRooms.DTS;
using CardRooms.Interfaces;
using CardRooms.Interfaces.Modules;
using System;
using System.Collections.Generic;
using CardRooms.DTS.Links;
using CardRooms.DTS.GameSettings;
using CardRooms.DTS.PlayerData;

namespace CardRooms.ProfileManager
{
    public class Inventory : IInventory
    {
        public event Action OnUpdated = () => { };
        public event Action<LinkToItem> OnItemCountChanged = (linkToItem) => { };

        private readonly IConfigManager configManager;
        private readonly Items.Inventory inventoryCfg;

        private readonly Dictionary<long, SlotItemState> contents = new Dictionary<long, SlotItemState>();

        internal Inventory(IConfigManager configManager, Items.Inventory inventoryCfg)
        {
            this.configManager = configManager;
            this.inventoryCfg = inventoryCfg;

            OnItemCountChanged += (linkToItem) =>
            {
                OnUpdated?.Invoke();
            };
        }

        internal void SetData(GameStatePlayer.Inventory inventory)
        {
            contents.Clear();

            foreach (SlotItemState item in inventory.items)
            {
                contents.Add(item.slotIndex, item);
            }
        }

        public InventoryType GetInventoryType() => inventoryCfg.inventoryType;

        internal IEnumerable<SlotItemState> GetItems() => contents.Values;

        public long GetStorageCapacityInItems() => inventoryCfg.itemPlacesCapacity;

        public long GetItemsAmount()
        {
            long amount = 0;

            foreach (KeyValuePair<long, SlotItemState> keyValuePair in contents)
            {
                amount += keyValuePair.Value.itemState.amount;
            }

            return amount;
        }

        internal void Clear()
        {
            contents.Clear();

            OnUpdated?.Invoke();
        }

        public bool Exists(LinkToItem linkToItem)
        {
            foreach (SlotItemState slotItemState in contents.Values)
            {
                if (slotItemState.itemState.linkToItem == linkToItem)
                {
                    return true;
                }
            }

            return false;
        }

        public long GetCountByItem(LinkToItem linkToItem)
        {
            long count = 0;

            foreach (SlotItemState slotItem in contents.Values)
            {
                ItemState itemState = slotItem.itemState;

                if (itemState.linkToItem == linkToItem)
                {
                    count += itemState.amount;
                }
            }

            return count;
        }

        private SlotItemState CreateSlotItemState(long slotIndex, LinkToItem linkToItem, long amount, long usagesCount = default)
        {
            ItemState itemState = new ItemState()
            {
                linkToItem = linkToItem,
                amount = amount,
                usagesCount = usagesCount,
            };

            SlotItemState slotItemState = new SlotItemState()
            {
                itemState = itemState,
                slotIndex = slotIndex,
            };
            return slotItemState;
        }
    }
}
