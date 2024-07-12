
using CardRooms.DTS.Links;
using CardRooms.DTS.LinkTargets;
using UnityEditor;
using UnityEngine;

namespace CardRooms.Editor.Links
{
    [CustomPropertyDrawer(typeof(LinkToRoomStatic), true)]
    public class LinkToRoomStaticPropertyDrawer : LinkToConfigPropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawLink<RoomStatic>(position, property, label, nameof(LinkToRoomStatic.roomStaticId));
        }
    }
}
