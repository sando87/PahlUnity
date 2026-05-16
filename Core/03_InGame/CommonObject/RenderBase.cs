using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlUnity
{
    public class RenderBase : MonoBehaviour
    {
        public virtual Color Color { get; set; } = Color.white;
        public virtual float Opacity { get; set; } = 1.0f;
        public virtual float BurnRate { get; set; } = 0.0f;
    }
}