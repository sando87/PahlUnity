using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlUnity
{
    public class RenderSprite : RenderBase
    {
        public override Color Color { get => mSpriteRenderer.color; set => mSpriteRenderer.color = value; }
        public override float Opacity { get => mSpriteRenderer.color.a; set => mSpriteRenderer.ExSetAlpha(value); }
        public override bool FlipX { get => mSpriteRenderer.flipX; set => mSpriteRenderer.flipX = value; }

        SpriteRenderer mSpriteRenderer;

        void Awake()
        {
            mSpriteRenderer = GetComponent<SpriteRenderer>();
        }

    }
}