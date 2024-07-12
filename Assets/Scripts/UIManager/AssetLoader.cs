
using CardRooms.Interfaces.Modules;
using System;
using UnityEngine;

namespace CardRooms.UIManager
{
    public class AssetLoader : MonoBehaviour
    {
        private IConfigManager configManager;

        internal void Init(IConfigManager configManager)
        {
            this.configManager = configManager;
        }

        internal T LoadAsset<T>() where T : UnityEngine.Object
        {
            string path = $"{configManager.GameSettings.userInterface.pathToWindowPrefabsFolder}/{typeof(T).Name}";

            T asset = Resources.Load<T>(path);

            if (asset == null)
            {
                Debug.LogError("can't load resource: '" + path + "'");
            }

            return asset;
        }

        internal GameObject LoadWindowGameObject(Type type)
        {
            string path = $"{configManager.GameSettings.userInterface.pathToWindowPrefabsFolder}/{type.Name}";

            GameObject asset = Resources.Load<GameObject>(path);

            if (asset == null)
            {
                Debug.LogError("can't load resource: '" + path + "'");
            }

            return asset;
        }
    }
}
