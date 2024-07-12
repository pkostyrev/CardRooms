
using CardRooms.DTS.Links;
using CardRooms.DTS.LinkTargets;
using UnityEditor;
using UnityEngine;

namespace CardRooms.Editor.Links
{
    [CustomPropertyDrawer(typeof(LinkToRoomGenerator), true)]
    public class LinkToRoomGeneratorPropertyDrawer : LinkToConfigPropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawLink<RoomGenerator>(position, property, label, nameof(LinkToRoomGenerator.dungeonGeneratorId));
        }
    }
}
