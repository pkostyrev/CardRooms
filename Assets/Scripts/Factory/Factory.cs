using CardRooms.Interfaces.Modules;
using System;
using UnityEngine;

namespace CardRooms.Factory
{
    public class Factory : MonoBehaviour
    {
        [SerializeField] private Transform instancesParent;

        [SerializeField] private GameObject cacheManager;
        [SerializeField] private GameObject cameraController;
        [SerializeField] private GameObject configManager;
        [SerializeField] private GameObject eventSystemControl;
        [SerializeField] private GameObject profileManager;
        [SerializeField] private GameObject saveGameManager;
        [SerializeField] private GameObject scenario;
        [SerializeField] private GameObject splashScreen;
        [SerializeField] private GameObject uiManager;
        [SerializeField] private GameObject windows;

        public ICacheManager CreateCacheManager() => Instantiate(cacheManager, instancesParent).GetComponent<ICacheManager>();
        public ICameraController CreateCameraController() => Instantiate(cameraController, instancesParent).GetComponent<ICameraController>();
        public IConfigManager CreateConfigManager() => Instantiate(configManager, instancesParent).GetComponent<IConfigManager>();
        public IEventSystemControl CreateEventSystemControl() => Instantiate(eventSystemControl, instancesParent).GetComponent<IEventSystemControl>();
        public IProfileManager CreateProfileManager() => Instantiate(profileManager, instancesParent).GetComponent<IProfileManager>();
        public ISaveGameManager CreateSaveGameManager() => Instantiate(saveGameManager, instancesParent).GetComponent<ISaveGameManager>();
        public IScenario CreateScenario() => Instantiate(scenario, instancesParent).GetComponent<IScenario>();
        public ISplashScreen CreateSplashScreen() => Instantiate(splashScreen, instancesParent).GetComponent<ISplashScreen>();
        public IUIManager CreateUIManager() => Instantiate(uiManager, instancesParent).GetComponent<IUIManager>();
        public IWindows CreateWindows() => Instantiate(windows, instancesParent).GetComponent<IWindows>();
    }
}
