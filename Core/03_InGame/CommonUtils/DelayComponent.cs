using UnityEngine;
using UnityEngine.Events;

namespace PahlUnity
{
    public class DelayComponent : MonoBehaviour
    {
        [SerializeField] float _DelaySeconds = 1;
        [SerializeField] bool _StartOnEnable = true;
        [SerializeField] UnityEvent _EventDelayStarted = null;

        void OnEnable()
        {
            if (_StartOnEnable)
                StartDelay();
        }

        public void StartDelay()
        {
            StopAllCoroutines();
            this.ExDelayedCoroutine(_DelaySeconds, _EventDelayStarted.Invoke);
        }

    }
}