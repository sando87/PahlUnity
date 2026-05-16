using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class RenderBurn : RenderBase
    {
        public override float BurnRate { get => mBurnState; set => SetBurnRate(value); }

        float mStartPosY = 0;
        float mBurnState = 0;

        void Awake()
        {
            mStartPosY = transform.localPosition.y;
            mBurnState = 0;
        }

        void SetBurnRate(float burnRate)
        {
            mBurnState = burnRate;

            if (burnRate == 0)
            {
                transform.ExSetLocalPosY(mStartPosY);
                transform.localScale = new Vector3(0, 0, 1);
            }
            else
            {
                float localPosY = mStartPosY * (1 - burnRate);
                localPosY.ExSetMinimum(0);
                transform.ExSetLocalPosY(localPosY);
                float scale = 1 + burnRate;
                scale.ExSetMaximum(2);
                transform.localScale = new Vector3(scale, scale, 1);
            }
        }
    }
}