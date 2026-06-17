using System;
using UnityEngine;

namespace PahlUnity
{
    public class InteractableCollider : MonoBehaviour
    {
        [SerializeField, InteractMaskSelector] uint _MyProperty = 0;
        [SerializeField, InteractMaskSelector] uint _InteractableWith = 0;

        public uint MyProperty => _MyProperty;
        public bool LockInteract { get; set; } = false;

        public event Action<Collider2D> OnInteractEnter2D;
        public event Action<Collider2D> OnInteractLeave2D;
        public event Action<Collider> OnInteractEnter3D;
        public event Action<Collider> OnInteractLeave3D;
        public event Action<BaseObject, uint> OnInteractSignal;

        private ColliderDetector2D mCollider2D = null;
        private ColliderDetector3D mCollider3D = null;

        void Awake()
        {
            mCollider2D = GetComponent<ColliderDetector2D>();
            if (mCollider2D != null)
            {
                mCollider2D.OnDetectEnter += OnDetectEnter2D;
                mCollider2D.OnDetectExit += OnDetectLeave2D;
            }

            mCollider3D = GetComponent<ColliderDetector3D>();
            if (mCollider3D != null)
            {
                mCollider3D.OnDetectEnter += OnDetectEnter3D;
                mCollider3D.OnDetectExit += OnDetectLeave3D;
            }
        }

        public void SetTargetLayerMask(LayerMask targetLayerMask)
        {
            if (mCollider2D != null)
            {
                mCollider2D.SetTargetLayerMask(targetLayerMask);
            }
            if (mCollider3D != null)
            {
                mCollider3D.SetTargetLayerMask(targetLayerMask);
            }
        }

        private void OnDetectEnter2D(Collider2D col)
        {
            if (IsInteractable(col.GetComponent<InteractableCollider>()))
            {
                OnInteractEnter2D?.Invoke(col);
            }
        }
        private void OnDetectLeave2D(Collider2D col)
        {
            if (IsInteractable(col.GetComponent<InteractableCollider>()))
            {
                OnInteractLeave2D?.Invoke(col);
            }
        }
        private void OnDetectEnter3D(Collider col)
        {
            if (IsInteractable(col.GetComponent<InteractableCollider>()))
            {
                OnInteractEnter3D?.Invoke(col);
            }
        }
        private void OnDetectLeave3D(Collider col)
        {
            if (IsInteractable(col.GetComponent<InteractableCollider>()))
            {
                OnInteractLeave3D?.Invoke(col);
            }
        }

        private bool IsInteractable(InteractableCollider other)
        {
            if (LockInteract || other == null)
                return false;

            // 콜라이더 이벤트는 콜라이더가 붙어있는 객체에게만 이벤트가 전달 되도록 하기 위함
            if (mCollider2D != null && gameObject != mCollider2D.gameObject)
                return false;

            if (mCollider3D != null && gameObject != mCollider3D.gameObject)
                return false;

            uint mask = _InteractableWith & other._MyProperty;
            return mask != 0;
        }
        public void InvokeInteractSignal(BaseObject invoker, uint signal)
        {
            if (LockInteract)
                return;

            // 콜라이더 이벤트는 콜라이더가 붙어있는 객체에게만 이벤트가 전달 되도록 하기 위함
            if (gameObject != mCollider2D.gameObject)
                return;

            uint mask = _InteractableWith & signal;
            if (mask != 0)
            {
                OnInteractSignal?.Invoke(invoker, signal);
            }
        }

    }
}