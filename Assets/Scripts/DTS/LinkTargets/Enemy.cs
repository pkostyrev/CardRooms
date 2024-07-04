
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
            public DifficultyType health;
            public DifficultyType damage;
            public bool canDefend;
        }

        public Sprite sprite;
        public GameObject viewPrefab;
        public Data data;
    }
}
