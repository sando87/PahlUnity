using UnityEngine;
using PahlUnity;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using NaughtyAttributes;

namespace PahlUnity.Demo
{
    public class ProjectileHoming : ProjectileBase
    {
        [SerializeField] float _HomingDetectRange;
        [SerializeField] float _HomingDetectInterval;
        [SerializeField] float _HomingTurnSpeed;

        protected Health mHomingTargetHealth = null;

        public bool IsHomingDetected => mHomingTargetHealth != null && !mHomingTargetHealth.IsDead;
        public bool IsHomingSmoothMode => _HomingTurnSpeed > 0;

        readonly Collider[] mHomingResults = new Collider[32];
        float mHomingLastDetectTime = float.NegativeInfinity;

        protected override void Update()
        {
            base.Update();

            UpdateHoming();
        }

        void UpdateHoming()
        {
            float detectInterval = IsHomingDetected ? _HomingDetectInterval : 0.5f;
            bool tryDetect = MyUtils.IsCooltimeOver(mHomingLastDetectTime, detectInterval);
            if (!IsHomingSmoothMode && !tryDetect)
                return;

            if (tryDetect)
            {
                mHomingLastDetectTime = Time.time;
                DetectHomingTarget();
            }

            if (!IsHomingDetected)
                return;

            Vector3 targetPos = mHomingTargetHealth.transform.position;
            Vector3 toTarget = targetPos - transform.position;
            toTarget.y = 0f;
            if (toTarget.sqrMagnitude < 0.000001f)
                return;

            Vector3 desiredDir = toTarget.normalized;
            float turnSpeedDeg = _HomingTurnSpeed;
            if (turnSpeedDeg <= 0f)
            {
                transform.forward = desiredDir;
            }
            else
            {
                float maxRadiansDelta = turnSpeedDeg * Mathf.Deg2Rad * Time.deltaTime;
                transform.forward = Vector3.RotateTowards(transform.forward, desiredDir, maxRadiansDelta, 0f);
            }

            // 호밍으로 회전한 방향에 맞춰 velocity도 갱신
            Vector3 forward = transform.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude > 0f)
                mPhy.Velocity = forward.normalized * Stats.MoveSpeed;
        }

        void DetectHomingTarget()
        {
            float range = _HomingDetectRange;
            if (range <= 0f)
                return;

            Vector3 origin = transform.position;
            float nearestDistSqr = float.PositiveInfinity;
            Health nearestHealth = null;

            int hitCount = Physics.OverlapSphereNonAlloc(origin, range, mHomingResults, mTargetLayerMask, QueryTriggerInteraction.Collide);
            for (int i = 0; i < hitCount; i++)
            {
                Collider col = mHomingResults[i];
                if (col == null)
                    continue;

                Health health = col.ExGetCompInBase<Health>();
                if (health == null || health.IsDead)
                    continue;

                float distSqr = (health.transform.position - origin).sqrMagnitude;
                if (distSqr < nearestDistSqr)
                {
                    nearestDistSqr = distSqr;
                    nearestHealth = health;
                }
            }

            mHomingTargetHealth = nearestHealth;
        }

    }
}