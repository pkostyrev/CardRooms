
using CardRooms.Common.Promises;
using CardRooms.DTS;
using CardRooms.Interfaces;
using CardRooms.Windows.Helpers;
using System;
using UnityEngine;

namespace CardRooms.Windows
{
    public abstract class BaseWindow : MonoBehaviour, IWindow
    {
        public event Action<IWindow, WindowEventType> OnWindowEvent = (window, et) => { };


        // used to hold (re)init commands until window is fully hidden and ready to update it's content
        // always resolved when window shown or showing
        // in pending mode during hiding animation
        private IPromise actionsGate = Deferred.GetFromPool().Resolve();

        public bool IsInitializedWith(int id) => this.initId == id;

        public bool Shown { get; private set; }
        public bool Focused { get; private set; }

        private WindowAnimator windowAnimator;

        private bool unloaded = false;

        protected int initId { get; private set; }

        private Func<IPromise> closeWindowRequest;

        protected void Start()
        {
            // don't use Start() in windows - use OnLoad
        }

        protected void Awake()
        {
            // don't use Awake() in windows - use OnLoad

            initId = -1;

            windowAnimator = GetComponent<WindowAnimator>();
            if (windowAnimator == null)
            {
                windowAnimator = gameObject.AddComponent<WindowAnimator>();
            }
        }

        protected virtual void Update()
        {

        }

        protected void OnDestroy()
        {
            // don't use OnDestroy() in windows - use OnUnload

            if (unloaded == false)
            {
                Debug.LogWarning("window is destroying externally without unloading using UI manager");
                OnUnload();
                unloaded = true;
            }
        }

        public RectTransform GetRootRectTransform() { return unloaded ? null : GetComponent<RectTransform>(); }

        public void WarmUpFromUIManager(Func<IPromise> closeWindowRequest)
        {
            this.closeWindowRequest = closeWindowRequest;

            OnLoad();
        }

        public IPromise SmoothDestroy()
        {
            if (this == null) { Deferred.GetFromPool().Resolve(); }

            return HideFromUIManager()
                .Done(() =>
                {
                    if (this != null)
                    {
                        OnUnload();
                        unloaded = true;
                        Destroy(gameObject);
                    }
                });
        }

        public IPromise ShowFromUIManager(int id, bool thisWindowIsFirst, Action initAction)
        {
            if (Shown == false)
            {
                Shown = true;

                gameObject.SetActive(true);

                actionsGate = actionsGate
                    .Then(() => Timers.Instance.WaitOneFrame())
                    .Done(() =>
                    {
                        if (initAction != null)
                        {
                            initAction();
                            initId = id;
                            OnInit();
                        }
                        gameObject.SetActive(true);
                    })
                    .Then(() => Timers.Instance.WaitOneFrame())
                    .Done(() => OnShowing())
                    .Then(() => windowAnimator.Show())
                    .Done(() =>
                    {
                        OnShown();
                        if (thisWindowIsFirst == true)
                        {
                            OnFocused();
                        }
                        else
                        {
                            OnUnfocused();
                        }
                    });
            }
            else
            {
                if (initAction != null)
                {
                    initAction();
                    initId = id;
                }

                OnSetInitDataWhileVisible();

                actionsGate = actionsGate.Then(() => Timers.Instance.WaitOneFrame()).Done(() =>
                {
                    if (thisWindowIsFirst == true)
                    {
                        OnFocused();
                    }
                    else
                    {
                        OnUnfocused();
                    }
                });
            }

            return actionsGate;
        }

        public IPromise HideFromUIManager()
        {
            if (Shown == true)
            {
                Shown = false;

                gameObject.SetActive(true);

                actionsGate = actionsGate
                    .Done(() => gameObject.SetActive(true))
                    .Then(() => Timers.Instance.WaitOneFrame())
                    .Done(() => OnUnfocused())
                    .Done(() => OnHiding())
                    .Then(() => windowAnimator.Hide())
                    .Done(() => gameObject.SetActive(false))
                    .Done(() => OnHidden());
            }

            return actionsGate;
        }

        public IPromise StackingHide() => closeWindowRequest();

        public virtual void OnWindowCloseCommand() { StackingHide(); }

        public virtual void OnLoad() { gameObject.SetActive(false); OnWindowEvent(this, WindowEventType.OnLoad); }

        public virtual void OnInit() { OnWindowEvent(this, WindowEventType.OnInit); }

        public virtual void OnShowing() { OnWindowEvent(this, WindowEventType.OnShowing); }

        public virtual void OnShown() { OnWindowEvent(this, WindowEventType.OnShown); }

        public virtual void OnSetInitDataWhileVisible() { OnWindowEvent(this, WindowEventType.OnSetInitDataWhileVisible); }

        public virtual void OnFocused()
        {
            Focused = true;
            OnWindowEvent(this, WindowEventType.OnFocused);
        }

        public virtual void OnUnfocused()
        {
            Focused = false;
            OnWindowEvent(this, WindowEventType.OnUnfocused);
        }

        public virtual void OnHiding() { OnWindowEvent(this, WindowEventType.OnHiding); }

        public virtual void OnHidden() { OnWindowEvent(this, WindowEventType.OnHidden); }

        public virtual void OnUnload() { OnWindowEvent(this, WindowEventType.OnUnload); }

        /*
        Order of events:

                OnLoad()
                    ||
                    \/                on going forward
               SetInitData(T) <==============================================
                    ||                                                     ||
                    \/                                                     ||
                 OnInit()                                                  ||
                    ||                                                     ||
                    \/         on going backward                           ||
                OnShowing() <==============================================||
      (can call private SetupContent())                                    ||
                    ||                                                     ||
                    \/                                                     ||
                 OnShown()                                                 ||
                    ||                                                     ||
                    \/               on going backward wihout hide         ||
       OnSetInitDataWhileVisible() <=================================      ||
                    \/                                             ||      ||
                    ||                                             ||      ||
        OnFocused() / OnUnfocused() =================================      ||
                    ||                                                     ||
                    \/                                                     ||
                OnHiding()                                                 ||
                    ||                                                     ||
                    \/                                                     ||
                OnHidden() ==================================================
                    ||
                    \/
                OnUnload()
        */
    }

    public abstract class BaseWindow<I> : BaseWindow, IWindow<I>
    {
        protected I initData;

        public void InitFromUIManager(I initData) { this.initData = initData; }
    }

    public abstract class BaseWindowNoInitData : BaseWindow<object>
    {

    }
}
