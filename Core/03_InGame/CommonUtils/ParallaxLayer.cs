using UnityEngine;

namespace PahlUnity
{
    public class ParallaxLayer : MonoBehaviour
    {
        [SerializeField] float parallaxFactor = 0.5f;

        Transform cam;
        Vector3 prevCamPos;

        void Start()
        {
            cam = Camera.main.transform;
            prevCamPos = cam.position;
        }

        void LateUpdate()
        {
            Vector3 delta = cam.position - prevCamPos;
            transform.position -= new Vector3(delta.x * parallaxFactor, delta.y * parallaxFactor, 0);
            prevCamPos = cam.position;
        }
    }
}
