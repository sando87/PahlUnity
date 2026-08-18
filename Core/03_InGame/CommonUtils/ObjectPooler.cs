using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

namespace PahlUnity
{
    public class ObjectPooler : MonoBehaviour
    {
        public void InstantiateVFX(GameObject poolingObj)
        {
            if (poolingObj.IsPrefab())
                ObjectPoolingManager.Instance.PopForDuration(poolingObj, transform.position, transform.rotation, null, 3);
            else
                ObjectPoolingManager.Instance.PopForDuration(poolingObj, poolingObj.transform.position, poolingObj.transform.rotation, null, 3);
        }

        public void InstantiateVFX(Collider target)
        {
            Vector3 pos = target.ExGetBase().Body3D.Center;
            pos.y = 1;
            Quaternion rot = target.ExGetBase().transform.rotation;
            ObjectPoolingManager.Instance.PopForDuration(gameObject, pos, rot, null, 3);
        }
    }
}