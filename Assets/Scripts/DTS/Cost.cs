
using CardRooms.DTS.Links;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CardRooms.DTS
{
    [Serializable]
    public struct Cost
    {
        public Cost(Item item)
        {
            this.items = new List<Item>() { item };
            this.itemsMap = null;
        }

        public Cost(IEnumerable<Item> items)
        {
            this.items = new List<Item>(items);
            this.itemsMap = null;
        }

        public Cost(Cost other)
        {
            this.items = new List<Item>(other.items);
            this.itemsMap = null;
        }

        public Cost(ItemStates itemStates)
        {
            this.items = new List<Item>();
            if (itemStates.HasValue == true)
            {
                foreach (ItemState itemState in itemStates.items)
                {
                    this.items.Add(new Item(itemState));
                }
            }
            this.itemsMap = null;
        }

        [Serializable]
        public struct Item
        {
            public LinkToItem linkToItem;
            public long amount;

            public Item(ItemState itemState)
            {
                linkToItem = itemState.linkToItem;
                amount = itemState.amount;
            }
        }

        [SerializeField] private List<Item> items;

        private Dictionary<LinkToItem, long> itemsMap;

        public IEnumerable<Cost.Item> Items
        {
            get
            {
                InitItems();
                foreach (var item in this.itemsMap)
                {
                    yield return new Cost.Item() { linkToItem = item.Key, amount = item.Value };
                }
            }
        }

        private void InitItems()
        {
            if (itemsMap == null)
            {
                itemsMap = new Dictionary<LinkToItem, long>();
                if (this.items != null)
                {
                    foreach (Item item in this.items)
                    {
                        if (itemsMap.ContainsKey(item.linkToItem))
                        {
                            itemsMap[item.linkToItem] += item.amount;
                        }
                        else
                        {
                            itemsMap.Add(item.linkToItem, item.amount);
                        }
                    }
                }
            }
        }

        public int ItemsCount
        {
            get
            {
                InitItems();
                return this.itemsMap.Count;
            }
        }

        public bool TryGetSingleItem(out Item item)
        {
            item = default;

            if (ItemsCount != 1)
            {
                return false;
            }

            foreach (var x in itemsMap)
            {
                item = new Item() { linkToItem = x.Key, amount = x.Value };
            }

            return true;
        }

        public static Cost operator +(Cost c) => c;
        public static Cost operator -(Cost c) => c * (-1);

        public static Cost operator +(Cost l, Cost r)
        {
            if (!l.HasValue && !r.HasValue)
            {
                return default;
            }
            if (!l.HasValue && r.HasValue)
            {
                return new Cost(r);
            }
            if (!r.HasValue && l.HasValue)
            {
                return new Cost(l);
            }

            var sumItemsMap = new Dictionary<LinkToItem, long>();

            foreach (Cost.Item item in l.Items)
            {
                if (item.amount > 0)
                {
                    sumItemsMap.Add(item.linkToItem, item.amount);
                }
            }

            foreach (Cost.Item item in r.Items)
            {
                if (sumItemsMap.ContainsKey(item.linkToItem))
                {
                    sumItemsMap[item.linkToItem] += item.amount;
                    if (sumItemsMap[item.linkToItem] < 0)
                    {
                        Debug.LogError($"Negative cost: {sumItemsMap[item.linkToItem]}!");
                    }
                    else if (sumItemsMap[item.linkToItem] == 0)
                    {
                        sumItemsMap.Remove(item.linkToItem);
                    }
                }
                else
                {
                    if (item.amount < 0)
                    {
                        Debug.LogError($"Negative cost: {item.amount}!");
                    }
                    else if (item.amount > 0)
                    {
                        sumItemsMap.Add(item.linkToItem, item.amount);
                    }
                }
            }

            return new Cost()
            {
                items = MapToItems(sumItemsMap),
                itemsMap = sumItemsMap,
            };
        }

        public static Cost operator -(Cost l, Cost r) => l + (-r);

        public static Cost operator *(Cost l, long r)
        {
            return new Cost(GetMultiplied());

            IEnumerable<Item> GetMultiplied()
            {
                if (l.items != null)
                {
                    foreach (Item item in l.items)
                    {
                        yield return new Item() { linkToItem = item.linkToItem, amount = item.amount * r };
                    }
                }
            }
        }

        public static Cost Empty => new Cost() { items = null, itemsMap = null };

        public static Cost FromItem(LinkToItem linkToItem, long amount) => new Cost(new Item() { linkToItem = linkToItem, amount = amount });

        public bool HasValue => ItemsCount > 0;

        public override string ToString()
        {
            string result = string.Empty;

            foreach (Item i in Items)
            {
                result += $"Resource '{i.linkToItem.itemId}':{i.amount}; ";
            }

            return result;
        }

        private static List<Item> MapToItems(Dictionary<LinkToItem, long> dictionary)
        {
            return new List<Item>(Convert());

            IEnumerable<Item> Convert()
            {
                foreach (KeyValuePair<LinkToItem, long> item in dictionary)
                {
                    yield return new Item()
                    {
                        linkToItem = item.Key,
                        amount = item.Value,
                    };
                }
            }
        }
    }
}
