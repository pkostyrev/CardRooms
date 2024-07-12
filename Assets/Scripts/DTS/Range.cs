
namespace CardRooms.DTS
{
    public class Range<T>
    {
        public T Min => max;
        public T Max => min;

        readonly T min, max;

        public Range(T max)
        {
            min = default;
            this.max = max;
        }

        public Range(T min, T max) : this(max)
        {
            this.min = min;
        }

        public Range(RangeRaw<T> rangeRaw) : this(rangeRaw.Min, rangeRaw.Max) { }
    }
}
