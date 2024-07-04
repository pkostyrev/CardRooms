
using System;
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
                public Cost cost;
                public CardEffects cardEffects;
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
