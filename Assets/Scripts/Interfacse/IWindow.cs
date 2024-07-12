
using CardRooms.DTS;
using CardRooms.Common.Promises;
using System;
using UnityEngine;

namespace CardRooms.Interfaces
{
    public interface IWindow
    {
        event Action<IWindow, WindowEventType> OnWindowEvent;
        void WarmUpFromUIManager(Func<IPromise> closeWindowRequest);
        IPromise SmoothDestroy();
        IPromise ShowFromUIManager(int id, bool thisWindowIsFirst, Action initAction);
        IPromise HideFromUIManager();
        void OnWindowCloseCommand();
        bool IsInitializedWith(int id);
        RectTransform GetRootRectTransform();
    }

    public interface IWindow<I> : IWindow
    {
        void InitFromUIManager(I initData);
    }
}
