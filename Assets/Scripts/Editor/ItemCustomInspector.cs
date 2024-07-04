
using CardRooms.DTS;
using CardRooms.DTS.LinkTargets;
using UnityEditor;

namespace CardRooms.Editor
{
    [CustomEditor(typeof(Item))]
    [CanEditMultipleObjects]
    public class ItemCustomInspector : UnityEditor.Editor
    {
        private SerializedProperty spriteProperty;
        private SerializedProperty viewPrefabProperty;
        private SerializedProperty typeProperty;
        private SerializedProperty worthProperty;
        private ItemType selectedType;
        private SerializedProperty selectedTypeProperty;

        void OnEnable()
        {
            spriteProperty = serializedObject.FindProperty(nameof(Item.sprite));
            viewPrefabProperty = serializedObject.FindProperty(nameof(Item.viewPrefab));
            typeProperty = serializedObject.FindProperty(nameof(Item.data))
                .FindPropertyRelative(nameof(Item.data.type));
            worthProperty = serializedObject.FindProperty(nameof(Item.data))
                .FindPropertyRelative(nameof(Item.data.worth));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spriteProperty);
            EditorGUILayout.PropertyField(viewPrefabProperty);
            EditorGUILayout.PropertyField(typeProperty);
            EditorGUILayout.PropertyField(worthProperty);

            ItemType type = (ItemType)typeProperty.intValue;

            if (type != selectedType)
            {
                selectedTypeProperty = serializedObject
                    .FindProperty(nameof(Item.data))
                    .FindPropertyRelative(TypeToPropertyName(type));
                selectedType = type;
            }

            if (selectedTypeProperty != null)
            {
                EditorGUILayout.PropertyField(selectedTypeProperty, true);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private static string TypeToPropertyName(ItemType type)
        {
            string propertyName = type.ToString();

            return propertyName.Substring(0, 1).ToLower() + propertyName.Substring(1);
        }
    }
}
