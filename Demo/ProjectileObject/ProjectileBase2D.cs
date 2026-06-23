using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PahlUnity.Demo
{
    public class ProjectileBase2D : MonoBehaviour
    {
        [field: SerializeField]
        public ProjectileInfo Stats { get; set; }

        protected BaseObject mBaseObj = null;
        protected ObjectPhysics2D mPhy = null;
        protected ObjectBody2D mBody = null;
        protected InteractableCollider mInteractCollider = null;
        protected List<HitColliderInfo> mHitColliders = new List<HitColliderInfo>();
        protected Vector2 mStartPos = Vector2.zero;

        public UnityEvent OnStart;
        public UnityEvent<Collider2D> OnHit;
        public UnityEvent OnEnd;

        public static ProjectileBase2D Create(ProjectileBase2D prefab, Vector2 position, Vector2 direction, int layer)
        {
            ProjectileBase2D obj = Instantiate(prefab, position, Quaternion.identity);
            obj.transform.right = direction;
            obj.mStartPos = position;
            obj.gameObject.ExSetLayerAll(layer);
            return obj;
        }
        public static ProjectileBase2D Create(ProjectileBase2D prefab, Vector3 position, Quaternion rotation, int layer)
        {
            ProjectileBase2D obj = Instantiate(prefab, position, rotation);
            obj.mStartPos = position;
            obj.gameObject.ExSetLayerAll(layer);
            return obj;
        }

        protected virtual void Awake()
        {
            mBaseObj = this.ExGetBase();
            mPhy = mBaseObj.ExGetCompInBase<ObjectPhysics2D>();
            mBody = mBaseObj.ExGetCompInBase<ObjectBody2D>();
            mInteractCollider = GetComponentInChildren<InteractableCollider>();
            InitColliderEvents();
        }

        void InitColliderEvents()
        {
            mInteractCollider.OnInteractEnter2D += (col) =>
            {
                mHitColliders.Add(new HitColliderInfo { Collider = col, HitTime = Time.time });
                OnHit?.Invoke(col);

                if (Stats.EndOnHit)
                    DoEndProjectile();
            };

            mInteractCollider.OnInteractLeave2D += (col) =>
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
                transform.right = mPhy.Velocity.normalized;
            else if (Stats.RotateSpeed != 0)
                transform.Rotate(0, 0, Stats.RotateSpeed * Time.deltaTime);
        }

        void LaunchProjectile()
        {
            if (Stats.FireAngle != 0)
            {
                Vector2 dir = Quaternion.AngleAxis(Stats.FireAngle, transform.forward) * transform.right;
                Vector2 vel = dir * Stats.MoveSpeed;
                mPhy.VelocityX = vel.x;
                mPhy.VelocityY = vel.y;
            }
            else
            {
                Vector2 dir = transform.right;
                Vector2 vel = dir * Stats.MoveSpeed;
                mPhy.VelocityX = vel.x;
                mPhy.VelocityY = vel.y;
            }
        }
        void EndAfterDistance()
        {
            if ((mStartPos - transform.position.ExToVector2()).magnitude > Stats.AttackRange)
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
            mPhy.Velocity = Vector2.zero;
            mPhy.LockGravity = true;
            mBody.LockBody = true;

            OnEnd?.Invoke();
            mInteractCollider.LockInteract = true;
            mHitColliders.Clear();
            enabled = false;

            if (Stats.DestroyDelay > 0)
                mBaseObj.DestroyObj(Stats.DestroyDelay);
            else if (Stats.DestroyDelay == 0)
                mBaseObj.DestroyObj();
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
            public Collider2D Collider;
        }

    }
}