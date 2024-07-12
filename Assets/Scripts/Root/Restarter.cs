
using CardRooms.Common.Promises;
using System;
using UnityEngine;

namespace CardRooms.Root
{
    public class Restarter : MonoBehaviour
    {
        [SerializeField] private float maxDeviceSleepSeconds;

        private DateTime? restartTime;

        private static bool restartInProgress = false;

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                restartTime = DateTime.UtcNow.AddSeconds(maxDeviceSleepSeconds);
            }
            else
            {
                if (restartTime.HasValue && DateTime.UtcNow > restartTime.Value)
                {
                    RestartApplication();
                }

                restartTime = null;
            }
        }

        public static void RestartApplication()
        {
            if (restartInProgress == true)
            {
                return;
            }

            restartInProgress = true;

            GameObject oldSplashScreenInstance = Root.SplashScreen.ShowImmediatelyAndDetachFromRoot();
            DontDestroyOnLoad(oldSplashScreenInstance);

            Timers.Instance.WaitOneFrame()
                .Then(() =>
                {
                    return LoadSceneAsync("Loading");
                })
                .Done(() =>
                {
                    Root.Die();
                })
                .Then(() =>
                {
                    return LoadSceneAsync("Main");
                })
                .Always(() =>
                {
                    DestroyImmediate(oldSplashScreenInstance);
                    restartInProgress = false;
                });

            IPromise LoadSceneAsync(string sceneName)
            {
                AsyncOperation op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);

                if (op.isDone == true)
                {
                    return Deferred.Resolved();
                }

                Deferred deferred = Deferred.GetFromPool();
                op.completed += _ => { deferred.Resolve(); };
                return deferred;
            }
        }
    }
}
