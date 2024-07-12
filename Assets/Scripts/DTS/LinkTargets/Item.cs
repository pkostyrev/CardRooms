
using CardRooms.DTS.Links;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CardRooms.DTS.LinkTargets
{
    [CreateAssetMenu(fileName = nameof(Item), menuName = nameof(CardRooms) + "/" + nameof(Item))]
    public class Item : ScriptableObject, ISerializationCallbackReceiver
    {

        [Serializable]
        public struct Data
        {
            [Serializable]
            public struct Card
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

                public Cost cost;
                public List<Effect> effects;
            }

            [Serializable]
            public struct Score
            {
            }

            public ItemType type;
            public float worth;
            public Card card;
            public Score score;
        }

        public Sprite sprite;
        public GameObject viewPrefab;
        public Data data;

        public void OnBeforeSerialize()
        {
            Data data = this.data;
            if (data.type != ItemType.Card) data.card = default;
            if (data.type != ItemType.Score) data.score = default;
            this.data = data;
        }

        public void OnAfterDeserialize()
        {
        }
    }
}
