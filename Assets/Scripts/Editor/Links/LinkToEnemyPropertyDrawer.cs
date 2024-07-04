
using CardRooms.DTS.LinkTargets;
using CardRooms.DTS.Links;
using UnityEditor;
using UnityEngine;

namespace CardRooms.Editor.Links
{
    [CustomPropertyDrawer(typeof(LinkToEnemy), true)]
    public class LinkToEnemyPropertyDrawer : LinkToConfigPropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawLink<Enemy>(position, property, label, nameof(LinkToEnemy.enemyId));
        }
    }
}
