
using System;
using System.Collections.Generic;

namespace CardRooms.DTS
{
    [Serializable]
    public struct ItemStates
    {
        public List<ItemState> items;

        public int ItemsCount => items == null ? 0 : items.Count;

        public bool HasValue
        {
            get
            {
                if (items != null)
                {
                    foreach (ItemState itemState in items)
                    {
                        if (itemState.HasValue == true)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        public static ItemStates FromCost(Cost cost)
        {
            List<ItemState> items = new List<ItemState>();

            if (cost.HasValue == true)
            {
                foreach (Cost.Item costItem in cost.Items)
                {
                    items.Add(ItemState.FromCostItem(costItem));
                }
            }

            return new ItemStates() { items = items };
        }

        public static ItemStates FromItemState(ItemState itemState)
        {
            List<ItemState> items = new List<ItemState>() { itemState };

            return new ItemStates() { items = items };
        }
    }
}
