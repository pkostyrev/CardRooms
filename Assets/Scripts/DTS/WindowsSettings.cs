
using CardRooms.DTS.Links;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CardRooms.DTS
{
    [CreateAssetMenu(fileName = nameof(WindowsSettings), menuName = nameof(CardRooms) + "/" + nameof(WindowsSettings))]
    public class WindowsSettings : ScriptableObject
    {
        [Serializable]
        public class Settings
        {
            public string windowType = string.Empty;
            public WindowBackgroundType windowBackgroundType = WindowBackgroundType.Empty;
            public bool tinted = false;
            public bool onTopOfOthers = false;
            public bool tapAnywhereToClose = false;
            public LinkToItem[] itemsToShow = new LinkToItem[0];
            public bool useSafeAreaAdjuster = true;

            public bool IsItemShown(LinkToItem linkToItem)
            {
                return Array.IndexOf(itemsToShow, linkToItem) >= 0;
            }
        }

        public GameObject windowOverlayPrefab;
        public Settings[] settings;

        private readonly Dictionary<Type, Settings> settingsCache = new Dictionary<Type, Settings>();

        public Settings Get(Type windowType)
        {
            if (settingsCache.TryGetValue(windowType, out Settings result) == true)
            {
                return result;
            }

            string windowTypeAsString = windowType.Name;

            foreach (Settings s in settings)
            {
                if (s.windowType == windowTypeAsString)
                {
                    settingsCache.Add(windowType, s);
                    return s;
                }
            }

            Debug.LogError($"WindowsSettings for window '{windowTypeAsString}' not defined");
            settingsCache.Add(windowType, new Settings());
            return settingsCache[windowType];
        }
    }
}
