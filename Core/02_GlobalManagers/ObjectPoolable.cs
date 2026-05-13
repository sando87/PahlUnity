using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PahlUnity
{
    public class ObjectPoolable : MonoBehaviour
    {
        IObjectPoolable[] mPoolables;

        void Awake()
        {
            mPoolables = GetComponentsInChildren<IObjectPoolable>();
        }

        public void PopFromPool()
        {
            foreach (IObjectPoolable poolable in mPoolables)
            {
                poolable.OnPopFromPool();
            }
        }
        public void PushBackToPool()
        {
            foreach (IObjectPoolable poolable in mPoolables)
            {
                poolable.OnPushBackToPool();
            }
        }
    }

    public interface IObjectPoolable
    {
        void OnPopFromPool();
        void OnPushBackToPool();
    }
}