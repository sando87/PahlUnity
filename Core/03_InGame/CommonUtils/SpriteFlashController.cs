using System.Collections;
using UnityEngine;

namespace PahlUnity
{
    public class SpriteFlashController : MonoBehaviour
    {
        SpriteRenderer mSR;
        MaterialPropertyBlock mMPB;

        int mIsFlashingState = 0;

        void Awake()
        {
            mSR = GetComponent<SpriteRenderer>();
            mMPB = new MaterialPropertyBlock();
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
                mSR.GetPropertyBlock(mMPB);
                mMPB.SetFloat("_FlashAmount", 1);
                mSR.SetPropertyBlock(mMPB);
                yield return new WaitForSeconds(0.05f);

                mMPB.SetFloat("_FlashAmount", 0);
                mSR.SetPropertyBlock(mMPB);
                yield return new WaitForSeconds(0.05f);

                if (mIsFlashingState == 1)
                    break;
            }
            mIsFlashingState = 0;
        }
    }
}
