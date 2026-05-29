using System;
using System.Collections.Generic;
using UnityEngine;


namespace PahlUnity
{
    public class EventManager : SingletonMono<EventManager>
    {
        public EventBus GlobalEvents { get; private set; } = null;

        protected override void Awake()
        {
            base.Awake();

            GlobalEvents = GetComponent<EventBus>();
        }
    }
}
