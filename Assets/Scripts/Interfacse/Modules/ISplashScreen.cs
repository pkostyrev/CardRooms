
using UnityEngine;

namespace CardRooms.Interfaces.Modules
{
    public interface ISplashScreen
    {
        void Init(IConfigManager configManager);
        GameObject ShowImmediatelyAndDetachFromRoot();
        void Show();
        void Hide();
    }
}
