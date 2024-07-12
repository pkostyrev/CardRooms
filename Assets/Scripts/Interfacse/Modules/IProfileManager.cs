
using CardRooms.DTS;
using CardRooms.DTS.Links;
using CardRooms.DTS.PlayerData;
using System;
using System.Collections.Generic;

namespace CardRooms.Interfaces.Modules
{
    public interface IProfileManager
    {
        event Action OnUpdated;
        bool IsLoaded { get; }

        IProfileManagerItems Items { get; }
        IProfileManagerRooms Rooms { get; }

        public void Init(IConfigManager configManager);

        void ApplyGameState(GameStatePlayer gameStatePlayer);
        GameStatePlayer GetGameState();
    }

    public interface IProfileManagerItems
    {
        event Action OnUpdated;
        event Action<LinkToItem> OnItemCountChanged;

        IInventory GetInventory(InventoryType inventoryType);

    }

    public interface IProfileManagerRooms
    {
        event Action OnUpdated;

        public LinkToEnemy GetWeakestEnemy(List<LinkToEnemy> enemies, out float wight);
        public LinkToEnemy GetStrongestEnemy(List<LinkToEnemy> enemies, out float wight);
    }
}
