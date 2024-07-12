
using CardRooms.Common.Promises;
using CardRooms.Common.StatefulEvent;
using CardRooms.DTS.PlayerData;
using CardRooms.Interfaces.Modules;
using System;
using System.Threading;
using UnityEditor.PackageManager;
using UnityEngine;

namespace CardRooms.SaveGameManager
{
    public class SaveGameManager : MonoBehaviour, ISaveGameManager
    {
        private const int saveGameWorkerUpdateInterval = 5000;
        private const int gameStateAttemptsCountToRaiseWarning = 3;

        public event Action OnPlayerStateApplied = () => { };
        public IStatefulEvent<bool> GameSavedOnServer => gameSavedOnline;

        private IConfigManager configManager;
        private IProfileManager profileManager;

        private readonly ManualResetEvent stopSignal = new ManualResetEvent(false);
        private readonly object gameStateToSaveLocker = new();
        private long gameStateToSaveAttemptsCount = 0;
        private GameState gameStateToSaveLatest;

        private ICacheStorage<GameState> gameState;
        private bool saveGameSheduled = false;
        private DateTime lastSaveGameTime;
        private const float saveGameCoolDown = 5f;
        private readonly StatefulEventInt<bool> gameSavedOnline = StatefulEventInt.Create(true);

        public void Init(ICacheManager cacheManager, IConfigManager configManager, IProfileManager profileManager)
        {
            this.configManager = configManager;
            this.profileManager = profileManager;

            this.gameState = cacheManager.GetCacheStorage<GameState>(true);

            this.profileManager.OnUpdated += SheduleSaveGame;

            Thread thread = new Thread(SaveGameWorker);
            thread.Start();
        }

        private void Update()
        {
            if (saveGameSheduled == true && (DateTime.UtcNow - lastSaveGameTime).TotalSeconds > saveGameCoolDown)
            {
                SaveGameNow();
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause == true)
            {
                SaveGameNow();
            }
        }

        private void SaveGameNow()
        {
            if (profileManager.IsLoaded == true)
            {
                GameState gameStateToSave = GetGameState();

                lock (this.gameStateToSaveLocker)
                {
                    this.gameStateToSaveLatest = gameStateToSave;
                    this.gameStateToSaveAttemptsCount++;
                }
            }

            saveGameSheduled = false;
            lastSaveGameTime = DateTime.UtcNow;

            gameSavedOnline.Set(gameStateToSaveAttemptsCount < gameStateAttemptsCountToRaiseWarning);
        }

        public void CreateNewGame()
        {
            this.gameState.Clear();

            GameState gameState;

            gameState.player = configManager.GameSettings.player.defaultProfile;

            Apply(gameState);

            SheduleSaveGame();
        }

        public void LoadGame()
        {
            if (gameState.HasValue == true || TryLoadLegacyGameState())
            {
                Apply(gameState.Value);

                SheduleSaveGame();

                gameState.Clear();
            }
            else
            {
                CreateNewGame();
            }
        }

        bool TryLoadLegacyGameState()
        {
            OverwriteSavedGame("LegacyGameState");

            return gameState.HasValue;
        }

        public void SheduleSaveGame()
        {
            saveGameSheduled = true;
        }

#if UNITY_EDITOR
        public void SaveCurrentGame()
        {
            string fileName = DateTime.Now.ToString("yyyyMMddHHmmss");

            string saveTo = UnityEditor.EditorUtility.SaveFilePanel("Save as JSON file", Application.dataPath + "/Resources/GameSaves/", fileName, "json");

            if (string.IsNullOrEmpty(saveTo) == false)
            {
                System.IO.File.WriteAllText(saveTo, JsonUtility.ToJson(GetGameState(), true));

                Debug.Log("Save as JSON file: success");
            }
        }
#endif
        public void OverwriteSavedGame(string nameGame)
        {
            if (string.IsNullOrEmpty(nameGame) == false)
            {
                foreach (TextAsset textAsset in Resources.LoadAll<TextAsset>("GameSaves"))
                {
                    if (textAsset.name.Equals(nameGame))
                    {
                        string json = Resources.Load<TextAsset>($"GameSaves/{nameGame}").ToString();

                        this.gameState.Value = JsonUtility.FromJson<GameState>(json);

                        saveGameSheduled = false;

                        Debug.Log("Load from JSON file: success");

                        return;
                    }
                }
            }
        }

        private void Apply(GameState gameState)
        {
            profileManager.ApplyGameState(gameState.player);

            OnPlayerStateApplied();
            return;
        }

        private GameState GetGameState()
        {
            GameState gameState;
            gameState.player = profileManager.GetGameState();
            return gameState;
        }

        private void SaveGameWorker()
        {
            bool saveInProgress = false;

            while (stopSignal.WaitOne(saveGameWorkerUpdateInterval) == false)
            {
                if (saveInProgress == false && this.gameStateToSaveAttemptsCount > 0)
                {
                    GameState gameStateToSave;
                    long gameStateToSaveAttemptsCountToDiscard;

                    lock (this.gameStateToSaveLocker)
                    {
                        gameStateToSave = this.gameStateToSaveLatest;
                        gameStateToSaveAttemptsCountToDiscard = this.gameStateToSaveAttemptsCount;
                    }

                    if (gameStateToSaveAttemptsCountToDiscard > 0)
                    {
                        saveInProgress = true;

                        SaveGame(gameStateToSave)
                            .Done(() =>
                            {
                                Timers.Instance.WaitForMainThread(() =>
                                {
                                    lock (this.gameStateToSaveLocker)
                                    {
                                        this.gameStateToSaveAttemptsCount -= gameStateToSaveAttemptsCountToDiscard;
                                    }

                                    gameSavedOnline.Set(gameStateToSaveAttemptsCount < gameStateAttemptsCountToRaiseWarning);
                                });
                            })
                            .Fail(ex =>
                            {
                                Debug.LogError($"failed to save game: {ex.Message}");
                            })
                            .Always(() =>
                            {
                                saveInProgress = false;
                            });
                    }
                }
            }
        }


        IPromise SaveGame(GameState gameStateToSave)
        {
            string saveTo = $"{Application.dataPath}/Resources/GameSaves/LegacyGameState.json";

            if (string.IsNullOrEmpty(saveTo) == false)
            {
                System.IO.File.WriteAllText(saveTo, JsonUtility.ToJson(gameStateToSave, true));

                return Deferred.Resolved();
            }

            return Deferred.Rejected(new Exception("Save game failed"));
        }
    }
}
