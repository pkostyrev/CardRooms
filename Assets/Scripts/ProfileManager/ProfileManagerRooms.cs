
using CardRooms.DTS;
using CardRooms.DTS.Links;
using CardRooms.DTS.LinkTargets;
using CardRooms.DTS.PlayerData;
using CardRooms.Interfaces.Modules;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace CardRooms.ProfileManager
{
    public class ProfileManagerRooms : MonoBehaviour, IProfileManagerRooms
    {
        public event Action OnUpdated;

        private IConfigManager configManager;
        private ProfileManager profileManager;

        private readonly List<RoomState> rooms = new List<RoomState>();
        private readonly List<RoomGenerator> roomGenerators = new List<RoomGenerator>();
        private readonly Dictionary<LinkToEnemy, Range<float>> enemiesWightRange = new Dictionary<LinkToEnemy, Range<float>>();

        internal void Init(IConfigManager configManager, ProfileManager profileManager)
        {
            this.configManager = configManager;
            this.profileManager = profileManager;
        }

        public IEnumerable<(LinkToEnemy, Range<float>)> GetAllEnemyToWight(List<LinkToEnemy> enemies, float maxWight)
        {
            foreach (LinkToEnemy enemy in enemiesWightRange.Where(enemy => enemies.Contains(enemy.Key) && enemy.Value.Min <= maxWight).Select(e => e.Key))
            {
                yield return (enemy, enemiesWightRange[enemy]);
            }
        }

        public LinkToEnemy GetWeakestEnemy(List<LinkToEnemy> enemies, out float wight)
        {
            var enemy = enemiesWightRange.Where(enemy => enemies.Contains(enemy.Key)).OrderBy(e => e.Value.Min).First();
            wight = enemy.Value.Min;
            return enemy.Key;
        }

        public LinkToEnemy GetStrongestEnemy(List<LinkToEnemy> enemies, out float wight)
        {
            var enemy = enemiesWightRange.Where(enemy => enemies.Contains(enemy.Key)).OrderByDescending(e => e.Value.Max).First();
            wight = enemy.Value.Max;
            return enemy.Key;
        }

        private void Clear()
        {
            this.rooms.Clear();
        }

        internal void Load(RoomState[] roomStates, GameStatePlayer.RoomGeneration roomGenerationState)
        {
            if (roomStates != null)
            {
                LoadRoomsProgresses(roomStates);
            }

            CalculateEnemyWightRange();
        }

        public void CalculateEnemyWightRange()
        {
            foreach (LinkToEnemy linkToEnemy in configManager.GameSettings.player.enemy.enemies)
            {
                Enemy enemy = configManager.GetByLink(linkToEnemy);

                float minWight = enemy.data.healthRange.Min * configManager.GameSettings.player.enemy.enemyHealthUnitWeight +
                    enemy.data.damageRange.Min * configManager.GameSettings.player.enemy.enemyDefendWeight;

                float maxWight = enemy.data.healthRange.Max * configManager.GameSettings.player.enemy.enemyHealthUnitWeight +
                    enemy.data.damageRange.Max * configManager.GameSettings.player.enemy.enemyDefendWeight +
                    (enemy.data.canUseDefend ? configManager.GameSettings.player.enemy.enemyDefendWeight : 0);

                enemiesWightRange.Add(linkToEnemy, new Range<float>(minWight, maxWight));
            }
        }

        private void LoadRoomsProgresses(RoomState[] roomStates)
        {
            foreach (RoomState roomState in roomStates)
            {
                int index = rooms.FindIndex((x) => { return x.room.linkToRoomStatic == roomState.room.linkToRoomStatic; });
                if (index >= 0)
                {
                    rooms[index] = roomState;
                }
                else
                {
                    rooms.Add(roomState);
                }
            }
        }


        internal GameStatePlayer.Rooms GetState()
        {
            return new GameStatePlayer.Rooms()
            {
                roomStates = rooms.ToArray(),
            };
        }

        private void GenerateRooms()
        {
            foreach (RoomGenerator roomGenerator in roomGenerators)
            {
                if (roomGenerator.TryGenerateRooms(out List<RoomState> roomStates) == true)
                {
                    
                }
            }
        }

        internal bool AllRoomsComplited(LinkToRoomGenerator linkToRoomGenerator)
        {
            foreach (RoomState roomState in GetRooms(linkToRoomGenerator))
            {
                if (roomState.Completed == false)
                {
                    return false;
                }
            }
            return true;
        }

        private IEnumerable<RoomState> GetRooms(LinkToRoomGenerator linkToRoomGenerator)
        {
            foreach (RoomState roomState in rooms)
            {
                if (roomState.room.linkToRoomGenerator == linkToRoomGenerator)
                {
                    yield return roomState;
                }
            }
        }

        internal void ClearCurrentRooms(LinkToRoomGenerator linkToRoomGenerator)
        {
            for (int i = rooms.Count - 1; i >= 0; i--)
            {
                if (rooms[i].room.linkToRoomGenerator == linkToRoomGenerator)
                {
                    rooms.RemoveAt(i);
                }
            }
        }

        public static string GenerateRoomGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
