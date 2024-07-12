
using System;
using UnityEngine;

namespace CardRooms.DTS.GameSettings
{
    [CreateAssetMenu(fileName = nameof(GameSettings), menuName = nameof(CardRooms) + "/" + nameof(GameSettings))]
    public class GameSettings : ScriptableObject
    {
        [Serializable]
        public class Data
        {
            public Items items;
            public Player player;
        }

        public Data data;
    }
}
