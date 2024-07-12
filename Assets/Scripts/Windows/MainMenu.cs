
using CardRooms.Windows.Elements;
using UnityEngine;

namespace CardRooms.Windows
{
    public class MainMenu : BaseWindowNoInitData
    {
        [SerializeField] private ButtonBase playButton;

        public override void OnLoad()
        {
            base.OnLoad();

            playButton.OnClick += OnPlayButtonClick;
        }

        private void OnPlayButtonClick()
        {
            Root.Root.Scenario.Play();
        }
    }
}
