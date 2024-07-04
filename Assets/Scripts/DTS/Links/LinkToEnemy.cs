
using System;

namespace CardRooms.DTS.Links
{
    [Serializable]
    public struct LinkToEnemy : ILink
    {
        public string LinkedObjectId => enemyId;

        public string enemyId;

        private int cachedHashCode;

        public bool HasValue => Helper.HasValue(enemyId);

        public static bool operator ==(LinkToEnemy a, LinkToEnemy b) => a.HasValue && b.HasValue && string.Equals(a.enemyId, b.enemyId);
        public static bool operator !=(LinkToEnemy a, LinkToEnemy b) => a == b == false;
        public override bool Equals(object obj) => obj is LinkToEnemy link && this == link;
        public override int GetHashCode() => Helper.GetHashCode(ref cachedHashCode, enemyId);
        public override string ToString() { return $"{nameof(LinkToEnemy)}: {LinkedObjectId}"; }
    }
}
