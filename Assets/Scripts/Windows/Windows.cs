
using CardRooms.Interfaces.Modules;
using UnityEngine;

namespace CardRooms.Windows
{
    public class Windows : MonoBehaviour, IWindows
    {
        public void Init()
        {

        }

        public void ShowMainMenu()
        {
            Root.Root.UIManager.ClearStack().StackingShow<MainMenu>();
        }
    }
}
