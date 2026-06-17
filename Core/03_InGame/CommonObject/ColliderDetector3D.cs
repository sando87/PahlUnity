using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PahlUnity
{
    [RequireComponent(typeof(Collider))]
    public class ColliderDetector3D : MonoBehaviour
    {
        public enum DetectionMethod
        {
            UnityTrigger,
            CustomOverlap,
        }

        [SerializeField] private DetectionMethod _DetectionMethod = DetectionMethod.CustomOverlap;
        [SerializeField] private LayerMask _TargetLayerMask = 0;

        public event Action<Collider> OnDetectEnter;
        public event Action<Collider> OnDetectExit;

        Collider mCollider = null;
        Collider[] mResults = new Collider[32];
        Coroutine mDetectCoroutine = null;
        readonly WaitForFixedUpdate mWaitForFixedUpdate = new WaitForFixedUpdate();
        readonly HashSet<Collider> mDetectedColliders = new HashSet<Collider>();
        readonly HashSet<Collider> mCurrentColliders = new HashSet<Collider>();

        void Awake()
        {
            mCollider = GetComponent<Collider>();
        }

        void OnEnable()
        {
            StartDetectCoroutine();
        }

        void OnTriggerEnter(Collider col)
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

        void OnTriggerExit(Collider col)
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
                Collider col = mResults[i];
                if (col == null || col == mCollider)
                    continue;

                mCurrentColliders.Add(col);

                if (mDetectedColliders.Add(col))
                {
                    OnDetectEnter?.Invoke(col);
                }
            }

            foreach (Collider col in mDetectedColliders)
            {
                if (!mCurrentColliders.Contains(col))
                {
                    OnDetectExit?.Invoke(col);
                }
            }

            mDetectedColliders.Clear();
            foreach (Collider col in mCurrentColliders)
            {
                mDetectedColliders.Add(col);
            }
        }

        int OverlapCollider()
        {
            int count = OverlapColliderNonAlloc();
            while (count == mResults.Length)
            {
                Array.Resize(ref mResults, mResults.Length * 2);
                count = OverlapColliderNonAlloc();
            }

            return count;
        }

        int OverlapColliderNonAlloc()
        {
            if (mCollider is BoxCollider boxCollider)
                return OverlapBox(boxCollider);

            if (mCollider is SphereCollider sphereCollider)
                return OverlapSphere(sphereCollider);

            if (mCollider is CapsuleCollider capsuleCollider)
                return OverlapCapsule(capsuleCollider);

            Bounds bounds = mCollider.bounds;
            return Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents, mResults, Quaternion.identity, _TargetLayerMask, QueryTriggerInteraction.Collide);
        }

        int OverlapBox(BoxCollider boxCollider)
        {
            Transform colTransform = boxCollider.transform;
            Vector3 center = colTransform.TransformPoint(boxCollider.center);
            Vector3 halfExtents = Vector3.Scale(boxCollider.size, Abs(colTransform.lossyScale)) * 0.5f;
            return Physics.OverlapBoxNonAlloc(center, halfExtents, mResults, colTransform.rotation, _TargetLayerMask, QueryTriggerInteraction.Collide);
        }

        int OverlapSphere(SphereCollider sphereCollider)
        {
            Transform colTransform = sphereCollider.transform;
            Vector3 center = colTransform.TransformPoint(sphereCollider.center);
            Vector3 scale = Abs(colTransform.lossyScale);
            float radius = sphereCollider.radius * Mathf.Max(scale.x, scale.y, scale.z);
            return Physics.OverlapSphereNonAlloc(center, radius, mResults, _TargetLayerMask, QueryTriggerInteraction.Collide);
        }

        int OverlapCapsule(CapsuleCollider capsuleCollider)
        {
            Transform colTransform = capsuleCollider.transform;
            Vector3 scale = Abs(colTransform.lossyScale);
            Vector3 center = colTransform.TransformPoint(capsuleCollider.center);
            Vector3 direction = GetCapsuleDirection(capsuleCollider.direction);

            float axisScale = GetAxisScale(scale, capsuleCollider.direction);
            float radiusScale = GetCapsuleRadiusScale(scale, capsuleCollider.direction);
            float radius = capsuleCollider.radius * radiusScale;
            float height = Mathf.Max(capsuleCollider.height * axisScale, radius * 2f);
            float halfCylinderHeight = Mathf.Max(0f, (height * 0.5f) - radius);
            Vector3 worldDirection = colTransform.TransformDirection(direction);
            Vector3 pointOffset = worldDirection * halfCylinderHeight;

            return Physics.OverlapCapsuleNonAlloc(center - pointOffset, center + pointOffset, radius, mResults, _TargetLayerMask, QueryTriggerInteraction.Collide);
        }

        public void SetLayerMask(LayerMask layerMask)
        {
            if (_TargetLayerMask == layerMask)
                return;

            ClearDetectedColliders(true);
            _TargetLayerMask = layerMask;
        }

        bool IsDetectable(Collider col)
        {
            if (col == null || col == mCollider)
                return false;

            int layerMask = 1 << col.gameObject.layer;
            return (_TargetLayerMask.value & layerMask) != 0;
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
                foreach (Collider col in mDetectedColliders)
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

        Vector3 Abs(Vector3 value)
        {
            value.x = Mathf.Abs(value.x);
            value.y = Mathf.Abs(value.y);
            value.z = Mathf.Abs(value.z);
            return value;
        }

        Vector3 GetCapsuleDirection(int direction)
        {
            if (direction == 0)
                return Vector3.right;

            if (direction == 1)
                return Vector3.up;

            return Vector3.forward;
        }

        float GetAxisScale(Vector3 scale, int direction)
        {
            if (direction == 0)
                return scale.x;

            if (direction == 1)
                return scale.y;

            return scale.z;
        }

        float GetCapsuleRadiusScale(Vector3 scale, int direction)
        {
            if (direction == 0)
                return Mathf.Max(scale.y, scale.z);

            if (direction == 1)
                return Mathf.Max(scale.x, scale.z);

            return Mathf.Max(scale.x, scale.y);
        }
    }
}