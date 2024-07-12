
using CardRooms.DTS;
using CardRooms.Root;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CozyCardRoomsIslands.Editor
{
    [CustomEditor(typeof(WindowsSettings))]
    public class WindowsSettingsCustomInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            WindowsSettings windowsSettings = target as WindowsSettings;

            if (GUILayout.Button("Find all windows"))
            {
                GameObject[] windows = Resources.LoadAll<GameObject>(Root.ConfigManagerEditor.GameSettings.userInterface.pathToWindowPrefabsFolder);

                List<WindowsSettings.Settings> newSettings = new List<WindowsSettings.Settings>();

                foreach (GameObject window in windows)
                {
                    string windowType = window.name;

                    WindowsSettings.Settings existing = Array.Find(windowsSettings.settings, s => s.windowType == windowType);
                    if (existing != null)
                    {
                        newSettings.Add(existing);
                        continue;
                    }

                    WindowsSettings.Settings emptySettings = new WindowsSettings.Settings();
                    emptySettings.windowType = windowType;
                    newSettings.Add(emptySettings);
                }

                windowsSettings.settings = newSettings.ToArray();

                EditorUtility.SetDirty(target);
            }
        }
    }
}
