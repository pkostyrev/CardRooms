
namespace CardRooms.DTS.Links
{
    public static class Helper
    {
        public const string EmptyLinkKeyword = "null";

        public static bool HasValue(string linkedObjectId)
        {
            return
                string.IsNullOrEmpty(linkedObjectId) == false
                &&
                linkedObjectId != EmptyLinkKeyword;
        }

        public static int GetHashCode(ref int cachedHashCode, string linkedObjectId)
        {
            if (cachedHashCode == 0)
            {
                if (string.IsNullOrEmpty(linkedObjectId) == true)
                {
                    return 0;
                }

                cachedHashCode = linkedObjectId.GetHashCode();
            }

            return cachedHashCode;
        }
    }
}
