using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlBit
{
    struct ColorEntry
    {
        public int Token;
        public Color Color;
        public Coroutine Coroutine;
    }

    public class RenderController : MonoBehaviour
    {
        List<RenderBase> mRenderList = new();

        List<ColorEntry> mColorList = new(16);

        void Awake()
        {
            mRenderList.AddRange(GetComponentsInChildren<RenderBase>());
        }

        public Color GetColor()
        {
            return mColorList.Count > 0 ? mColorList[^1].Color : Color.white;
        }
        public void SetColor(int tokenID, Color color)
        {
            RemoveColor(tokenID);
            ColorEntry entry = new ColorEntry();
            entry.Token = tokenID;
            entry.Color = color;
            mColorList.Add(entry);
            ApplyColor();
        }
        public void SetColor(int tokenID, Color color, float duration)
        {
            RemoveColor(tokenID);
            ColorEntry entry = new ColorEntry();
            entry.Token = tokenID;
            entry.Color = color;
            entry.Coroutine = this.ExDelayedCoroutine(duration, () => UnSetColor(tokenID));
            mColorList.Add(entry);
            ApplyColor();

        }
        public void SetColor(Color color, float duration)
        {
            ColorEntry entry = new ColorEntry();
            entry.Token = 0;
            entry.Color = color;
            entry.Coroutine = this.ExDelayedCoroutine(duration, () =>
            {
                RemoveColor(entry.Coroutine);
                ApplyColor();
            });
            mColorList.Add(entry);
            ApplyColor();
        }
        public void UnSetColor(int tokenID)
        {
            RemoveColor(tokenID);
            ApplyColor();
        }
        void RemoveColor(int tokenID)
        {
            for (int i = mColorList.Count - 1; i >= 0; --i)
            {
                if (mColorList[i].Token == tokenID)
                {
                    if (mColorList[i].Coroutine != null)
                        StopCoroutine(mColorList[i].Coroutine);

                    mColorList.RemoveAt(i);
                    break;
                }
            }
        }
        void RemoveColor(Coroutine _coroutine)
        {
            for (int i = mColorList.Count - 1; i >= 0; --i)
            {
                if (mColorList[i].Coroutine == _coroutine)
                {
                    StopCoroutine(mColorList[i].Coroutine);
                    mColorList.RemoveAt(i);
                    break;
                }
            }
        }
        void ApplyColor()
        {
            Color color = GetColor();
            foreach (RenderBase renderObj in mRenderList)
            {
                renderObj.Color = color;
            }
        }


        public void SetOpacity(float opacity)
        {
            foreach (RenderBase renderObj in mRenderList)
            {
                renderObj.Opacity = opacity;
            }
        }

        public void SetBurnRate(float burnRate)
        {
            foreach (RenderBase renderObj in mRenderList)
            {
                renderObj.BurnRate = burnRate;
            }
        }
    }
}