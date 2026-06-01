using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PahlUnity.Demo
{
    public class ProjectileBase3D : MonoBehaviour
    {
        [field: SerializeField]
        public ProjectileInfo Stats { get; set; }

        protected BaseObject mBaseObj = null;
        protected ObjectPhysics3D mPhy = null;
        protected ObjectBody3D mBody = null;
        protected InteractableCollider mInteractCollider = null;
        protected List<HitColliderInfo> mHitColliders = new List<HitColliderInfo>();
        protected Vector3 mStartPos = Vector3.zero;

        public UnityEvent OnStart;
        public UnityEvent<Collider> OnHit;
        public UnityEvent OnEnd;

        public static ProjectileBase3D Create(ProjectileBase3D prefab, Vector3 position, Vector3 direction, int layer)
        {
            Quaternion rotation = direction == Vector3.zero ? Quaternion.identity : Quaternion.LookRotation(direction);
            ProjectileBase3D obj = Instantiate(prefab, position, rotation);
            obj.mStartPos = position;
            obj.gameObject.ExSetLayerAll(layer);
            return obj;
        }
        public static ProjectileBase3D Create(ProjectileBase3D prefab, Vector3 position, Quaternion rotation, int layer)
        {
            ProjectileBase3D obj = Instantiate(prefab, position, rotation);
            obj.mStartPos = position;
            obj.gameObject.ExSetLayerAll(layer);
            return obj;
        }

        protected virtual void Awake()
        {
            mBaseObj = this.ExGetBase();
            mPhy = mBaseObj.ExGetCompInBase<ObjectPhysics3D>();
            mBody = mBaseObj.ExGetCompInBase<ObjectBody3D>();
            mInteractCollider = GetComponentInChildren<InteractableCollider>();
            InitColliderEvents();
        }

        void InitColliderEvents()
        {
            mInteractCollider.OnInteractEnter3D += (col) =>
            {
                mHitColliders.Add(new HitColliderInfo { Collider = col, HitTime = Time.time });
                OnHit?.Invoke(col);
            };

            mInteractCollider.OnInteractLeave3D += (col) =>
            {
                mHitColliders.RemoveAll(info => info.Collider == col);
            };
        }

        protected virtual void Start()
        {
            if (Stats.StartDelay > 0)
            {
                enabled = false;
                this.ExDelayedCoroutine(Stats.StartDelay, StartProjectile);
            }
            else if (Stats.StartDelay == 0)
            {
                StartProjectile();
            }
            else
            {
                enabled = false;
            }
        }

        public virtual void StartProjectile()
        {
            enabled = true;

            if (Stats.Duration > 0)
                EndAfterDuration();

            if (Stats.MoveSpeed > 0)
                LaunchProjectile();

            OnStart?.Invoke();
        }

        protected virtual void Update()
        {
            if (Stats.Interval > 0)
                HitEventEveryInterval();

            if (Stats.AttackRange > 0)
                EndAfterDistance();

            if (Stats.AimToVelocity)
                AimToVelocity();
            else if (Stats.RotateSpeed != 0)
                transform.Rotate(0, 0, Stats.RotateSpeed * Time.deltaTime);
        }

        void LaunchProjectile()
        {
            Vector3 dir = transform.forward;
            if (Stats.FireAngle != 0)
                dir = Quaternion.AngleAxis(Stats.FireAngle, transform.right) * dir;

            Vector3 vel = dir.normalized * Stats.MoveSpeed;
            mPhy.Velocity = vel;
        }

        void AimToVelocity()
        {
            Vector3 velocity = mPhy.Velocity;
            if (velocity == Vector3.zero)
                return;

            transform.forward = velocity.normalized;
        }

        void EndAfterDistance()
        {
            if ((mStartPos - transform.position).magnitude > Stats.AttackRange)
            {
                DoEndProjectile();
            }
        }

        void EndAfterDuration()
        {
            this.ExDelayedCoroutine(Stats.Duration, DoEndProjectile);
        }

        public virtual void DoEndProjectile()
        {
            StopAllCoroutines();
            mPhy.Velocity = Vector3.zero;
            mPhy.LockGravity = true;
            mBody.LockBody = true;

            OnEnd?.Invoke();
            mInteractCollider.LockInteract = true;
            mHitColliders.Clear();
            enabled = false;
        }

        public void DestroyNow()
        {
            Destroy(gameObject);
        }

        void HitEventEveryInterval()
        {
            // 현재 Hit된 콜라이더들을 interval마다 OnHit콜백 호출해줌
            double interval = Stats.Interval;
            foreach (HitColliderInfo info in mHitColliders)
            {
                float lastHitTime = info.HitTime;
                if (Time.time - lastHitTime >= interval)
                {
                    info.HitTime = Time.time;
                    OnHit?.Invoke(info.Collider);
                }
            }
        }

        public class HitColliderInfo
        {
            public float HitTime;
            public Collider Collider;
        }
    }
}