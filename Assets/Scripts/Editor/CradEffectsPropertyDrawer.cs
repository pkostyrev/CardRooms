
using CardRooms.DTS;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Effect = CardRooms.DTS.CardEffects.Effect;

namespace CardRooms.Editor
{
    [CustomPropertyDrawer(typeof(CardEffects))]
    public class CradEffectsPropertyDrawer : PropertyDrawer
    {
        private SerializedProperty effectsProperty;
        private bool effectsPropertyIsExpanded = false;
        private const float OFFSET_FOR_BUTTON = 3f;

        public override float GetPropertyHeight(SerializedProperty poperty, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;

            if (effectsPropertyIsExpanded)
            {
                height += (effectsProperty.arraySize + 1) * EditorGUIUtility.singleLineHeight + OFFSET_FOR_BUTTON;

                for (int i = 0; i < effectsProperty.arraySize; i++)
                {
                    height += EditorGUIUtility.singleLineHeight + OFFSET_FOR_BUTTON;

                    SerializedProperty effectProperty = effectsProperty.GetArrayElementAtIndex(i);

                    SerializedProperty typeProperty = effectProperty.FindPropertyRelative(nameof(Effect.type));

                    height += EditorGUI.GetPropertyHeight(typeProperty);
                    height += EditorGUI.GetPropertyHeight(effectProperty.FindPropertyRelative(nameof(Effect.worth)));

                    Effect.Type effectType = (Effect.Type)typeProperty.intValue;

                    if (effectType != Effect.Type.None)
                    {
                        height += EditorGUI.GetPropertyHeight(effectProperty.FindPropertyRelative(GetEffectConfigPropertyName(effectType)));
                    }
                }
            }

            return height;
        }

        public override void OnGUI(Rect mainRect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(mainRect, label, property);

            EditorGUI.indentLevel = 1;

            Rect rect = new Rect(mainRect.min.x, mainRect.min.y, mainRect.size.x, EditorGUIUtility.singleLineHeight);

            effectsPropertyIsExpanded = EditorGUI.Foldout(rect, effectsPropertyIsExpanded, "Effects", true);

            if (effectsPropertyIsExpanded)
            {
                effectsProperty = property.FindPropertyRelative(nameof(CardEffects.effects));

                for (int i = 0; i < effectsProperty.arraySize; i++)
                {
                    DrawEffect(i, ref rect);
                }

                if (GUI.Button(new Rect(rect.xMin + 30f, rect.min.y + EditorGUIUtility.singleLineHeight + OFFSET_FOR_BUTTON, rect.width - 30f, EditorGUIUtility.singleLineHeight), "AddEffect"))
                {
                    effectsPropertyIsExpanded = true;
                    effectsProperty.InsertArrayElementAtIndex(effectsProperty.arraySize);
                    effectsProperty.GetArrayElementAtIndex(effectsProperty.arraySize - 1).FindPropertyRelative(nameof(Effect.type)).intValue = 0;
                }

            }

            EditorGUI.EndProperty();
        }

        private void DrawEffect(int indexEffect, ref Rect rect)
        {
            EditorGUI.indentLevel = 2;

            SerializedProperty effectProperty = effectsProperty.GetArrayElementAtIndex(indexEffect);

            Effect.Type effectType = (Effect.Type)effectProperty.FindPropertyRelative(nameof(Effect.type)).intValue;

            rect = new Rect(rect.min.x, rect.min.y + EditorGUIUtility.singleLineHeight + OFFSET_FOR_BUTTON, rect.size.x, EditorGUIUtility.singleLineHeight);

            effectProperty.isExpanded = EditorGUI.Foldout(rect, effectProperty.isExpanded, effectType.ToString(), true);

            if (effectProperty.isExpanded)
            {
                EditorGUI.indentLevel = 3;

                Draw(nameof(Effect.type), ref rect);
                Draw(nameof(Effect.worth), ref rect);

                if (effectType != Effect.Type.None)
                {
                    SerializedProperty effectParammetrsProperty = effectProperty.FindPropertyRelative(GetEffectConfigPropertyName(effectType));
                    foreach (SerializedProperty childPoperty in effectParammetrsProperty)
                    {
                        DrawProperty(childPoperty, ref rect);
                    }
                }

                if (GUI.Button(new Rect(rect.xMin + 40f, rect.min.y + EditorGUIUtility.singleLineHeight + OFFSET_FOR_BUTTON, rect.width - 40f, EditorGUIUtility.singleLineHeight), "Delete"))
                {
                    effectsProperty.DeleteArrayElementAtIndex(indexEffect);
                    return;
                }

                rect = new Rect(rect.min.x, rect.min.y + EditorGUIUtility.singleLineHeight, rect.size.x, EditorGUIUtility.singleLineHeight);
            }

            void Draw(string propertyName, ref Rect rect)
            {
                DrawProperty(effectProperty.FindPropertyRelative(propertyName), ref rect);
            }

            void DrawProperty(SerializedProperty property, ref Rect rect)
            {
                rect = new Rect(rect.min.x, rect.min.y + EditorGUIUtility.singleLineHeight, rect.size.x, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(rect, property, true);

                if (property.isExpanded)
                {
                    rect = new Rect(rect.min.x, rect.min.y + EditorGUI.GetPropertyHeight(property), rect.size.x, EditorGUIUtility.singleLineHeight);
                }
            }
        }

        private static string GetEffectConfigPropertyName(Effect.Type type)
        {
            return type switch
            {
                Effect.Type.Damage => nameof(Effect.damage),
                Effect.Type.Health => nameof(Effect.health),
                Effect.Type.Protection => nameof(Effect.protection),
                _ => default
            };
        }
    }
}
