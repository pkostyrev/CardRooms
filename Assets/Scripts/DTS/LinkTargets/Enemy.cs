
using System;
using UnityEngine;

namespace CardRooms.DTS.LinkTargets
{
    [CreateAssetMenu(fileName = nameof(Enemy), menuName = nameof(CardRooms) + "/" + nameof(Enemy))]
    public class Enemy : ScriptableObject
    {
        [Serializable]
        public struct Data
        {
            public RangeRaw<long> healthRange;
            public RangeRaw<long> damageRange;
            public bool canUseDefend;
        }

        public Sprite sprite;
        public GameObject viewPrefab;
        public Data data;
    }
}
