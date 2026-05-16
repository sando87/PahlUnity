using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class RenderSprite : RenderBase
    {
        public override Color Color { get => mSpriteRenderer.color; set => mSpriteRenderer.color = value; }

        SpriteRenderer mSpriteRenderer;

        void Awake()
        {
            mSpriteRenderer = GetComponent<SpriteRenderer>();
        }

    }
}