using System.Collections;
using UnityEngine;

namespace PahlUnity
{
    public class ShaderController : MonoBehaviour
    {
        SpriteRenderer mSR;
        MaterialPropertyBlock mMPB;

        void Awake()
        {
            mSR = GetComponent<SpriteRenderer>();
            mMPB = new MaterialPropertyBlock();
        }

        public void SetInt(string name, int value)
        {
            mSR.GetPropertyBlock(mMPB);
            mMPB.SetInt(name, value);
            mSR.SetPropertyBlock(mMPB);
        }
        public void SetFloat(string name, float value)
        {
            mSR.GetPropertyBlock(mMPB);
            mMPB.SetFloat(name, value);
            mSR.SetPropertyBlock(mMPB);
        }
        public void SetColor(string name, Color value)
        {
            mSR.GetPropertyBlock(mMPB);
            mMPB.SetColor(name, value);
            mSR.SetPropertyBlock(mMPB);
        }
        public void SetVector(string name, Vector4 value)
        {
            mSR.GetPropertyBlock(mMPB);
            mMPB.SetVector(name, value);
            mSR.SetPropertyBlock(mMPB);
        }
        public void SetTexture(string name, Texture value)
        {
            mSR.GetPropertyBlock(mMPB);
            mMPB.SetTexture(name, value);
            mSR.SetPropertyBlock(mMPB);
        }
    }
}
