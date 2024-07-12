
using System;

namespace CardRooms.Interfaces.Modules
{
    public interface ISaveGameManager
    {
        event Action OnPlayerStateApplied;

        void Init(ICacheManager cacheManager, IConfigManager configManager, IProfileManager profileManager);
        void CreateNewGame();
        void LoadGame();
        void OverwriteSavedGame(string nameGame);
#if UNITY_EDITOR
        void SaveCurrentGame();
#endif
    }
}
