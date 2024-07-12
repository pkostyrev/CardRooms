
using CardRooms.DTS;
using CardRooms.Interfaces.Modules;
using CardRooms.Common.StatefulEvent;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CardRooms.EventSystemControl
{
    public class EventSystemControl : MonoBehaviour, IEventSystemControl
    {
        public IStatefulEvent<bool> HasSomeUserInput { get { return hasSomeUserInput; } }

        public IStatefulEvent<bool> BlockedState { get { return blockedState; } }

        private StatefulEventInt<bool> hasSomeUserInput = StatefulEventInt.Create(false);
        private StatefulEventInt<bool> blockedState = StatefulEventInt.Create(false);

        private Dictionary<EventSystemClient, bool> clients = new Dictionary<EventSystemClient, bool>(new ClientComparer());

        [SerializeField] private EventSystem eventSystem;

#if UNITY_EDITOR
        [SerializeField] private bool debugPrint;
#endif

        public void Init() { }

        private void Awake()
        {
            foreach (EventSystemClient client in Enum.GetValues(typeof(EventSystemClient)))
            {
                clients.Add(client, false);
            }

            blockedState.OnValueChanged += BlockedState_OnChanged;
        }

        private void Update()
        {
            bool hasSomeUserInput = Input.touchCount > 0 || Input.GetMouseButton(0) == true || Input.anyKey == true;

            this.hasSomeUserInput.Set(hasSomeUserInput);
        }

#if UNITY_EDITOR
        private void LateUpdate()
        {
            if (debugPrint == true)
            {
                debugPrint = false;
                Debug.Log(this.ToString());
            }
        }
#endif

        private void BlockedState_OnChanged(bool blocked)
        {
            if (eventSystem == null)
            {
                Debug.LogWarning("eventSystem is null but expected to be existing instance, check stack trace and fix this error");
            }
            else
            {
                eventSystem.gameObject.SetActive(blocked == false);
                if (blocked == true)
                {
                    hasSomeUserInput.Set(false);
                }
            }
        }

        public void Block(EventSystemClient client)
        {
            clients[client] = true;
            blockedState.Set(true);
        }

        public void Release(EventSystemClient client)
        {
            clients[client] = false;
            blockedState.Set(IsBlocked());
        }

        public bool IsBlocked()
        {
            foreach (bool blockState in clients.Values)
            {
                if (blockState == true)
                {
                    return true;
                }
            }

            return false;
        }

        public override string ToString()
        {
            string result = "EventSystemControl.clients: ";

            int counter = clients.Count;

            foreach (KeyValuePair<EventSystemClient, bool> pair in clients)
            {
                counter--;
                result += string.Format("{0}:{1}{2}", pair.Key, pair.Value, (counter > 0 ? "; " : ""));
            }

            return result;
        }

        public void RaycastAll(PointerEventData pointerEventData, List<RaycastResult> results) => eventSystem.RaycastAll(pointerEventData, results);

        private class ClientComparer : IEqualityComparer<EventSystemClient>
        {
            public bool Equals(EventSystemClient x, EventSystemClient y)
            {
                return x == y;
            }

            public int GetHashCode(EventSystemClient obj)
            {
                return (int)obj;
            }
        }
    }
}
