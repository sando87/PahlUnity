using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using NaughtyAttributes;

namespace PahlUnity
{
    public class ProjectileBase : MonoBehaviour
    {
        [field: SerializeField] public ProjectileInfo Stats { get; set; }

        [SerializeField] protected InteractableCollider _AttackCollider = null;

        [Foldout("Events"), SerializeField] UnityEvent _OnStart;
        [Foldout("Events"), SerializeField] UnityEvent<Collider> _OnHit;
        [Foldout("Events"), SerializeField] UnityEvent _OnEnd;

        protected BaseObject mBaseObj = null;
        protected ObjectPhysics3D mPhy = null;
        protected ObjectBodyBase mBody = null;
        protected List<HitColliderInfo> mHitColliders = new();
        protected int mTargetLayerMask = 0;
        protected Vector3 mStartPos = Vector3.zero;

        public event Action OnStart;
        public event Action<Collider> OnHit;
        public event Action OnEnd;

        public static ProjectileBase Create(ProjectileBase prefab, Vector3 position, Vector3 direction, int targetLayerMask)
        {
            Quaternion rotation = direction == Vector3.zero ? Quaternion.identity : Quaternion.LookRotation(direction);
            ProjectileBase obj = Instantiate(prefab, position, rotation);
            obj.gameObject.SetActive(true);
            obj.mStartPos = position;
            obj.mTargetLayerMask = targetLayerMask;
            obj._AttackCollider.SetTargetLayerMask(targetLayerMask);
            return obj;
        }
        public static ProjectileBase Create(ProjectileBase prefab, Vector3 position, Quaternion rotation, int targetLayerMask)
        {
            ProjectileBase obj = Instantiate(prefab, position, rotation);
            obj.gameObject.SetActive(true);
            obj.mStartPos = position;
            obj.mTargetLayerMask = targetLayerMask;
            obj._AttackCollider.SetTargetLayerMask(targetLayerMask);
            return obj;
        }

        protected virtual void Awake()
        {
            mBaseObj = this.ExGetBase();
            mPhy = mBaseObj.ExGetCompInBase<ObjectPhysics3D>();
            mBody = mBaseObj.ExGetCompInBase<ObjectBodyBase>();
            InitColliderEvents();

            OnStart += () => _OnStart?.Invoke();
            OnHit += (col) => _OnHit?.Invoke(col);
            OnEnd += () => _OnEnd?.Invoke();
        }

        void InitColliderEvents()
        {
            _AttackCollider.OnInteractEnter3D += (col) =>
            {
                HitColliderInfo colInfo = new HitColliderInfo { Collider = col, HitTime = Time.time };
                mHitColliders.Add(colInfo);

                DoHit(colInfo);
            };

            _AttackCollider.OnInteractLeave3D += (col) =>
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
            if (Stats.Acceleration != 0)
                UpdateAcceleration();

            if (Stats.Interval > 0)
                HitEventEveryInterval();

            if (Stats.AttackRange > 0)
                EndAfterDistance();

            if (Stats.AimToVelocity)
            {
                AimToVelocity();
            }
            else if (Stats.RotateSpeed != 0)
            {
                Vector3 rotAxix = Vector3.Cross(Vector3.up, transform.forward).normalized;
                float curVelRate = mBaseObj.Physics3D.Velocity.magnitude / Stats.MoveSpeed;
                float rotSpeed = Stats.RotateSpeed * curVelRate;
                mBaseObj.Render.transform.Rotate(rotAxix, rotSpeed * Time.deltaTime, Space.World);
            }
        }

        void UpdateAcceleration()
        {
            mBaseObj.Physics3D.Acceleration = Stats.Acceleration * transform.forward;
        }

        void LaunchProjectile()
        {
            Vector3 forwardDir = transform.forward;
            float vertLength = 0;
            float horiLength = 0;
            if (Stats.FireAngle != 0)
                vertLength = Mathf.Tan(Stats.FireAngle * Mathf.Deg2Rad);
            if (Stats.FireAngleHori != 0)
                horiLength = Mathf.Tan(Stats.FireAngleHori * Mathf.Deg2Rad);

            Vector3 launchDir = (forwardDir + (vertLength * transform.up) + (horiLength * transform.right)).normalized;
            Vector3 vel = launchDir * Stats.MoveSpeed;
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
            mPhy.Acceleration = Vector3.zero;
            mPhy.LockGravity = true;
            mBody.LockBody = true;

            OnEnd?.Invoke();
            _AttackCollider.LockInteract = true;
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
                    DoHit(info);
                }
            }
        }

        protected virtual void DoHit(HitColliderInfo colInfo)
        {
            colInfo.HitTime = Time.time;
            OnHit?.Invoke(colInfo.Collider);

            if (Stats.PierceChance > 0)
            {
                if (!MyUtils.IsPercentHit((int)Stats.PierceChance))
                {
                    DoEndProjectile();
                }
            }
            else
            {
                if (Stats.EndOnHit)
                {
                    DoEndProjectile();
                }
            }
        }

        public class HitColliderInfo
        {
            public float HitTime;
            public Collider Collider;
        }
    }
    [System.Serializable]
    public class ProjectileInfo
    {
        public float MoveSpeed;
        public float Acceleration;
        public float FireAngle;
        public float FireAngleHori;
        public float AttackRange;
        public float SplashRange;
        public float Duration;
        public float Interval;
        public float StartDelay;
        public float RotateSpeed;
        public float PierceChance;
        public bool AimToVelocity;
        public bool EndOnHit;
        public float DestroyDelay;
    }
}