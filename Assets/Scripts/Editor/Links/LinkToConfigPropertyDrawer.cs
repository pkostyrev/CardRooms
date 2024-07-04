﻿
using CardRooms.DTS.Links;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CardRooms.Editor.Links
{
    public class LinkToConfigPropertyDrawer : PropertyDrawer
    {
        private const string emptyLinkDisplayValue = "No Link";
        private const string refreshCommandDisplayValue = "Refresh...";

        private static readonly Dictionary<string, List<string>> valuesBufferDict = new Dictionary<string, List<string>>();

        protected static void DrawLink<T>(Rect position, SerializedProperty property, GUIContent label, string nameOfId) where T : Object
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            DrawLinkToAssetGUI<T>(nameOfId, position, property);

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        private static void DrawLinkToAssetGUI<T>(string nameOfId, Rect position, SerializedProperty property) where T : Object
        {
            string currentValue = property.FindPropertyRelative(nameOfId).stringValue;
            string typeKey = typeof(T).FullName;

            if (valuesBufferDict.ContainsKey(typeKey) == false)
            {
                valuesBufferDict.Add(typeKey, new List<string>());
                GetListOfAssets<T>(valuesBufferDict[typeKey], true);
            }

            List<string> valuesBuffer = valuesBufferDict[typeKey];

            T assetValue = GetAsset<T>(nameOfId, property);

            string currentDisplayValue;

            bool currentIsEmpty = currentValue == Helper.EmptyLinkKeyword;

            if (currentIsEmpty)
            {
                currentDisplayValue = emptyLinkDisplayValue;
            }
            else
            {
                currentDisplayValue = currentValue;
            }

            int currentValueIndex = valuesBuffer.IndexOf(currentDisplayValue);

            const float clearButtonWidth = 18f;
            const float showButtonWidth = 25f;

            float y = position.y;
            float h = position.height;

            float x1 = position.x;
            float w1 = position.width - showButtonWidth - clearButtonWidth;

            float x2 = x1 + w1;
            float w2 = showButtonWidth;

            float x3 = x2 + w2;
            float w3 = clearButtonWidth;

            if (property.hasMultipleDifferentValues == true)
            {
                EditorGUI.LabelField(new Rect(x1, y, w1, h), $"__");
                return;
            }

            bool valid = assetValue != null || currentIsEmpty;

            Color oldColor = GUI.color;
            GUI.color = valid ? GUI.color : Color.red;

            currentValueIndex = EditorGUI.Popup(new Rect(x1, y, w1, h), currentValueIndex, valuesBuffer.ToArray());
            if (currentValueIndex >= 0 && currentValueIndex < valuesBuffer.Count)
            {
                if (valuesBuffer[currentValueIndex] == emptyLinkDisplayValue)
                {
                    property.FindPropertyRelative(nameOfId).stringValue = Helper.EmptyLinkKeyword;
                }
                else if (valuesBuffer[currentValueIndex] == refreshCommandDisplayValue)
                {
                    valuesBufferDict.Clear();
                    Debug.Log("Links cache refreshed");
                }
                else
                {
                    property.FindPropertyRelative(nameOfId).stringValue = valuesBuffer[currentValueIndex];
                }
            }

            GUI.color = oldColor;

            if (valid == false)
            {
                EditorGUI.LabelField(new Rect(x1, y, w1, h), $"Invalid link: " + (string.IsNullOrEmpty(currentValue) ? "null" : currentValue));
            }
            else
            {
                GUI.enabled = assetValue != null && currentIsEmpty == false;

                if (GUI.Button(new Rect(x2, y, w2, h), "go"))
                {
                    if (typeof(Component).IsAssignableFrom(typeof(T)))
                    {
                        EditorGUIUtility.PingObject((assetValue as Component).gameObject);
                    }
                    else
                    {
                        EditorGUIUtility.PingObject(assetValue);
                    }
                }

                if (GUI.Button(new Rect(x3, y, w3, h), "X"))
                {
                    property.FindPropertyRelative(nameOfId).stringValue = Helper.EmptyLinkKeyword;
                }
            }
        }

        protected static T GetAsset<T>(string nameOfId, SerializedProperty property) where T : Object
        {
            string currentValue = property.FindPropertyRelative(nameOfId).stringValue;

            string assetPath = Path.Combine("Assets", GetResourcesPathForAsset<T>(), $"{currentValue}.{GetExtension<T>()}");

            if (typeof(Component).IsAssignableFrom(typeof(T)))
            {
                return AssetDatabase.LoadAssetAtPath<GameObject>(assetPath)?.GetComponent<T>();
            }
            else
            {
                return AssetDatabase.LoadAssetAtPath<T>(assetPath);
            }
        }

        public static void GetListOfAssets<T>(List<string> buffer, bool addCommands)
        {
            string pathToSearch = Path.Combine(Application.dataPath, GetResourcesPathForAsset<T>());

            buffer.Clear();

            buffer.AddRange(Directory.GetFiles(pathToSearch, "*." + GetExtension<T>(), SearchOption.AllDirectories));

            for (int i = buffer.Count - 1; i >= 0; i--)
            {
                buffer[i] =
                    Path.Combine(Path.GetDirectoryName(buffer[i]), Path.GetFileNameWithoutExtension(buffer[i])) // get same path without extension
                        .Substring(pathToSearch.Length + 1) // remove path to config root && 1 separator
                        .Replace(Path.DirectorySeparatorChar, '/')
                        .Replace(Path.AltDirectorySeparatorChar, '/'); // replace all kind of path separators to unity-style path separator
            }

            if (addCommands == true)
            {
                buffer.Insert(0, emptyLinkDisplayValue);

                buffer.Add(refreshCommandDisplayValue);
            }
        }

        private static string GetResourcesPathForAsset<T>()
        {
            return Path.Combine("Resources", Path.Combine("LinkTargets", typeof(T).Name).Replace(@"\", @"/"));
        }

        internal static string GetExtension<T>()
        {
            if (typeof(ScriptableObject).IsAssignableFrom(typeof(T))) { return "asset"; }

            if (typeof(MonoBehaviour).IsAssignableFrom(typeof(T))) { return "prefab"; }

            if (typeof(GameObject).IsAssignableFrom(typeof(T))) { return "prefab"; }

            if (typeof(AudioClip).IsAssignableFrom(typeof(T))) { return "wav"; }

            if (typeof(SceneAsset).IsAssignableFrom(typeof(T))) { return "unity"; }

            return "*";
        }
    }
}
