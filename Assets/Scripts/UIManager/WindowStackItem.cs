
using CardRooms.Interfaces;
using System;

namespace CardRooms.UIManager
{
    internal class WindowStackItem
    {
        private static int nextId = 0;

        public readonly int id;
        public readonly Type type;
        public readonly Action<IWindow> initAction;

        public WindowStackItem(Type type, Action<IWindow> initAction)
        {
            this.id = nextId++;
            this.type = type;
            this.initAction = initAction;
        }
    }

    internal class WindowStackItem<T, I> : WindowStackItem where T : class, IWindow<I>
    {
        public WindowStackItem(I initData)
            : base(typeof(T), bw => (bw as T).InitFromUIManager(initData))
        {
        }
    }
}
