
using CardRooms.DTS;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CardRooms.Common
{
    public static class RandomExtensions
    {
        public static T Random<T>(this T[] list)
        {
            return list[RandomIndex(list)];
        }

        public static T Random<T>(this List<T> list)
        {
            return list[RandomIndex(list)];
        }

        public static T Random<T>(this IEnumerable<T> values)
        {
            return values.ElementAt(RandomIndex(values));
        }

        public static int Random(this int value)
        {
            return RandomInt(0, value + 1);
        }

        public static int Random(this RangeRaw<int> range)
        {
            return RandomInt(range.Min, range.Max + 1);
        }

        public static int Random(this Range<int> range)
        {
            return RandomInt(range.Min, range.Max);
        }

        public static int RandomIndex<T>(this IEnumerable<T> values)
        {
            return RandomInt(0, values.Count() - 1);
        }

        public static float Random(this float value)
        {
            return RandomFloat(0, value);
        }

        public static float Random(this RangeRaw<float> range)
        {
            return RandomFloat(range.Min, range.Max);
        }

        public static float Random(this Range<float> range)
        {
            return RandomFloat(range.Min, range.Max);
        }

        public static float Random(this Range<float> range, float maxValueInRange)
        {
            return RandomFloat(range.Min, Mathf.Clamp(maxValueInRange, range.Min, range.Max));
        }

        private static int RandomInt(int x1, int x2)
        {
            return UnityEngine.Random.Range(0, x2 + 1);
        }

        private static float RandomFloat(float x1, float x2)
        {
            return UnityEngine.Random.Range(x1, x2);
        }
    }
}
