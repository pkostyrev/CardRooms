
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using static CardRooms.DTS.LinkTargets.RoomGenerator;
using CardRooms.DTS;
using CardRooms.Common;
using CardRooms.DTS.Links;
using CardRooms.Interfaces.Modules;

namespace CardRooms.ProfileManager
{
    public class RoomGenerator
    {
        private DTS.LinkTargets.RoomGenerator config;
        private LinkToRoomGenerator linkToRoomGenerator;

        private IConfigManager configManager;
        private ProfileManagerRooms profileManagerRooms;

        public readonly List<Range<float>> enemyGenerationRulesWightRange = new List<Range<float>>();

        public RoomGenerator(LinkToRoomGenerator linkToRoomGenerator, IConfigManager configManager, ProfileManagerRooms profileManagerRooms)
        {
            this.configManager = configManager;
            this.profileManagerRooms = profileManagerRooms;

            config = configManager.GetByLink(linkToRoomGenerator);
            this.linkToRoomGenerator = linkToRoomGenerator;

            enemyGenerationRulesWightRange.AddRange(CalculateEnemyGenerationRulesWightRange(config.data.enemyGenerationRules));
        }

        public bool TryGenerateRooms(out List<RoomState> roomStates)
        {
            roomStates = default;

            if (profileManagerRooms.AllRoomsComplited(linkToRoomGenerator))
            {
                profileManagerRooms.ClearCurrentRooms(linkToRoomGenerator);

                roomStates = GenerateRooms();

                return true;
            }

            return false;
        }

        private IEnumerable<Range<float>> CalculateEnemyGenerationRulesWightRange(List<EnemyGenerationRule> enemyGenerationRules)
        {
            foreach (EnemyGenerationRule enemyGenerationRule in enemyGenerationRules)
            {
                profileManagerRooms.GetWeakestEnemy(enemyGenerationRule.enemies, out float minEnemyWight);
                float minWight = enemyGenerationRule.generateCountRange.Min * minEnemyWight;

                profileManagerRooms.GetStrongestEnemy(enemyGenerationRule.enemies, out float maxEnemyWight);
                float maxWight = enemyGenerationRule.generateCountRange.Max * maxEnemyWight;

                yield return new Range<float>(minWight, maxWight);
            }
        }

        private List<RoomState> GenerateRooms()
        {
            if (config.data.generateCount.Max <= 0)
            {
                Debug.LogError("Room generation failed. Generate max count <= 0.");
                return default;
            }

            if (config.data.enemyGenerationRules.Count == 0)
            {
                Debug.LogError("Room generation failed. No generation rules.");
                return default;
            }

            int generateCount = new Range<int>(config.data.generateCount).Random();

            IEnumerable<(float, EnemyGenerationRule)> enemyGenerationRules = GenerateEnemyGenerationRules(generateCount).OrderBy(e => e.Item1);

            return GenerateRooms(enemyGenerationRules).ToList();
        }

        private IEnumerable<(float, EnemyGenerationRule)> GenerateEnemyGenerationRules(int generateCount)
        {
            float wight = config.data.startWight;
            float minRoomWight = enemyGenerationRulesWightRange.Min(e => e.Min);

            for (int i = generateCount - 1; i >= 0; i--)
            {
                float maxWight = wight - minRoomWight * i;
                int randomIndex = enemyGenerationRulesWightRange.Where(w => w.Min <= maxWight).RandomIndex();
                float roomWight = i == 0 ? wight : enemyGenerationRulesWightRange[randomIndex].Random(maxWight);

                wight -= roomWight;

                yield return (roomWight, config.data.enemyGenerationRules[randomIndex]);
            }
        }

        private IEnumerable<RoomState> GenerateRooms(IEnumerable<(float, EnemyGenerationRule)> enemyGenerationRules)
        {
            foreach ((float wight, EnemyGenerationRule enemyGenerationRule) in enemyGenerationRules)
            {
                Room room = new Room()
                {
                    linkToRoomGenerator = linkToRoomGenerator,
                    roomId = ProfileManagerRooms.GenerateRoomGuid(),
                    enemies = GenerateEnemies(enemyGenerationRule, wight).ToArray()
                };

                yield return new RoomState { room = room };
            }
        }


