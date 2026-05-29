using UnityEngine;


namespace PahlUnity
{
    public class BaseObject : MonoBehaviour
    {
        void Awake()
        {
        }

        void Start()
        {

        }

        public T GetComp<T>() where T : MonoBehaviour
        {
            return GetComponentInChildren<T>();
        }
        public bool HasComp<T>() where T : MonoBehaviour
        {
            return GetComponentInChildren<T>() != null;
        }
        public bool TryGetComp<T>(out T component) where T : MonoBehaviour
        {
            component = GetComponentInChildren<T>();
            return component != null;
        }

        public void DestroyObj()
        {
            Destroy(gameObject);
        }
        public void DestroyObj(float delay)
        {
            Destroy(gameObject, delay);
        }
    }
}
