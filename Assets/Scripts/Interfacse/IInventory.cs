
using CardRooms.DTS;
using CardRooms.DTS.Links;
using System;

namespace CardRooms.Interfaces
{
    public interface IInventory
    {
        event Action OnUpdated;
        event Action<LinkToItem> OnItemCountChanged;
        long GetStorageCapacityInItems();
        long GetItemsAmount();
        InventoryType GetInventoryType();
        bool Exists(LinkToItem linkToItem);
        long GetCountByItem(LinkToItem linkToItem);
    }
}
