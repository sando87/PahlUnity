using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PahlUnity
{
    public class ObjectPoolingManager : SingletonMono<ObjectPoolingManager>
    {
        // 요청한 객체가 부족할때 추가 할당해주는 객체의 개수
        private const int AllocCount = 10;

        // 풀링 객체의 이름을 key로 사용함.
        private Dictionary<string, Transform> mObjectPool = new Dictionary<string, Transform>();

        public GameObject PopForDuration(GameObject poolingObj, Vector3 position, Quaternion rotation, Transform parent, float duration)
        {
            GameObject obj = Pop(poolingObj, position, rotation, parent);
            PushBack(obj, duration);
            return obj;
        }

        // 풀링 대상으로 넣어주는 프리팹은 Asset상태로 존재해도 되고, 씬상에 이미 배치된 객체여도 된다.
        // 단 해당 객체의 이름을 key로 사용하므로 주의..
        public GameObject Pop(GameObject poolingObj, Vector3 position, Quaternion rotation, Transform parent)
        {
            GameObject obj = Pop(poolingObj);
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.transform.SetParent(parent);
            obj.SetActive(true);

            ObjectPoolable poolingComp = obj.GetComponent<ObjectPoolable>();
            if (poolingComp != null)
            {
                poolingComp.PopFromPool();
            }

            return obj;
        }
        private GameObject Pop(GameObject poolingObj)
        {
            // 프리팹이 Asset상태로 있는 경우에는 AssetID를 추가하여 key로 설정하고,
            // 씬 상에 이미 생성된 객체를 풀링할때는 해당 객체의 이름을 key로 사용함.
            string key = poolingObj.name;
            if (poolingObj.IsPrefab())
            {
                key += poolingObj.GetEntityId();
            }

            // 요청한 객체가 할당 가능하지 확인하고 불가능 하면 추가로 객체 생성한다.
            if (!IsAllocable(key))
            {
                if (!AllocObjects(key, poolingObj))
                    return null;
            }

            // Pool에서 실제 요청한 객체 하나를 빼와서 반환해준다
            Transform obj = mObjectPool[key].GetChild(0);
            obj.SetParent(null);
            return obj.gameObject;
        }

        // 요청한 객체가 할당 가능한지 여부 확인
        private bool IsAllocable(string key)
        {
            if (mObjectPool.ContainsKey(key))
            {
                return mObjectPool[key].childCount > 0;
            }
            return false;
        }
        // 요청한 객체가 Pool에 없을경우 10개의 객체를 미리 생성
        private bool AllocObjects(string key, GameObject prefab)
        {
            // 최초 요청시에는 그룹 관리를 위한 부모 객체 생성
            if (!mObjectPool.ContainsKey(key))
            {
                GameObject newParent = new GameObject();
                newParent.transform.SetParent(transform);
                newParent.name = key;
                mObjectPool[key] = newParent.transform;
            }

            // 그룹 즉 부모 객체 하위에 풀링할 실제 객체를 10개 미리 생성해 놓는다
            Transform parentTr = mObjectPool[key];
            for (int i = 0; i < AllocCount; ++i)
            {
                GameObject newObj = Instantiate(prefab, parentTr);
                newObj.name = key;
                newObj.gameObject.SetActive(false);
            }
            return true;
        }

        public void PushBack(GameObject obj)
        {
            LOG.errorif(obj == null, "obj is null");

            // 대상객체가 이미 풀링 안에 있는 상태이면 그냥 반환
            if (IsAlreadyInPoolingGroup(obj))
                return;

            // 다 사용한 객체는 재활용을 위해 다시 Pool에 넣어준다
            string key = obj.name;
            if (mObjectPool.ContainsKey(key))
            {
                ObjectPoolable poolingObj = obj.GetComponent<ObjectPoolable>();
                if (poolingObj != null)
                {
                    poolingObj.PushBackToPool();
                }

                Transform parentTr = mObjectPool[key];
                obj.transform.SetParent(parentTr);
                obj.SetActive(false);
            }
        }

        public void PushBack(GameObject obj, float delay)
        {
            if (delay <= 0)
            {
                PushBack(obj);
            }
            else
            {
                this.ExDelayedCoroutine(delay, () =>
                {
                    PushBack(obj);
                });
            }
        }

        public bool IsAlreadyInPoolingGroup(GameObject obj)
        {
            ObjectPoolingManager poolRoot = obj.GetComponentInParent<ObjectPoolingManager>();
            if (poolRoot == null)
                return false;

            return true;
        }
    }
}