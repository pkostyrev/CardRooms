
namespace CardRooms.Common.StatefulEvent
{
    public interface IValue<T>
    {
        bool Equals(T other);
    }
}
