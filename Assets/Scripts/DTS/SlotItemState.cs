
using System;

namespace CardRooms.DTS
{
    [Serializable]
    public struct SlotItemState
    {
        public long slotIndex;
        public ItemState itemState;

        public bool HasValue => itemState.HasValue;

        public override string ToString()
        {
            return $"slot index: {slotIndex}, item: {itemState}";
        }
    }
}
