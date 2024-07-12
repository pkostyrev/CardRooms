
using CardRooms.Interfaces.Modules;
using System.Collections;
using System.Net;
using UnityEngine;

namespace CardRooms.Root
{
    public class Root : MonoBehaviour
    {
        [SerializeField] private Factory.Factory factory;

        private static Root instance;

        private void Awake()
        {
            Application.targetFrameRate = 60;

            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }


        private IEnumerator Start()
        {
            var foo1a = Root.EventSystemControl;

            yield return null;

            yield return ConfigLoader.Load();

            yield return null; var foo2 = Root.CameraController;
            yield return null; Root.SaveGameManager.LoadGame();

            yield return null;

            yield return null; Root.Windows.ShowMainMenu();
            yield return null; Root.SplashScreen.Hide();
        }

        internal static void Die()
        {
            if (instance != null)
            {
                DestroyImmediate(instance.gameObject);
            }

            instance = null;
        }

        public static void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private ICacheManager cacheManager;
        public static ICacheManager CacheManager
        {
            get
            {
                if (instance.cacheManager == null)
                {
                    instance.cacheManager = instance.factory.CreateCacheManager();
                    instance.cacheManager.Init();
                }
                return instance.cacheManager;
            }
        }

        private ICameraController cameraController;
        public static ICameraController CameraController
        {
            get
            {
                if (instance.cameraController == null)
                {
                    instance.cameraController = instance.factory.CreateCameraController();
                    instance.cameraController.Init(UIManager);
                }
                return instance.cameraController;
            }
        }

        private IConfigManager configManager;
        public static IConfigManager ConfigManager
        {
            get
            {
                if (instance.configManager == null)
                {
                    instance.configManager = instance.factory.CreateConfigManager();
                    instance.configManager.Init();
                }
                return instance.configManager;
            }
        }

        private IEventSystemControl eventSystemControl;
        public static IEventSystemControl EventSystemControl
        {
            get
            {
                if (instance.eventSystemControl == null)
                {
                    instance.eventSystemControl = instance.factory.CreateEventSystemControl();
                    instance.eventSystemControl.Init();
                }
                return instance.eventSystemControl;
            }
        }

        private IProfileManager profileManager;
        public static IProfileManager ProfileManager
        {
            get
            {
                if (instance.scenario == null)
                {
                    instance.profileManager = instance.factory.CreateProfileManager();
                    instance.profileManager.Init(ConfigManager);
                }
                return instance.profileManager;
            }
        }

        private ISaveGameManager saveGameManager;
        public static ISaveGameManager SaveGameManager
        {
            get
            {
                if (instance.saveGameManager == null)
                {
                    instance.saveGameManager = instance.factory.CreateSaveGameManager();
                    instance.saveGameManager.Init(CacheManager, ConfigManager, ProfileManager);
                }
                return instance.saveGameManager;
            }
        }

        private IScenario scenario;
        public static IScenario Scenario
        {
            get
            {
                if (instance.scenario == null)
                {
                    instance.scenario = instance.factory.CreateScenario();
                    instance.scenario.Init(ProfileManager);
                }
                return instance.scenario;
            }
        }

        private ISplashScreen splashScreen;
        public static ISplashScreen SplashScreen
        {
            get
            {
                if (instance.splashScreen == null)
                {
                    instance.splashScreen = instance.factory.CreateSplashScreen();
                    instance.splashScreen.Init(ConfigManager);
                }
                return instance.splashScreen;
            }
        }

        private IUIManager uiManager;
        public static IUIManager UIManager
        {
            get
            {
                if (instance.uiManager == null)
                {
                    instance.uiManager = instance.factory.CreateUIManager();
                    instance.uiManager.Init(EventSystemControl, ConfigManager);
                }
                return instance.uiManager;
            }
        }

        private IWindows windows;
        public static IWindows Windows
        {
            get
            {
                if (instance.windows == null)
                {
                    instance.windows = instance.factory.CreateWindows();
                    instance.windows.Init();
                }
                return instance.windows;
            }
        }

#if UNITY_EDITOR
        private static IConfigManager configManagerEditor;
        public static IConfigManager ConfigManagerEditor
        {
            get
            {
                if (configManagerEditor == null)
                {
                    configManagerEditor = FindObjectOfType<Factory.Factory>().CreateConfigManager();
                    (configManagerEditor as MonoBehaviour).gameObject.hideFlags = HideFlags.DontSave;
                    configManagerEditor.InitEditor();
                }
                return configManagerEditor;
            }
        }
#endif
    }
}
