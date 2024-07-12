
using CardRooms.Interfaces.Modules;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CardRooms.CacheManager
{
    public class CacheManager : MonoBehaviour, ICacheManager, IUnityInvocations
    {
        public event Action OnUpdateEvent = () => { };
        public event Action OnApplicationPauseEvent = () => { };
        public event Action OnDestroyEvent = () => { };

        private readonly Dictionary<string, ICacheStorage> cacheStoragesCache = new Dictionary<string, ICacheStorage>();

        private void Update() => OnUpdateEvent();

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                OnApplicationPauseEvent();
            }
        }

        private void OnDestroy() => OnDestroyEvent();

        public void Init()
        {
        }

        public ICacheStorage<T> GetCacheStorage<T>(bool rememberBetweenSessions)
        {
            string cacheId = typeof(T).FullName;

            if (cacheStoragesCache.ContainsKey(cacheId) == false)
            {
                CacheStorage<T> cacheStorage = new CacheStorage<T>(cacheId, rememberBetweenSessions, this);
                cacheStoragesCache.Add(cacheId, cacheStorage);
                return cacheStorage;
            }
            else
            {
                if (cacheStoragesCache[cacheId] is CacheStorage<T> cacheStorage)
                {
                    if (cacheStorage.RememberBetweenSessions == rememberBetweenSessions)
                    {
                        return cacheStorage;
                    }
                    else
                    {
                        Debug.LogError($"CacheStorage of type '{typeof(T).Name}' exists but has another 'RememberBetweenSessions' setting, this is not supported");
                        return null;
                    }
                }
                else
                {
                    Debug.LogError($"CacheStorage with id '{cacheId}' used with different data types, this is not supported");
                    return null;
                }
            }
        }
    }
}
