using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PahlUnity
{
    [RequireComponent(typeof(Collider2D))]
    public class ColliderDetector2D : MonoBehaviour
    {
        public enum DetectionMethod
        {
            UnityTrigger,
            CustomOverlap,
        }

        [SerializeField] private DetectionMethod _DetectionMethod = DetectionMethod.CustomOverlap;
        [SerializeField] private LayerMask _TargetLayerMask = 0;

        public event Action<Collider2D> OnDetectEnter;
        public event Action<Collider2D> OnDetectExit;

        Collider2D mCollider = null;
        ContactFilter2D mContactFilter;
        Collider2D[] mResults = new Collider2D[32];
        Coroutine mDetectCoroutine = null;
        readonly WaitForFixedUpdate mWaitForFixedUpdate = new WaitForFixedUpdate();
        readonly HashSet<Collider2D> mDetectedColliders = new HashSet<Collider2D>();
        readonly HashSet<Collider2D> mCurrentColliders = new HashSet<Collider2D>();

        void Awake()
        {
            mCollider = GetComponent<Collider2D>();
            RefreshContactFilter();
        }

        void OnValidate()
        {
            RefreshContactFilter();
        }

        void OnEnable()
        {
            StartDetectCoroutine();
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            if (_DetectionMethod != DetectionMethod.UnityTrigger)
                return;

            if (!IsDetectable(col))
                return;

            if (mDetectedColliders.Add(col))
            {
                OnDetectEnter?.Invoke(col);
            }
        }

        void OnTriggerExit2D(Collider2D col)
        {
            if (_DetectionMethod != DetectionMethod.UnityTrigger)
                return;

            if (mDetectedColliders.Remove(col))
            {
                OnDetectExit?.Invoke(col);
            }
        }

        void OnDisable()
        {
            StopDetectCoroutine();
            ClearDetectedColliders(true);
        }

        IEnumerator DetectCoroutine()
        {
            while (true)
            {
                yield return mWaitForFixedUpdate;
                DetectColliders();
            }
        }

        void DetectColliders()
        {
            if (mCollider == null)
                return;

            mCurrentColliders.Clear();

            int count = OverlapCollider();
            for (int i = 0; i < count; ++i)
            {
                Collider2D col = mResults[i];
                if (col == null || col == mCollider)
                    continue;

                mCurrentColliders.Add(col);

                if (mDetectedColliders.Add(col))
                {
                    OnDetectEnter?.Invoke(col);
                }
            }

            foreach (Collider2D col in mDetectedColliders)
            {
                if (!mCurrentColliders.Contains(col))
                {
                    OnDetectExit?.Invoke(col);
                }
            }

            mDetectedColliders.Clear();
            foreach (Collider2D col in mCurrentColliders)
            {
                mDetectedColliders.Add(col);
            }
        }

        int OverlapCollider()
        {
            int count = mCollider.Overlap(mContactFilter, mResults);
            while (count == mResults.Length)
            {
                Array.Resize(ref mResults, mResults.Length * 2);
                count = mCollider.Overlap(mContactFilter, mResults);
            }

            return count;
        }

        public void SetTargetLayerMask(LayerMask targetlayerMask)
        {
            if (_TargetLayerMask == targetlayerMask)
                return;

            ClearDetectedColliders(true);
            _TargetLayerMask = targetlayerMask;
            RefreshContactFilter();
        }

        bool IsDetectable(Collider2D col)
        {
            if (col == null || col == mCollider)
                return false;

            int layerMask = 1 << col.gameObject.layer;
            return (_TargetLayerMask.value & layerMask) != 0;
        }

        void RefreshContactFilter()
        {
            mContactFilter = new ContactFilter2D();
            mContactFilter.useLayerMask = true;
            mContactFilter.layerMask = _TargetLayerMask;
            mContactFilter.useTriggers = true;
        }

        void StartDetectCoroutine()
        {
            if (_DetectionMethod != DetectionMethod.CustomOverlap)
                return;

            if (mDetectCoroutine != null)
                return;

            mDetectCoroutine = StartCoroutine(DetectCoroutine());
        }

        void StopDetectCoroutine()
        {
            if (mDetectCoroutine == null)
                return;

            StopCoroutine(mDetectCoroutine);
            mDetectCoroutine = null;
        }

        void ClearDetectedColliders(bool invokeExitEvent)
        {
            if (invokeExitEvent)
            {
                foreach (Collider2D col in mDetectedColliders)
                {
                    if (col != null)
                    {
                        OnDetectExit?.Invoke(col);
                    }
                }
            }

            mDetectedColliders.Clear();
            mCurrentColliders.Clear();
        }
    }
}