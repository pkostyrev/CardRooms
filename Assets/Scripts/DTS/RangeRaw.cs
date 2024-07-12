using System;
using UnityEngine;

namespace CardRooms.DTS
{
    [Serializable]
    public struct RangeRaw<T>
    {
        public T Min => min;
        public T Max => max;

        [SerializeField] T min;
        [SerializeField] T max;
    }
}
