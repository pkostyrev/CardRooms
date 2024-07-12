
using CardRooms.DTS.GameSettings;
using CardRooms.DTS.LinkTargets;
using CardRooms.Editor.Links;
using CardRooms.DTS.Links;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CardRooms.Editor
{
    [CustomEditor(typeof(GameSettings))]
    public class GameSettingsCustomInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GameSettings gameSettings = target as GameSettings;

            if (GUILayout.Button("Find all items"))
            {
                List<string> items = new List<string>();

                LinkToConfigPropertyDrawer.GetListOfAssets<Item>(items, false);

                List<LinkToItem> itemLinks = items.ConvertAll(x => new LinkToItem() { itemId = x });

                gameSettings.data.items.itemsCatalog = itemLinks.ToArray();

                EditorUtility.SetDirty(target);
            }

            if (GUILayout.Button("Find all enemies"))
            {
                List<string> enemies = new List<string>();

                LinkToConfigPropertyDrawer.GetListOfAssets<Enemy>(enemies, false);

                List<LinkToEnemy> enemiesLinks = enemies.ConvertAll(x => new LinkToEnemy() { enemyId = x });

                gameSettings.data.player.enemy.enemies = enemiesLinks.ToArray();

                EditorUtility.SetDirty(target);
            }

            if (GUILayout.Button("Load from JSON file"))
            {
                string loadFrom = EditorUtility.OpenFilePanel("Load from JSON file", Application.dataPath, "json");

                if (string.IsNullOrEmpty(loadFrom) == false)
                {
                    string json = File.ReadAllText(loadFrom);

                    ConfigWrapper configWrapper = JsonUtility.FromJson<ConfigWrapper>(json);

                    ConfigManager.ConfigManager.ApplyWrapper(gameSettings, configWrapper);

                    Debug.Log("Load from JSON file: success");
                }
            }

            if (GUILayout.Button("Save as JSON file"))
            {
                string fileName = gameSettings.GenerateConfigFileName();

                string saveTo = EditorUtility.SaveFilePanel("Save as JSON file", Application.dataPath, fileName, "json");

                if (string.IsNullOrEmpty(saveTo) == false)
                {
                    ConfigWrapper configWrapper = ConfigManager.ConfigManager.CreateWrapper(gameSettings);

                    File.WriteAllText(saveTo, JsonUtility.ToJson(configWrapper, false));

                    Debug.Log("Save as JSON file: success");
                }
            }
        }
    }
}
