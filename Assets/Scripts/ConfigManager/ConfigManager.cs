
using CardRooms.Common.Promises;
using CardRooms.Common.StatefulEvent;
using CardRooms.DTS.GameSettings;
using CardRooms.DTS.Links;
using CardRooms.DTS.LinkTargets;
using CardRooms.Interfaces.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CardRooms.ConfigManager
{
    public class ConfigManager : MonoBehaviour, IConfigManager
    {
        public IStatefulEvent<float> DownloadProgress => downloadProgress;

        [SerializeField] private GameSettings gameSettings;

        public GameSettings.Data GameSettings => gameSettings.data;

        private readonly StatefulEventInt<float> downloadProgress = StatefulEventInt.Create(0f);

        private readonly Dictionary<Type, Dictionary<string, object>> cache = new Dictionary<Type, Dictionary<string, object>>();

        public void Init()
        {

        }

#if UNITY_EDITOR
        public void InitEditor()
        {
        }
#endif

        public IPromise LoadConfigs()
        {
            downloadProgress.Set(1f);
            return Deferred.Resolved();
        }

        public Item GetByLink(LinkToItem link) => GetByLink(link, ConfigPostProcessors.PostProcessOnLoadItem);
        public Enemy GetByLink(LinkToEnemy link) => GetByLink(link, ConfigPostProcessors.PostProcessOnLoadEnemy);
        public RoomStatic GetByLink(LinkToRoomStatic link) => GetByLink(link, ConfigPostProcessors.PostProcessOnLoadRoomStatic);
        public RoomGenerator GetByLink(LinkToRoomGenerator link) => GetByLink(link, ConfigPostProcessors.PostProcessOnRoomGenerator);

        private T GetByLink<T>(ILink link, Action<ConfigWrapper, string, T> PostProcessOnLoad) where T : UnityEngine.Object
        {
            if (cache.TryGetValue(typeof(T), out Dictionary<string, object> typedCache) == false)
            {
                typedCache = new Dictionary<string, object>();
                cache.Add(typeof(T), typedCache);
            }

            if (typedCache.TryGetValue(link.LinkedObjectId, out object cachedObject) == false)
            {
                cachedObject = Resources.Load<T>(Path.Combine(GetPathForAssetInsideResources<T>(), link.LinkedObjectId));
                typedCache.Add(link.LinkedObjectId, cachedObject);
            }

            return cachedObject as T;
        }

        public static string GetPathForAssetInsideResources<T>()
        {
            return Path.Combine("LinkTargets", typeof(T).Name).Replace(@"\", @"/");
        }

#if UNITY_EDITOR
        public static ConfigWrapper CreateWrapper(GameSettings gameSettings)
        {
            ConfigWrapper configWrapper;

            configWrapper.gameSettings = gameSettings.data;

            configWrapper.items = GrabAllConfigs<Item>()
                .ConvertAll(x => new ConfigWrapper.Item() { id = x.Item1, data = x.Item2.data })
                .ToArray();

            configWrapper.enemys = GrabAllConfigs<Enemy>()
                .ConvertAll(x => new ConfigWrapper.Enemy() { id = x.Item1, data = x.Item2.data })
                .ToArray();

            configWrapper.roomStatics = GrabAllConfigs<RoomStatic>()
                .ConvertAll(x => new ConfigWrapper.RoomStatic() { id = x.Item1, data = x.Item2.data })
                .ToArray();

            configWrapper.roomGenerators = GrabAllConfigs<RoomGenerator>()
                .ConvertAll(x => new ConfigWrapper.RoomGenerator() { id = x.Item1, data = x.Item2.data })
                .ToArray();

            return configWrapper;
        }

        public static void ApplyWrapper(GameSettings gameSettings, ConfigWrapper configWrapper)
        {
            gameSettings.data = configWrapper.gameSettings;
            UnityEditor.EditorUtility.SetDirty(gameSettings);

            ApplyAllConfigs<ConfigWrapper.Item, Item>(configWrapper.items, w => w.id, (w, t) => { t.data = w.data; });
            ApplyAllConfigs<ConfigWrapper.Enemy, Enemy>(configWrapper.enemys, w => w.id, (w, t) => { t.data = w.data; });

            UnityEditor.AssetDatabase.SaveAssets();
        }

        private static List<(string, T)> GrabAllConfigs<T>() where T : UnityEngine.Object
        {
            List<(string, T)> result = new List<(string, T)>();

            string[] guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T).Name}");

            foreach (string guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);

                string removePartStart = Path.Combine("Assets", "Resources", GetPathForAssetInsideResources<T>())
                    .Replace(Path.DirectorySeparatorChar, '/')
                    .Replace(Path.AltDirectorySeparatorChar, '/')
                    + "/";

                string removePartEnd = ".asset";

                if (path.StartsWith(removePartStart) && path.EndsWith(removePartEnd))
                {
                    string id = path
                        .Substring(0, path.Length - removePartEnd.Length)
                        .Substring(removePartStart.Length);

                    result.Add((id, UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path)));
                }
                else
                {
                    Debug.LogError($"invalid config path: {path}");
                }
            }

            return result;
        }

        private static void ApplyAllConfigs<W, T>(W[] configs, Func<W, string> idExtractor, Action<W, T> applyCallback) where T : UnityEngine.Object
        {
            foreach (W config in configs)
            {
                string pathToItemConfig = Path.Combine("Assets", "Resources", GetPathForAssetInsideResources<W>(), idExtractor(config) + ".asset");

                T itemCfg = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(pathToItemConfig);

                applyCallback(config, itemCfg);

                UnityEditor.EditorUtility.SetDirty(itemCfg);
            }
        }
#endif
    }
}
