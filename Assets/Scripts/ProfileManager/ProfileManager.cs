
using CardRooms.DTS.PlayerData;
using CardRooms.Interfaces.Modules;
using System;
using UnityEngine;

namespace CardRooms.ProfileManager
{
    public class ProfileManager : MonoBehaviour, IProfileManager
    {
        public event Action OnUpdated = () => { };

        public bool IsLoaded => isLoaded;

        public IProfileManagerItems Items => items;
        public IProfileManagerRooms Rooms => rooms;

        [SerializeField] private ProfileManagerItems items;
        [SerializeField] private ProfileManagerRooms rooms;

        private bool isLoaded = false;

        public void Init(IConfigManager configManager)
        {
            items.Init(configManager, this);

            items.OnUpdated += () => OnUpdated?.Invoke();
        }

        public void ApplyGameState(GameStatePlayer state)
        {
            items.Load(state.inventories);
            isLoaded = true;
        }

        public GameStatePlayer GetGameState()
        {
            return new GameStatePlayer()
            {
                inventories = items.GetState()
            };
        }
    }
}
