
using System.Collections.Generic;
using System;
using UnityEngine;
using CardRooms.Interfaces;
using CardRooms.DTS;
using CardRooms.Common.Promises;
using CardRooms.DTS.Links;
using CardRooms.Windows.Elements;

namespace CardRooms.Windows
{
    public class WindowOverlay : MonoBehaviour, IWindowOverlay
    {
        [Serializable]
        public struct CurrencyObject
        {
            public LinkToItem linkToItem;
            public WindowsOptionalElement element;
        }

        public event Action OnBackButtonClick = () => { };

        [SerializeField] private CurrencyObject[] currencyObjectsMap;

        public IPromise Show(IWindow window, WindowsSettings.Settings settings)
        {
            List<IPromise> promises = new List<IPromise>();

            foreach (var currencyObject in this.currencyObjectsMap)
            {
                bool visible = settings.IsItemShown(currencyObject.linkToItem);
                promises.Add(currencyObject.element.SetWindowsVisibility(visible));
            }
            return Deferred.All(promises);
        }

        public void Die()
        {
            Destroy(gameObject);
        }
    }
}
