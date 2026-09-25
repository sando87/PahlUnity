using System;
using UnityEngine;

namespace PahlUnity
{
    public class FiniteStateBase : MonoBehaviour
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
