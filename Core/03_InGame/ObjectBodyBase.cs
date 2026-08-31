using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace PahlUnity
{
    public class ObjectBodyBase : MonoBehaviour
    {
        public virtual Vector3 Center { get; }
        public virtual Vector3 Size { get; }
        public virtual Vector3 Foot { get; }
        public virtual Vector3 Head { get; }
        public virtual Vector3 Front { get; }
        public virtual Vector3 Back { get; }

        public virtual Vector3 FootFront { get; }
        public virtual Vector3 FootBack { get; }

        public virtual bool LockBody { get; set; }
    }
}