
using CardRooms.Interfaces.Modules;
using System;
using UnityEngine;

namespace CardRooms.Scenario
{
    public class Scenario : MonoBehaviour, IScenario
    {
        private IProfileManager profileManager;

        public void Init(IProfileManager profileManager)
        {
            this.profileManager = profileManager;
        }

        public void Play()
        {
            throw new NotImplementedException();
        }
    }
}