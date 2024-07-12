
using CardRooms.DTS.PlayerData;
using CardRooms.DTS;
using CardRooms.Interfaces.Modules;
using System;
using System.Collections.Generic;
using UnityEngine;
using CardRooms.DTS.Links;
using CardRooms.Interfaces;
using CardRooms.DTS.GameSettings;

namespace CardRooms.ProfileManager
{
    public class ProfileManagerItems : MonoBehaviour, IProfileManagerItems
    {
        public event Action OnUpdated;
        public event Action<LinkToItem> OnItemCountChanged;

        private IConfigManager configManager;
        private ProfileManager profileManager;

        private readonly Dictionary<InventoryType, Inventory> inventories = new Dictionary<InventoryType, Inventory>();

        internal void Init(IConfigManager configManager, ProfileManager profileManager)
        {
            this.configManager = configManager;
            this.profileManager = profileManager;

            Clear();
        }

        internal void Load(GameStatePlayer.Inventory[] inventories)
        {
            Clear();

            if (inventories != null)
            {
                foreach (GameStatePlayer.Inventory inventory in inventories)
                {
                    if (this.inventories.ContainsKey(inventory.inventoryType))
                    {
                        this.inventories[inventory.inventoryType].SetData(inventory);
                    }
                    else
                    {
                        Debug.LogError($"inventory of type {inventory.inventoryType} couldn't be loaded");
                    }
                }
            }
        }

        internal GameStatePlayer.Inventory[] GetState()
        {
            List<GameStatePlayer.Inventory> resources = new List<GameStatePlayer.Inventory>();

            foreach ((InventoryType type, Inventory inventory) in inventories)
            {
                resources.Add(new GameStatePlayer.Inventory()
                {
                    inventoryType = type,
                    items = new List<SlotItemState>(inventory.GetItems()).ToArray()
                });
            }

            return resources.ToArray();
        }

        internal void Clear()
        {
            foreach (Items.Inventory cfg in configManager.GameSettings.items.inventories)
            {
                if (inventories.ContainsKey(cfg.inventoryType) == true)
                {
                    inventories[cfg.inventoryType].Clear();
                }
                else
                {
                    Inventory inventory = new Inventory(configManager, cfg);

                    inventories.Add(cfg.inventoryType, inventory);

                    inventory.OnUpdated += () =>
                    {
                        OnUpdated();
                    };

                    inventory.OnItemCountChanged += (linkToItem) =>
                    {
                        OnItemCountChanged(linkToItem);
                    };
                }
            }
        }

        public IInventory GetInventory(InventoryType inventoryType) => inventories[inventoryType];
    }
}
