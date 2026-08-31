using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlUnity
{
    struct ColorEntry
    {
        public string Token;
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

        public bool GetShowState()
        {
            return mRenderList.Count > 0 ? mRenderList[^1].IsShow : false;
        }
        public void SetShowState(bool isShow)
        {
            foreach (RenderBase renderObj in mRenderList)
            {
                renderObj.IsShow = isShow;
            }
        }

        public Color GetColor()
        {
            return mColorList.Count > 0 ? mColorList[^1].Color : Color.white;
        }
        public void SetColor(string tokenID, Color color)
        {
            RemoveColor(tokenID);
            ColorEntry entry = new ColorEntry();
            entry.Token = tokenID;
            entry.Color = color;
            mColorList.Add(entry);
            ApplyColor();
        }
        public void SetColor(string tokenID, Color color, float duration)
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
            entry.Token = "";
            entry.Color = color;
            entry.Coroutine = this.ExDelayedCoroutine(duration, () =>
            {
                RemoveColor(entry.Coroutine);
                ApplyColor();
            });
            mColorList.Add(entry);
            ApplyColor();
        }
        public void UnSetColor(string tokenID)
        {
            RemoveColor(tokenID);
            ApplyColor();
        }
        void RemoveColor(string tokenID)
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

        public void SetFlipX(bool flipX)
        {
            foreach (RenderBase renderObj in mRenderList)
            {
                renderObj.FlipX = flipX;
            }
        }

        public void SetEmission(float emission)
        {
            foreach (RenderBase renderObj in mRenderList)
            {
                renderObj.Emission = emission;
            }
        }

        public void DoTwinkle(float duration)
        {
            SetEmission(0.5f);
            this.ExDelayedCoroutine(duration, () =>
            {
                SetEmission(0);
            });
        }

        public void DoFlicker(float duraion, float interval = 0.2f)
        {
            StartCoroutine(CoFlicker(duraion, interval));
        }

        IEnumerator CoFlicker(float duraion, float interval)
        {
            float time = 0;
            bool isShow = false;
            while (time < duraion)
            {
                SetShowState(isShow);
                yield return newWaitForSeconds.Cache(interval);
                time += interval;
                isShow = !isShow;
            }
            SetShowState(true);
        }
    }
}