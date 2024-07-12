
using CardRooms.DTS.Links;
using System;

namespace CardRooms.DTS
{
    [Serializable]
    public struct EnemyState
    {
        public LinkToEnemy enemy;
        public long maxHp;
        public long currentHp;
        public long damage;
        public bool canDefend;
        public bool isAttacking;
        public bool isProtected;

        public bool Died => currentHp <= 0;

        public void Attack()
        {
            isAttacking = false;
        }

        public void TakeDamage(long damage)
        {
            currentHp -= damage;
        }

        public void ChangeProtection()
        {
            isProtected = !isProtected;
        }
    }
}
