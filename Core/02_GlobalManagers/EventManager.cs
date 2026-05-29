using System;
using System.Collections.Generic;
using UnityEngine;


namespace PahlUnity
{
    [RequireComponent(typeof(EventBus))]
    public class EventManager : SingletonMono<EventManager>
    {
        public EventBus GlobalEvents { get; private set; } = null;

        protected override void Awake()
        {
            base.Awake();

            GlobalEvents = GetComponent<EventBus>();
            if (GlobalEvents == null)
            {
                GlobalEvents = gameObject.AddComponent<EventBus>();
            }
        }
    }
}
