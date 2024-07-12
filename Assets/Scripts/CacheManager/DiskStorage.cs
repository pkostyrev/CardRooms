
//#define SAVE_GAME_DEBUG

using System;
using UnityEngine;

namespace CardRooms.CacheManager
{
    internal class DiskStorage<T>
    {
        private const int IDLE_TIME_BEFORE_SAVE_TO_DISK = 5;
        private const int MAX_SECONDS_WITHOUT_SAVE_TO_DISK = 60;

        private readonly string cacheId;

        internal DiskStorage(string cacheId, IUnityInvocations unityInvocations)
        {
            this.cacheId = cacheId;

            unityInvocations.OnUpdateEvent += Update;
            unityInvocations.OnApplicationPauseEvent += TrySaveNow;
            unityInvocations.OnDestroyEvent += TrySaveNow;
        }

        private float? firstSaveCommandTime = null;
        private float? lastSaveCommandTime = null;

        private bool saveCandidateExists = false;
        private T saveCandidate;

        internal bool TryLoad(out T value)
        {
            if (PlayerPrefs.HasKey(cacheId) == false)
            {
                value = default;
                return false;
            }

            try
            {
                string json = PlayerPrefs.GetString(cacheId);
                value = JsonUtility.FromJson<T>(json);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                value = default;
                return false;
            }
        }

        internal void Save(T value)
        {
            if (firstSaveCommandTime.HasValue == false)
            {
                firstSaveCommandTime = Time.unscaledTime;
            }
            lastSaveCommandTime = Time.unscaledTime;

            saveCandidateExists = true;
            saveCandidate = value;
        }

        internal void Clear()
        {
            if (firstSaveCommandTime.HasValue == false)
            {
                firstSaveCommandTime = Time.unscaledTime;
            }
            lastSaveCommandTime = Time.unscaledTime;

            saveCandidateExists = false;
        }

        private void Update()
        {
            if (
                firstSaveCommandTime.HasValue && Time.unscaledTime > firstSaveCommandTime.Value + MAX_SECONDS_WITHOUT_SAVE_TO_DISK
                ||
                lastSaveCommandTime.HasValue && Time.unscaledTime > lastSaveCommandTime.Value + IDLE_TIME_BEFORE_SAVE_TO_DISK
                )
            {
                SaveNow();
            }
        }

        private void TrySaveNow()
        {
            if (firstSaveCommandTime.HasValue == true || lastSaveCommandTime.HasValue == true)
            {
                SaveNow();
            }
        }

        private void SaveNow()
        {
#if SAVE_GAME_DEBUG
            string path = System.IO.Path.Combine(Application.dataPath, $"{cacheId}.json");
#endif

            if (saveCandidateExists == true)
            {
                string json = JsonUtility.ToJson(saveCandidate);
                PlayerPrefs.SetString(cacheId, json);
#if SAVE_GAME_DEBUG
                System.IO.File.WriteAllText(path, json);
#endif
            }
            else
            {
                if (PlayerPrefs.HasKey(cacheId))
                {
                    PlayerPrefs.DeleteKey(cacheId);
                }

#if SAVE_GAME_DEBUG
                System.IO.File.Delete(path);
#endif
            }

            firstSaveCommandTime = null;
            lastSaveCommandTime = null;
        }
    }
}
