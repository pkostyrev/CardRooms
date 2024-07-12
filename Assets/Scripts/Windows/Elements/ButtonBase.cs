
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CardRooms.Windows.Elements
{
    public class ButtonBase : ElementBase
    {
        public event Action OnClick = () => { };

        [SerializeField] private Button button;

        public void Awake()
        {
            button.onClick.AddListener(() => OnClick());
        }

        public void SetInteractable(bool interactable)
        {
            button.interactable = interactable;
        }
    }
}
