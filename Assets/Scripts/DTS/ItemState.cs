
using CardRooms.DTS.Links;
using System;

namespace CardRooms.DTS
{
    [Serializable]
    public struct ItemState
    {
        public LinkToItem linkToItem;
        public long amount;
        public long usagesCount;

        public static ItemState FromCostItem(Cost.Item item) => FromLinkAndAmount(item.linkToItem, item.amount);

        public static ItemState FromLinkAndAmount(LinkToItem linkToItem, long amount) => new ItemState() { linkToItem = linkToItem, amount = amount };

        public bool HasValue => linkToItem.HasValue == true && amount != 0;

        public static bool AreIdentical(ItemState state1, ItemState state2)
        {
            return
                state1.linkToItem == state2.linkToItem
                &&
                state1.amount == state2.amount
                &&
                state1.usagesCount == state2.usagesCount
                ;
        }

        public override string ToString() => $"item:'{linkToItem.itemId}', amount:{amount}";//, usagesCount:{usagesCount}";
    }
}
