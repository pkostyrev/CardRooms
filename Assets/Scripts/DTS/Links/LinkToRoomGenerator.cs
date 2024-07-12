
using System;

namespace CardRooms.DTS.Links
{
    [Serializable]
    public struct LinkToRoomGenerator : ILink
    {
        public string LinkedObjectId => dungeonGeneratorId;

        public string dungeonGeneratorId;

        private int cachedHashCode;

        public bool HasValue => Helper.HasValue(dungeonGeneratorId);

        public static bool operator ==(LinkToRoomGenerator a, LinkToRoomGenerator b) => a.HasValue && b.HasValue && string.Equals(a.dungeonGeneratorId, b.dungeonGeneratorId);
        public static bool operator !=(LinkToRoomGenerator a, LinkToRoomGenerator b) => a == b == false;
        public override bool Equals(object obj) => obj is LinkToRoomGenerator link && this == link;
        public override int GetHashCode() => Helper.GetHashCode(ref cachedHashCode, dungeonGeneratorId);
        public override string ToString() { return $"{nameof(LinkToRoomGenerator)}: {LinkedObjectId}"; }
    }
}
