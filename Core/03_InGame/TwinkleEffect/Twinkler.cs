using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PahlUnity
{
    public class Twinkler : MonoBehaviour
    {
        [SerializeField] ShaderController _ShaderCtrl = null;

        bool mIsTweening = false;

        public void StartTwinkle()
        {
            HitFlash();
        }

        public void HitFlash()
        {
            if (!mIsTweening)
                StartCoroutine(Flash());
        }

        IEnumerator Flash()
        {
            mIsTweening = true;
            _ShaderCtrl.SetFloat("_FlashAmount", 1);
            yield return new WaitForSeconds(0.05f);

            _ShaderCtrl.SetFloat("_FlashAmount", 0);
            yield return new WaitForSeconds(0.05f);
            mIsTweening = false;
        }

    }
}
