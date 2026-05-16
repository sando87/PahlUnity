using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

namespace PahlUnity
{
    public class Instantiator : MonoBehaviour
    {
        public void InstantiateVFX(Transform _transform)
        {
            // ObjectPooling.Instance.Instantiate(gameObject, _transform.position, _transform.rotation).ReturnAfter(3);
        }

        public void InstantiateVFX(BaseObject _baseObj)
        {
            // ObjectPooling.Instance.Instantiate(gameObject, _baseObj.Body.Center, _baseObj.transform.rotation).ReturnAfter(3);
        }

        public void LogTest()
        {
            LOG.trace();
        }
    }
}