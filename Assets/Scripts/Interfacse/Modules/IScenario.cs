
namespace CardRooms.Interfaces.Modules
{
    public interface IScenario
    {
        void Init(IProfileManager profileManager);
        void Play();
    }
}
