
using System;

namespace CardRooms.DTS.Links
{
    [Serializable]
    public struct LinkToItem : ILink
    {
        public string LinkedObjectId => itemId;

        public string itemId;

        private int cachedHashCode;

        public bool HasValue => Helper.HasValue(itemId);

        public static bool operator ==(LinkToItem a, LinkToItem b) => a.HasValue && b.HasValue && string.Equals(a.itemId, b.itemId);
        public static bool operator !=(LinkToItem a, LinkToItem b) => a == b == false;
        public override bool Equals(object obj) => obj is LinkToItem link && this == link;
        public override int GetHashCode() => Helper.GetHashCode(ref cachedHashCode, itemId);
        public override string ToString() { return $"{nameof(LinkToItem)}: {LinkedObjectId}"; }
    }
}
