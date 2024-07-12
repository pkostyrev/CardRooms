
using CardRooms.DTS.Links;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CardRooms.DTS.LinkTargets
{
    [CreateAssetMenu(fileName = nameof(RoomGenerator), menuName = nameof(CardRooms) + "/" + nameof(RoomGenerator))]
    public class RoomGenerator : ScriptableObject
    {
        [Serializable]
        public struct EnemyGenerationRule
        {
            public List<LinkToEnemy> enemies;
            public RangeRaw<int> generateCountRange;
        }

        [Serializable]
        public struct Data
        {
            public LinkToRoomStatic[] firstGeneratedRooms;
            public RangeRaw<int> generateCount;
            public List<EnemyGenerationRule> enemyGenerationRules;
            public float startWight;
            public float addedWeight;
            public float subtractedWeight;
        }

        public Data data;
    }
}
