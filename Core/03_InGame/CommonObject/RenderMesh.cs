using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlUnity
{
    public class RenderMesh : RenderBase
    {
        public override Color Color { get => mMeshRenderer.material.color; set => mMeshRenderer.material.color = value; }
        public override float Opacity
        {
            get => mMeshRenderer.material.color.a;
            set => mMeshRenderer.material.color = new Color(mMeshRenderer.material.color.r, mMeshRenderer.material.color.g, mMeshRenderer.material.color.b, value);
        }

        MeshRenderer mMeshRenderer;

        void Awake()
        {
            mMeshRenderer = GetComponent<MeshRenderer>();
        }

    }
}