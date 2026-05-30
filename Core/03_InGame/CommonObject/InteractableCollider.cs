using System;
using UnityEngine;

namespace PahlUnity
{
    public class InteractableCollider : MonoBehaviour
    {
        [SerializeField] InteractMask _MyProperty = InteractMask.Everything;
        [SerializeField] InteractMask _InteractableWith = InteractMask.Everything;

        public InteractMask MyProperty => _MyProperty;
        public bool LockInteract { get; set; } = false;

        public event Action<Collider2D> OnInteractEnter2D;
        public event Action<Collider2D> OnInteractLeave2D;
        public event Action<Collider> OnInteractEnter3D;
        public event Action<Collider> OnInteractLeave3D;
        public event Action<BaseObject, InteractMask> OnInteractSignal;

        private Collider2D mCollider2D = null;
        private Collider mCollider3D = null;

        void Awake()
        {
            mCollider2D = GetComponent<Collider2D>();
            mCollider3D = GetComponent<Collider>();
        }

        #region  2D Collider
        void OnCollisionEnter2D(Collision2D collision)
        {
            if (IsInteractable(collision.collider.GetComponent<InteractableCollider>()))
            {
                OnInteractEnter2D?.Invoke(collision.collider);
            }
        }
        void OnCollisionExit2D(Collision2D collision)
        {
            if (IsInteractable(collision.collider.GetComponent<InteractableCollider>()))
            {
                OnInteractLeave2D?.Invoke(collision.collider);
            }
        }
        void OnTriggerEnter2D(Collider2D collision)
        {
            if (IsInteractable(collision.GetComponent<InteractableCollider>()))
            {
                OnInteractEnter2D?.Invoke(collision);
            }
        }
        void OnTriggerExit2D(Collider2D collision)
        {
            if (IsInteractable(collision.GetComponent<InteractableCollider>()))
            {
                OnInteractLeave2D?.Invoke(collision);
            }
        }
        #endregion

        #region 3D Collider
        void OnCollisionEnter(Collision collision)
        {
            if (IsInteractable(collision.collider.GetComponent<InteractableCollider>()))
            {
                OnInteractEnter3D?.Invoke(collision.collider);
            }
        }
        void OnCollisionExit(Collision collision)
        {
            if (IsInteractable(collision.collider.GetComponent<InteractableCollider>()))
            {
                OnInteractLeave3D?.Invoke(collision.collider);
            }
        }
        void OnTriggerEnter(Collider other)
        {
            if (IsInteractable(other.GetComponent<InteractableCollider>()))
            {
                OnInteractEnter3D?.Invoke(other);
            }
        }
        void OnTriggerExit(Collider other)
        {
            if (IsInteractable(other.GetComponent<InteractableCollider>()))
            {
                OnInteractLeave3D?.Invoke(other);
            }
        }
        #endregion

        private bool IsInteractable(InteractableCollider other)
        {
            if (LockInteract || other == null)
                return false;

            // 콜라이더 이벤트는 콜라이더가 붙어있는 객체에게만 이벤트가 전달 되도록 하기 위함
            if (mCollider2D != null && gameObject != mCollider2D.gameObject)
                return false;

            if (mCollider3D != null && gameObject != mCollider3D.gameObject)
                return false;

            InteractMask mask = _InteractableWith & other._MyProperty;
            return mask != InteractMask.Nothing;
        }
        public void InvokeInteractSignal(BaseObject invoker, InteractMask signal)
        {
            if (LockInteract)
                return;

            // 콜라이더 이벤트는 콜라이더가 붙어있는 객체에게만 이벤트가 전달 되도록 하기 위함
            if (gameObject != mCollider2D.gameObject)
                return;

            InteractMask mask = _InteractableWith & signal;
            if (mask != InteractMask.Nothing)
            {
                OnInteractSignal?.Invoke(invoker, signal);
            }
        }

    }
}