
using System.Collections;
using System.IO;
using UnityEngine;

namespace CardRooms.Root
{
    internal static class ConfigLoader
    {
        internal static IEnumerator Load()
        {
            bool configLoaded = false;

            LoadConfig();

            while (configLoaded == false)
            {
                yield return null;
            }

            void LoadConfig()
            {
                Root.ConfigManager.LoadConfigs()
                   .Done(() =>
                   {
                       configLoaded = true;
                   })
                   .Fail(ex =>
                   {
                       Debug.LogError(ex.Message);
                   });
            }
        }
    }
}
