
using CardRooms.DTS.Links;
using System;

namespace CardRooms.DTS.GameSettings
{
    [Serializable]
    public class Game
    {
        [Serializable]
        public struct GenerationEnemy
        {
            [Serializable]
            public struct Parametr
            {
                public DifficultyType difficulty;
                public RangeRaw<int> range;
                public float worth;
            }

            public Parametr[] healthParametrs;
            public Parametr[] damageParametrs;
        }

        [Serializable]
        public struct GenerationRoom
        {
            public RangeRaw<int> enemyCounrRange;
            public float worth;
        }

        [Serializable]
        public struct GenerationLevel
        {
            public RangeRaw<int> roomCounrRange;
            public float worth;
        }


        public GenerationEnemy generationEnemy;
        public GenerationRoom[] generationRoom;
        public GenerationLevel[] generationLevel;
        public LinkToEnemy[] allEnemies;
    }
}
