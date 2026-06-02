using UnityEngine;

namespace PahlUnity.Demo
{
    public class ParallaxLayer : MonoBehaviour
    {
        [SerializeField] float _ParallaxFactor = 0.5f;

        Transform mCam;
        Vector3 mPrevCamPos;

        void Start()
        {
            mCam = Camera.main.transform;
            mPrevCamPos = mCam.position;
        }

        void LateUpdate()
        {
            Vector3 delta = mCam.position - mPrevCamPos;
            transform.position -= new Vector3(delta.x * _ParallaxFactor, delta.y * _ParallaxFactor, 0);
            mPrevCamPos = mCam.position;
        }
    }
}
