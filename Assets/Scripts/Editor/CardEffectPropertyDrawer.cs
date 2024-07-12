
using UnityEngine;
using UnityEditor;
using CardEffect = CardRooms.DTS.LinkTargets.Item.Data.Card.Effect;

namespace CardRooms.Editor
{
    [CustomPropertyDrawer(typeof(CardEffect))]
    public class CardEffectPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float result = 0f;

            result += EditorGUI.GetPropertyHeight(property.FindPropertyRelative(nameof(CardEffect.type)));

            CardEffect.Type cardEffectType = GetCardEffectType(property);
            if (cardEffectType != CardEffect.Type.None)
            {
                result += EditorGUI.GetPropertyHeight(property.FindPropertyRelative(GetEffectConfigPropertyName(cardEffectType)));
            }

            result += EditorGUI.GetPropertyHeight(property.FindPropertyRelative(nameof(CardEffect.worth)));

            return result;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            rect.height = 0f;

            Draw(nameof(CardEffect.type), ref rect);
            Draw(nameof(CardEffect.worth), ref rect);

            CardEffect.Type cardEffectType = GetCardEffectType(property);
            if (cardEffectType != CardEffect.Type.None)
            {
                Draw(GetEffectConfigPropertyName(cardEffectType), ref rect);
            }

            EditorGUI.EndProperty();

            void Draw(string propertyName, ref Rect rect)
            {
                SerializedProperty _property = property.FindPropertyRelative(propertyName);
                rect = new Rect(rect.x, rect.y + rect.height, rect.width, EditorGUI.GetPropertyHeight(_property));
                EditorGUI.PropertyField(rect, _property, true);
            }
        }

        private static CardEffect.Type GetCardEffectType(SerializedProperty rootProperty)
        {
            return (CardEffect.Type)rootProperty.FindPropertyRelative(nameof(CardEffect.type)).intValue;
        }

        private static string GetEffectConfigPropertyName(CardEffect.Type type)
        {
            return type switch
            {
                CardEffect.Type.Damage => nameof(CardEffect.damage),
                CardEffect.Type.Health => nameof(CardEffect.health),
                CardEffect.Type.Protection => nameof(CardEffect.protection),
                _ => default
            };
        }
    }
}
