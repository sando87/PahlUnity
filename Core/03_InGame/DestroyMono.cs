using UnityEngine;
using UnityEngine.Events;

namespace PahlUnity
{
    public class DestroyMono : MonoBehaviour
    {
        [SerializeField] float _DestroyDelay = 0f;

        void OnEnable()
        {
            if (_DestroyDelay > 0)
                this.ExDelayedCoroutine(_DestroyDelay, DoDestroy);
        }

        public void DoDestroy()
        {
            Destroy(gameObject);
        }
        public void DoDestroy(GameObject obj)
        {
            Destroy(obj);
        }
    }
}