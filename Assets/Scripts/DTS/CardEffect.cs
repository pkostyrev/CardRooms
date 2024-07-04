
using System;
using UnityEngine;

namespace CardRooms.DTS
{
    [Serializable]
    public struct CardEffects
    {
        [Serializable]
        public struct Effect : ISerializationCallbackReceiver
        {
            [Serializable]
            public enum Type : byte
            {
                None = 0,
                Health = 1,
                Damage = 2,
                Protection = 3
            }

            [Serializable]
            public struct Health
            {
                public int point;
            }

            [Serializable]
            public struct Damage
            {
                public int point;
            }

            [Serializable]
            public struct Protection
            {
            }

            public Type type;
            public float worth;
            public Health health;
            public Damage damage;
            public Protection protection;

            public void OnBeforeSerialize()
            {
                if (type != Type.Health) health = default;
                if (type != Type.Damage) damage = default;
                if (type != Type.Protection) protection = default;
            }

            public void OnAfterDeserialize()
            {
            }
        }

        public Effect[] effects;
    }
}
