
using CardRooms.DTS.Links;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CardRooms.DTS
{
    [Serializable]
    public struct Room
    {
        [Serializable]
        public struct StartConditions
        {
            [HideInInspector] public List<string> roomsToBeCompletedIds;
        }

        public LinkToRoomStatic linkToRoomStatic;
        public LinkToRoomGenerator linkToRoomGenerator;
        [HideInInspector] public string roomId;
        public EnemyState[] enemies;
        public StartConditions startConditions;
    }
}
