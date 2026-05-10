using UnityEngine;

namespace PahlUnity
{
    public class BaseObject : MonoBehaviour
    {
        void Awake()
        {
            transform.position = new Vector3(0, 0, 0);
        }

        void Update()
        {

        }

        public void DoTest()
        {
            transform.position = new Vector3(0, 0, 0);
            gameObject.SetActive(true);
        }

    }
}
