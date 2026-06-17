using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;


namespace PahlUnity
{
    public class FiniteStateBase
    {
        public event Action EventEnter;
        public event Action EventUpdate;
        public event Action EventLeave;

        public virtual void EnterState()
        {
            EventEnter?.Invoke();
        }
        public virtual void UpdateState()
        {
            EventUpdate?.Invoke();
        }
        public virtual void LeaveState()
        {
            EventLeave?.Invoke();
        }
    }
}
