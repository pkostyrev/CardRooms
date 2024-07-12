
namespace CardRooms.Interfaces.Modules
{
    public interface ICacheManager
    {
        void Init();
        ICacheStorage<T> GetCacheStorage<T>(bool rememberBetweenSessions);
    }

    public interface ICacheStorage
    {
        bool HasValue { get; }

        void Clear();
    }

    public interface ICacheStorage<T> : ICacheStorage
    {
        T Value { get; set; }
    }
}
