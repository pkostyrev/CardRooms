
using CardRooms.DTS.Links;
using System;
using UnityEngine;

namespace CardRooms.DTS.LinkTargets
{
    [CreateAssetMenu(fileName = nameof(RoomStatic), menuName = nameof(CardRooms) + "/" + nameof(RoomStatic))]
    public class RoomStatic : ScriptableObject
    {
        [Serializable]
        public struct Data
        {
            [Serializable]
            public struct StartConditions
            {
                public LinkToRoomStatic[] roomsToBeCompleted;
            }

            public EnemyState[] enemies;
            public StartConditions startConditions;
        }

        public Data data;
    }
}