        private IEnumerable<EnemyState> GenerateEnemies(EnemyGenerationRule enemyGenerationRule, float wight)
        {
            profileManagerRooms.GetStrongestEnemy(enemyGenerationRule.enemies, out float maxEnemyWight);
            profileManagerRooms.GetWeakestEnemy(enemyGenerationRule.enemies, out float minEnemyWight);

            float correlation—oefficient = (wight - minEnemyWight) / (maxEnemyWight - minEnemyWight);

            RangeRaw<int> generateCountRange = enemyGenerationRule.generateCountRange;
            float generateCountFloat = correlation—oefficient * (generateCountRange.Max - generateCountRange.Min) + generateCountRange.Min;
            int generateCountToInt = Mathf.RoundToInt(generateCountFloat);
            int generateCount = Mathf.Clamp(generateCountToInt, generateCountRange.Min, generateCountRange.Max);

            float enemyHealthUnitWeight = configManager.GameSettings.player.enemy.enemyHealthUnitWeight;
            float enemyDamageUnitWeight = configManager.GameSettings.player.enemy.enemyDamageUnitWeight;
            float enemyDefendWeight = configManager.GameSettings.player.enemy.enemyDefendWeight;

            for (int i = generateCount - 1; i >= 0; i--)
            {
                float maxWight = wight - minEnemyWight * i;
                (LinkToEnemy link, Range<float> rangeWight) = profileManagerRooms.GetAllEnemyToWight(enemyGenerationRule.enemies, maxWight).Random();
                float enemyWight = i == 0 ? wight : rangeWight.Random(maxWight);

                wight -= enemyWight;

                DTS.LinkTargets.Enemy enemy = configManager.GetByLink(link);

                EnemyState enemyState = new EnemyState
                {
                    enemy = link,
                    maxHp = enemy.data.healthRange.Min,
                    damage = enemy.data.damageRange.Min
                };

                enemyWight -= enemy.data.healthRange.Min * enemyHealthUnitWeight + enemy.data.damageRange.Min * enemyDamageUnitWeight;

                float maxAddedHp = enemy.data.healthRange.Max - enemy.data.healthRange.Min;
                float maxAddedDamage = enemy.data.damageRange.Max - enemy.data.damageRange.Min;
                float probabilityAddDamage = maxAddedDamage / (maxAddedHp + maxAddedDamage);

                bool canSetDefend = enemy.data.canUseDefend && enemyWight < enemyDefendWeight;
                bool canAddHp = CanAddHp();
                bool canAddDamage = CanAddDamage();

                while (canSetDefend || canAddDamage || canAddHp)
                {
                    if (canSetDefend)
                    {
                        enemyState.canDefend = Random.Range(0f, 1f) > 0.5;
                        enemyWight -= enemyDefendWeight;
                        canSetDefend = false;
                    }

                    if (canAddDamage && Random.Range(0f, 1f) <= probabilityAddDamage)
                    {
                        enemyState.damage++;
                        enemyWight -= enemyDamageUnitWeight;
                        canAddDamage = CanAddDamage();
                    }
                    else if (canAddHp)
                    {
                        enemyState.maxHp++;
                        enemyWight -= enemyHealthUnitWeight;
                        canAddHp = CanAddHp();
                    }
                }

                wight += enemyWight;
                enemyState.currentHp = enemyState.maxHp;

                yield return enemyState;

                bool CanAddHp()
                {
                    return enemyState.maxHp < enemy.data.healthRange.Max && enemyWight < enemyHealthUnitWeight;
                }

                bool CanAddDamage()
                {
                    return enemyState.damage < enemy.data.damageRange.Max && enemyWight < enemyDamageUnitWeight;
                }
            }
        }
    }
}
