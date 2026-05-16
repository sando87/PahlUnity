using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PahlUnity
{
    public class ProjectileBase : MonoBehaviour
    {
        [field: SerializeField]
        public ProjectileInfo Stats { get; set; }

        protected BaseObject mBaseObj = null;
        protected InteractableCollider mInteractCollider = null;
        protected Dictionary<Collider2D, float> mHitColliders = new Dictionary<Collider2D, float>();
        protected Vector2 mStartPos = Vector2.zero;

        public UnityEvent OnStart;
        public UnityEvent<Collider2D> OnHit;
        public UnityEvent OnEnd;

        public static ProjectileBase Create(ProjectileBase prefab, Vector2 position, Vector2 direction, int layer)
        {
            ProjectileBase obj = Instantiate(prefab, position, Quaternion.identity);
            obj.transform.right = direction;
            obj.mStartPos = position;
            obj.gameObject.ExSetLayerAll(layer);
            return obj;
        }
        public static ProjectileBase Create(ProjectileBase prefab, Vector3 position, Quaternion rotation, int layer)
        {
            ProjectileBase obj = Instantiate(prefab, position, rotation);
            obj.mStartPos = position;
            obj.gameObject.ExSetLayerAll(layer);
            return obj;
        }

        protected virtual void Awake()
        {
            mBaseObj = this.ExGetBase();
            mInteractCollider = GetComponentInChildren<InteractableCollider>();
            InitColliderEvents();
        }

        void InitColliderEvents()
        {
            mInteractCollider.OnInteractEnter.AddListener((col) =>
            {
                mHitColliders[col] = Time.time;
                OnHit?.Invoke(col);
            });
            mInteractCollider.OnInteractLeave.AddListener((col) =>
            {
                mHitColliders.Remove(col);
            });
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
                transform.right = mBaseObj.Phy.Velocity.normalized;
            else if (Stats.RotateSpeed != 0)
                transform.Rotate(0, 0, Stats.RotateSpeed * Time.deltaTime);
        }

        void LaunchProjectile()
        {
            if (Stats.FireAngle != 0)
            {
                Vector2 dir = Quaternion.AngleAxis(Stats.FireAngle, transform.forward) * transform.right;
                Vector2 vel = dir * Stats.MoveSpeed;
                mBaseObj.Phy.VelocityX = vel.x;
                mBaseObj.Phy.VelocityY = vel.y;
            }
            else
            {
                Vector2 dir = transform.right;
                Vector2 vel = dir * Stats.MoveSpeed;
                mBaseObj.Phy.VelocityX = vel.x;
                mBaseObj.Phy.VelocityY = vel.y;
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
            mBaseObj.Phy.Velocity = Vector2.zero;
            mBaseObj.Phy.LockGravity = true;
            mBaseObj.Body.LockBody = true;

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
            List<Collider2D> tmpList = TemporaryList<Collider2D>.GetTempList();
            tmpList.AddRange(mHitColliders.Keys);

            double interval = Stats.Interval;
            foreach (Collider2D col in tmpList)
            {
                float lastHitTime = mHitColliders[col];
                if (Time.time - lastHitTime >= interval)
                {
                    mHitColliders[col] = Time.time;
                    OnHit?.Invoke(col);
                }
            }
            tmpList.Clear();
        }

        // public readonly struct HitColInfo
        // {
        //     public HitColInfo(float hitTime, Collider2D collider)
        //     {
        //         HitTime = hitTime;
        //         Collider = collider;
        //     }
        //     public float HitTime { get; }
        //     public Collider2D Collider { get; }
        // }

    }
}