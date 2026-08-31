using System;
using DG.Tweening;
using UnityEngine;

namespace PahlUnity
{
    public class ObjectPhysicsBase : MonoBehaviour
    {
        public virtual Vector3 Velocity { get; set; }
        public virtual bool LockGravity { get; set; }
        public virtual bool LockMovement { get; set; }
    }
}