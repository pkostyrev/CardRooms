
using System;

namespace CardRooms.DTS.Links
{
    [Serializable]
    public struct LinkToRoomStatic : ILink
    {
        public string LinkedObjectId => roomStaticId;

        public string roomStaticId;

        private int cachedHashCode;

        public bool HasValue => Helper.HasValue(roomStaticId);

        public static bool operator ==(LinkToRoomStatic a, LinkToRoomStatic b) => a.HasValue && b.HasValue && string.Equals(a.roomStaticId, b.roomStaticId);
        public static bool operator !=(LinkToRoomStatic a, LinkToRoomStatic b) => a == b == false;
        public override bool Equals(object obj) => obj is LinkToRoomStatic link && this == link;
        public override int GetHashCode() => Helper.GetHashCode(ref cachedHashCode, roomStaticId);
        public override string ToString() { return $"{nameof(LinkToRoomStatic)}: {LinkedObjectId}"; }
    }
}
