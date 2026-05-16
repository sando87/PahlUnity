using System.Collections;
using UnityEngine;

namespace PahlBit
{
    public class SpriteFlashController : MonoBehaviour
    {
        SpriteRenderer sr;
        MaterialPropertyBlock mpb;

        int mIsFlashingState = 0;

        void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            mpb = new MaterialPropertyBlock();
        }

        public void HitFlash()
        {
            if (mIsFlashingState == 0)
                StartCoroutine(Flash());
            else
                mIsFlashingState = 2;
        }

        IEnumerator Flash()
        {
            while (true)
            {
                mIsFlashingState = 1;
                sr.GetPropertyBlock(mpb);
                mpb.SetFloat("_FlashAmount", 1);
                sr.SetPropertyBlock(mpb);
                yield return new WaitForSeconds(0.05f);

                mpb.SetFloat("_FlashAmount", 0);
                sr.SetPropertyBlock(mpb);
                yield return new WaitForSeconds(0.05f);

                if (mIsFlashingState == 1)
                    break;
            }
            mIsFlashingState = 0;
        }
    }
}
