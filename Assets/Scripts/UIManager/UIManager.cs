
using CardRooms.Common.Promises;
using CardRooms.DTS;
using CardRooms.Interfaces;
using CardRooms.Interfaces.Modules;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CardRooms.UIManager
{
    public class UIManager : MonoBehaviour, IUIManager
    {
        public event Action OnNewWindowShown = () => { };
        public event Action OnTransitionHalfWay = () => { };

        [SerializeField] private RectTransform windowsParentAboveCover;
        [SerializeField] private RectTransform windowsParentBelowCover;
        [SerializeField] private AssetLoader assetLoader;
        [SerializeField] private WindowCover windowCover;
        [SerializeField] private RectTransform overlayParent;
        [SerializeField] private SafeAreaAdjuster safeAreaAdjuster;
        [SerializeField] private SplashManager splashManager;

        public IUIManager.WindowCloseBlockState WindowCloseBlockedState { get; set; }
        private IEventSystemControl eventSystemControl;
        private IConfigManager configManager;
        private WindowsSettings windowsSettings;

        private IWindowOverlay windowOverlay;
        private readonly List<WindowStackItem> windowsStack = new List<WindowStackItem>();
        private IPromise transitionQueue = Deferred.GetFromPool().Resolve();

        private readonly List<IWindow> windows = new List<IWindow>();

        public void Init(IEventSystemControl eventSystemControl, IConfigManager configManager)
        {
            this.eventSystemControl = eventSystemControl;
            this.configManager = configManager;

            assetLoader.Init(configManager);

            this.windowsSettings = assetLoader.LoadAsset<WindowsSettings>();

            SetWindowOverlay(windowsSettings.windowOverlayPrefab);
        }

        private void Awake()
        {
            if (windowCover == null)
                Debug.LogError("windowCover is null");
            else
                windowCover.OnCoverClick += OnWindowCoverClick;

            windowCover.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) == true && eventSystemControl.BlockedState.Value == false)
            {
                transitionQueue = transitionQueue.Done(HandleEscapeButton);
            }
        }

        public IPromise<T> StackingShow<T, I>(I initData) where T : class, IWindow<I> => StackingShow<T>(new WindowStackItem<T, I>(initData));

        public IPromise<T> StackingShow<T>() where T : class, IWindow<object> => StackingShow<T, object>(null);

        public IUIManager ClearStack()
        {
            windowsStack.Clear();

            return this;
        }

        public bool IsFullscreenWindowShown()
        {
            foreach (WindowStackItem windowStackItem in windowsStack)
            {
                switch (windowsSettings.Get(windowStackItem.type).windowBackgroundType)
                {
                    case WindowBackgroundType.NothingIsVisible:
                    case WindowBackgroundType.DefaultCover:
                        return true;
                }
            }

            return false;
        }

        private void SetWindowOverlay(GameObject overlayPrefab)
        {
            if (this.windowOverlay != null)
            {
                windowOverlay.Die();
                this.windowOverlay = null;
            }

            GameObject overlayInstance = Instantiate(overlayPrefab, overlayParent);

            safeAreaAdjuster.AddAdjustPanel(overlayInstance.GetComponent<RectTransform>());

            this.windowOverlay = overlayInstance.GetComponent<IWindowOverlay>();

            if (this.windowOverlay == null)
            {
                Debug.LogError("window overlay interface not found");
                return;
            }
        }

        private void OnWindowCoverClick()
        {
            Type activeWindowType = GetInputHandlerWindowType();

            if (activeWindowType != null)
            {
                IWindow window = GetOrCreateWindowInstance(activeWindowType, out Exception exception);

                if (exception == null)
                {
                    if (windowsSettings.Get(activeWindowType).tapAnywhereToClose == true)
                    {
                        window.OnWindowCloseCommand();
                    }
                }
            }
        }

        private IPromise<T> StackingShow<T>(WindowStackItem stackItem) where T : class, IWindow
        {
            Deferred jobDone = Deferred.GetFromPool();
            transitionQueue.Done(() =>
            {
                transitionQueue = jobDone;
                windowsStack.Insert(0, stackItem);
                RefreshStackVisibility(true).Done(() =>
                {
                    jobDone.Resolve();
                });
            });

            Deferred<T> result = Deferred<T>.GetFromPool();

            transitionQueue.Done(() =>
            {
                result.Resolve(GetExistingWindow<T>());
            });

            transitionQueue.Fail(ex =>
            {
                result.Reject(ex);
                Debug.LogError(ex.Message);
                transitionQueue = Deferred.GetFromPool().Resolve();
            });

            return result;
        }

        private IPromise StackingHide(Type type)
        {
            if (WindowCloseBlockedState == IUIManager.WindowCloseBlockState.Total)
            {
                Deferred result = Deferred.GetFromPool();
                return result.Reject(new Exception("Windows closing blocked"));
            }

            Deferred jobDone = Deferred.GetFromPool();
            transitionQueue.Done(() =>
            {
                transitionQueue = jobDone;
                for (int i = 0; i < windowsStack.Count; i++)
                {
                    if (windowsStack[i].type == type)
                    {
                        windowsStack.RemoveAt(i);
                        break;
                    }
                }
                RefreshStackVisibility(false).Done(() =>
                {
                    jobDone.Resolve();
                });
            });
            return transitionQueue;
        }

        private void HandleEscapeButton()
        {
            if (eventSystemControl.BlockedState.Value)
            {
                return;
            }

            if (WindowCloseBlockedState == IUIManager.WindowCloseBlockState.ByEscape)
            {
                return;
            }

            if (windowsStack.Count > 0)
            {
                Type activeWindowType = GetInputHandlerWindowType();

                if (activeWindowType == null)
                {
                    return;
                }

                IWindow activeWindow = GetExistingWindow(activeWindowType);

                if (activeWindow == null)
                {
                    return;
                }

                activeWindow.OnWindowCloseCommand();
            }
        }

        private IWindow GetOrCreateWindowInstance(Type type, out Exception exception)
        {
            IWindow existing = GetExistingWindow(type);

            if (existing != null)
            {
                exception = null;
                return existing;
            }
            else
            {
                return InstantiateWindow(type, out exception);
            }
        }

        private IWindow InstantiateWindow(Type type, out Exception exception)
        {
            GameObject windowPrefab = assetLoader.LoadWindowGameObject(type);

            if (windowPrefab == null)
            {
                exception = new Exception("Can't load window prefab");
                return null;
            }

            GameObject windowInstanceGameObject = Instantiate(windowPrefab, windowsParentBelowCover);
            windowInstanceGameObject.name = windowPrefab.name;

            IWindow windowInstance = windowInstanceGameObject.GetComponent<IWindow>();
            if (windowInstance == null)
            {
                exception = new Exception("No IWindow script added");
                Debug.LogError(exception.Message);
                return null;
            }

            windowInstance.OnWindowEvent += OnBaseWindowEvent;

            windows.Add(windowInstance);

            WindowsSettings.Settings settings = windowsSettings.Get(type);

            if (settings.useSafeAreaAdjuster == true)
            {
                safeAreaAdjuster.AddAdjustPanel(windowInstance.GetRootRectTransform());
            }

            exception = null;
            return windowInstance;
        }

        private readonly List<IWindow> windowsCacheHideCandidates = new List<IWindow>();
        private readonly List<IPromise> windowsInstantiatePromisesCache = new List<IPromise>();
        private readonly List<IPromise> transitionPromisesCache = new List<IPromise>();
        private readonly HashSet<Type> processedWindowTypesCache = new HashSet<Type>();

        private IPromise RefreshStackVisibility(bool openingNewWindow)
        {
            this.eventSystemControl.Block(EventSystemClient.UIManager);

            //// cache all known windows to hide invisible later
            windowsCacheHideCandidates.Clear();
            windowsCacheHideCandidates.AddRange(windows);

            transitionPromisesCache.Clear();
            processedWindowTypesCache.Clear();

            WindowBackgroundType lastBackgroundType = WindowBackgroundType.Empty;
            Deferred cameraReadyToShowWindowsPromise = Deferred.GetFromPool();

            windowCover.SetMode(false, false);

            SortWindowsByPriority();

            for (int w = 0; w < windowsStack.Count; w++)
            {
                WindowStackItem windowStackItem = windowsStack[w];

                if (processedWindowTypesCache.Add(windowStackItem.type) == false)
                {
                    continue;
                }

                bool thisWindowIsFirst = w == 0;

                Deferred<IWindow> showPromise = Deferred<IWindow>.GetFromPool();

                // show window and call init only for first one
                IWindow window = GetOrCreateWindowInstance(windowStackItem.type, out Exception exception);
                int initId = windowStackItem.id;

                if (exception == null)
                {
                    Transform windowTransform = window.GetRootRectTransform();
                    windowTransform.SetParent(windowCover.Enabled ? windowsParentBelowCover : windowsParentAboveCover);
                    windowTransform.SetAsFirstSibling();

                    windowsInstantiatePromisesCache.Add(Deferred<IWindow>.Resolved(window));

                    bool withInit = true;

                    // can skip init only for previously inited windows
                    if (window.IsInitializedWith(initId) == true)
                    {
                        // don't reinit window if we are going back and window exists
                        if (openingNewWindow == false) { withInit = false; }

                        // don't reinit windows that not currently on top - even if it's visible at background.
                        if (thisWindowIsFirst == false) { withInit = false; }
                    }

                    Action initAction = null;

                    if (withInit == true)
                    {
                        initAction = () => windowStackItem.initAction(window);
                    }

                    cameraReadyToShowWindowsPromise
                        .Then(() => window.ShowFromUIManager(initId, thisWindowIsFirst, initAction))
                        .Done(() => { showPromise.Resolve(window); })
                        .Fail(ex => { showPromise.Reject(ex); });
                }
                else
                {
                    showPromise.Reject(exception);

                    windowsInstantiatePromisesCache.Add(Deferred<IWindow>.Rejected(exception));

                    this.eventSystemControl.Release(EventSystemClient.UIManager); // release event system on loading problems
                }

                transitionPromisesCache.Add(showPromise);

                // remove shown window from hide list
                for (int i = windowsCacheHideCandidates.Count - 1; i >= 0; i--)
                {
                    if (windowsCacheHideCandidates[i].GetType() == windowStackItem.type)
                    {
                        windowsCacheHideCandidates.RemoveAt(i);
                    }
                }

                bool stopStackProcessing = false;

                WindowsSettings.Settings windowSettings = windowsSettings.Get(windowStackItem.type);
                lastBackgroundType = windowSettings.windowBackgroundType;
                bool tinted = windowSettings.tinted;

                switch (lastBackgroundType)
                {
                    case WindowBackgroundType.DefaultCover:
                    case WindowBackgroundType.NothingIsVisible:
                        if (windowCover.Enabled == false)
                        {
                            windowCover.SetMode(true, tinted);
                        }
                        // when we show fullscreen window, break stack interation - all other windows can be hidden
                        stopStackProcessing = true;
                        break;

                    case WindowBackgroundType.RaycastBlocker:
                        if (windowCover.Enabled == false)
                        {
                            windowCover.SetMode(true, tinted);
                        }
                        break;

                    case WindowBackgroundType.Hud:
                        // when we show hud window, break stack interation - no windows expected behind hud
                        stopStackProcessing = true;
                        break;

                    case WindowBackgroundType.HudNonInteractable:
                        // when we show hud window, break stack interation - no windows expected behind hud
                        stopStackProcessing = true;
                        if (windowCover.Enabled == false)
                        {
                            windowCover.SetMode(true, tinted);
                        }
                        break;

                    case WindowBackgroundType.Skip:
                        break;
                }

                if (stopStackProcessing == true)
                {
                    break;
                }
            }

            IPromise allWindowsInstantiatePromise = Deferred.All(windowsInstantiatePromisesCache);
            windowsInstantiatePromisesCache.Clear();

            IPromise cameraPromise =
                splashManager.ApplyBackground(
                    lastBackgroundType,
                    () =>
                    {
                        OnTransitionHalfWay?.Invoke();
                        cameraReadyToShowWindowsPromise.Resolve();
                    },
                    configManager.GameSettings.userInterface.animationSpeed);

            transitionPromisesCache.Add(cameraPromise);

            // when show complete, hide all other windows
            for (int i = windowsCacheHideCandidates.Count - 1; i >= 0; i--)
            {
                IWindow window = windowsCacheHideCandidates[i];

                transitionPromisesCache.Add(window.HideFromUIManager());
            }

            windowsCacheHideCandidates.Clear();

            if (openingNewWindow == true)
            {
                OnNewWindowShown?.Invoke(); ;
            }

            IPromise allTransitionsPromise = Deferred.All(transitionPromisesCache);
            transitionPromisesCache.Clear();

            allTransitionsPromise.Done(() =>
            {
                this.eventSystemControl.Release(EventSystemClient.UIManager);
            });

            return allTransitionsPromise;
        }

        private void SortWindowsByPriority()
        {
            while (true)
            {
                bool stackChanged = false;

                for (int i = 0, maxi = windowsStack.Count - 2; i <= maxi; i++)
                {
                    WindowStackItem top = windowsStack[i];
                    bool topWindowIsOnTopOfOthers = windowsSettings.Get(top.type).onTopOfOthers;

                    WindowStackItem bottom = windowsStack[i + 1];
                    bool bottomWindowIsOnTopOfOthers = windowsSettings.Get(bottom.type).onTopOfOthers;

                    if (topWindowIsOnTopOfOthers == false && bottomWindowIsOnTopOfOthers == true)
                    {
                        windowsStack[i] = bottom;
                        windowsStack[i + 1] = top;
                        stackChanged = true;
                    }
                }

                if (stackChanged == false)
                {
                    break;
                }
            }
        }

        private Type GetInputHandlerWindowType()
        {
            foreach (WindowStackItem stackItem in windowsStack)
            {
                return stackItem.type;
            }

            return null;
        }

        private T GetExistingWindow<T>() where T : class, IWindow
        {
            return GetExistingWindow(typeof(T)) as T;
        }

        private IWindow GetExistingWindow(Type type)
        {
            for (int i = 0, maxi = windows.Count - 1; i <= maxi; i++)
            {
                if (windows[i].GetType() == type)
                {
                    return windows[i];
                }
            }

            return null;
        }

        private void OnBaseWindowEvent(IWindow window, WindowEventType eventType)
        {
            if (eventType == WindowEventType.OnShowing)
            {
                var windowSettings = windowsSettings.Get(window.GetType());
                windowOverlay.Show(window, windowSettings);
            }
        }
    }
}
