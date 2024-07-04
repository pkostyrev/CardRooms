
using CardRooms.DTS.LinkTargets;
using CardRooms.DTS.Links;
using UnityEditor;
using UnityEngine;

namespace CardRooms.Editor.Links
{
    [CustomPropertyDrawer(typeof(LinkToItem), true)]
    public class LinkToItemPropertyDrawer : LinkToConfigPropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawLink<Item>(position, property, label, nameof(LinkToItem.itemId));
        }
    }
}
