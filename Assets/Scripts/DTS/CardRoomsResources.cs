
using System;
using UnityEngine;

namespace CardRooms.DTS
{
    [CreateAssetMenu(fileName = nameof(CardRoomsResources), menuName = nameof(CardRooms) + "/" + nameof(CardRoomsResources))]
    public class CardRoomsResources : ScriptableObject
    {
        [Serializable]
        public class ButtonFeedbackSettings
        {
            public AnimationCurve pointerUp;
            public float pointerUpDuration;
            public AnimationCurve pointerDown;
            public float pointerDownDuration;
            public float scaleMultiplier;
            public float offsetMultiplier;
        }
        public ButtonFeedbackSettings buttonFeedbackSettings;
    }
}
