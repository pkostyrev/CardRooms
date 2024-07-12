
using CardRooms.Common.Promises;
using System;

namespace CardRooms.Interfaces.Modules
{
    public interface IUIManager
    {
        public enum WindowCloseBlockState : byte
        {
            None,
            ByEscape,
            Total
        }

        event Action OnNewWindowShown;
        event Action OnTransitionHalfWay;
        //WindowCloseBlockState WindowCloseBlockedState { get; set; }
        //IWindowOverlay WindowOverlay { get; }
        //IHud Hud { get; }
        //ITutorialOverlay TutorialOverlay { get; }
        //IInterfaceAnimation InterfaceAnimation { get; }
        void Init(IEventSystemControl eventSystemControl, IConfigManager configManager);
        IPromise<T> StackingShow<T>() where T : class, IWindow<object>;
        IPromise<T> StackingShow<T, I>(I initData) where T : class, IWindow<I>;
        //IPromise StackingHide<T>();
        //Type GetActiveWindowType();
        //bool IsWindowInStack(Type type);
        IUIManager ClearStack();
        //IUIManager ClearStackExceptType<T>();
        //IUIManager AddToStack<T>() where T : class, IWindow<object>;
        //IUIManager RemoveFromStack<T>() where T : class, IWindow;
        //void PutCurrentStackToMemory();
        //IPromise RestoreStackFromMemory();
        //bool IsPopupAllowed();
        bool IsFullscreenWindowShown();
        //void IsPointHandledByUI(Vector2 screenPosition, out bool handlesClick, out bool handlesDrag);
    }
}
