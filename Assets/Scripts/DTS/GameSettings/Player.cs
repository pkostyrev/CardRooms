
using CardRooms.DTS.Links;
using CardRooms.DTS.PlayerData;
using System;

namespace CardRooms.DTS.GameSettings
{
    [Serializable]
    public class Player
    {
        [Serializable]
        public class Enemy
        {
            public float enemyHealthUnitWeight;
            public float enemyDamageUnitWeight;
            public float enemyDefendWeight;

            public LinkToEnemy[] enemies;
        }

        public GameStatePlayer defaultProfile;
        public Enemy enemy;
        public LinkToRoomGenerator[] roomGenerators;
    }
}