using UnityEngine;
using PahlUnity;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using NaughtyAttributes;

namespace PahlUnity.Demo
{
    public class ProjectileReflect : ProjectileBase
    {
        [SerializeField] bool _BoundsEnabled = true;

        readonly RaycastHit[] mReflectHits = new RaycastHit[8];
        float mLastReflectTime = float.NegativeInfinity;

        const float ReflectCooldown = 0.05f;
        const float ReflectCheckRadius = 0.08f;
        const float ReflectWallNormalMaxY = 0.7f;
        const float ReflectNudgeDistance = 0.05f;

        public override void StartProjectile()
        {
            base.StartProjectile();

            ApplyReflectLayerMask();
        }

        protected override void Update()
        {
            base.Update();

            UpdateReflect();
        }

        protected override void DoHit(HitColliderInfo colInfo)
        {
            TryBoundsOnTerrain(colInfo.Collider);

            TryReflectOnSurface(colInfo.Collider);

            base.DoHit(colInfo);
        }

        void ApplyReflectLayerMask()
        {
            int mask = mTargetLayerMask;
            if (LayerID.Terrain >= 0)
                mask |= 1 << LayerID.Terrain;
            if (LayerID.Props >= 0)
                mask |= 1 << LayerID.Props;

            mTargetLayerMask = mask;
            _AttackCollider.SetTargetLayerMask(mask);
        }

        void UpdateReflect()
        {
            if (Time.time - mLastReflectTime < ReflectCooldown)
                return;

            Vector3 velocity = mPhy.Velocity;
            if (velocity.sqrMagnitude < 0.0001f)
                return;

            Vector3 dir = velocity.normalized;
            float checkDistance = Mathf.Max(Stats.MoveSpeed * Time.deltaTime * 1.5f, 0.15f);
            int mask = mTargetLayerMask; // GetReflectLayerMask();
            if (mask == 0)
                return;

            int hitCount = Physics.SphereCastNonAlloc(
                transform.position,
                ReflectCheckRadius,
                dir,
                mReflectHits,
                checkDistance,
                mask,
                QueryTriggerInteraction.Ignore);

            if (hitCount <= 0)
                return;

            if (!TryFindReflectHit(hitCount, out RaycastHit bestHit))
                return;

            ApplyReflect(dir, bestHit.normal);
        }

        bool TryBoundsOnTerrain(Collider col)
        {
            if (!_BoundsEnabled || col == null)
                return false;

            if (!col.ExHasInteractProperty(InteractMask.Terrain))
                return false;

            Vector3 velocity = mPhy.VelocityPrev;
            if (velocity.sqrMagnitude < 0.0001f)
                return false;

            Vector3 dir = velocity.normalized;
            if (!TryGetBoundsNormal(col, dir, out Vector3 normal))
                return false;

            ApplyBounds(dir, normal);
            return true;
        }

        bool TryReflectOnSurface(Collider col)
        {
            if (!col.ExHasInteractProperty(InteractMask.Terrain | InteractMask.Props | InteractMask.Unit))
                return false;

            if (Time.time - mLastReflectTime < ReflectCooldown)
                return true;

            Vector3 velocity = mPhy.Velocity;
            if (velocity.sqrMagnitude < 0.0001f)
                return false;

            Vector3 dir = velocity.normalized;
            if (!TryGetReflectNormal(col, dir, out Vector3 normal))
                return false;

            ApplyReflect(dir, normal);
            return true;
        }

        void ApplyReflect(Vector3 enterDir, Vector3 normal)
        {
            Vector3 reflected = Vector3.Reflect(enterDir, normal);
            if (reflected.sqrMagnitude < 0.0001f)
                return;

            reflected.Normalize();
            float speed = Stats.MoveSpeed > 0f ? Stats.MoveSpeed : mPhy.Velocity.magnitude;
            mPhy.Velocity = reflected * speed;
            transform.forward = reflected;
            mLastReflectTime = Time.time;

            Vector3 nudge = normal;
            nudge.y = 0f;
            if (nudge.sqrMagnitude > 0.0001f)
                mPhy.Position = transform.position + nudge.normalized * ReflectNudgeDistance;
        }

        void ApplyBounds(Vector3 enterDir, Vector3 normal)
        {
            Vector3 reflected = Vector3.Reflect(enterDir, normal);
            if (reflected.sqrMagnitude < 0.0001f)
                return;

            reflected.Normalize();
            float speed = mPhy.VelocityPrev.magnitude * 0.7f;
            if (speed < 3)
            {
                mPhy.StopMoving();
            }
            else
            {
                mPhy.Velocity = reflected * speed;
            }
        }

        bool TryFindReflectHit(int hitCount, out RaycastHit bestHit)
        {
            bestHit = default;
            float bestDist = float.MaxValue;
            bool found = false;

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit hit = mReflectHits[i];
                if (hit.collider == null)
                    continue;

                if (IsSelfCollider(hit.collider))
                    continue;

                if (!IsWallNormal(hit.normal))
                    continue;

                if (hit.distance < bestDist)
                {
                    bestDist = hit.distance;
                    bestHit = hit;
                    found = true;
                }
            }

            return found;
        }

        bool TryGetReflectNormal(Collider col, Vector3 dir, out Vector3 normal)
        {
            normal = Vector3.zero;
            if (col.ExHasInteractProperty(InteractMask.Unit))
            {
                Vector3 randomReflectDir = MyUtils.Random(Vector3.zero, 1);
                randomReflectDir.y = 0;
                normal = randomReflectDir.normalized;
                return true;
            }

            Ray ray = new Ray(transform.position - dir * 0.5f, dir);
            if (col.Raycast(ray, out RaycastHit hit, 2f) && IsWallNormal(hit.normal))
            {
                normal = hit.normal;
                return true;
            }

            Vector3 closest = col.ClosestPoint(transform.position);
            Vector3 toProjectile = transform.position - closest;
            if (toProjectile.sqrMagnitude < 0.0001f)
                return false;

            normal = toProjectile.normalized;
            return IsWallNormal(normal);
        }

        bool TryGetBoundsNormal(Collider col, Vector3 dir, out Vector3 normal)
        {
            normal = Vector3.zero;
            Ray ray = new Ray(transform.position - dir * 0.5f, dir);
            if (col.Raycast(ray, out RaycastHit hit, 2f))
            {
                normal = hit.normal;
                return true;
            }

            Vector3 closest = col.ClosestPoint(transform.position);
            Vector3 toProjectile = transform.position - closest;
            if (toProjectile.sqrMagnitude < 0.0001f)
                return false;

            normal = toProjectile.normalized;
            return true;
        }

        bool IsWallNormal(Vector3 normal)
        {
            return Mathf.Abs(normal.y) < ReflectWallNormalMaxY;
        }

        bool IsSelfCollider(Collider col)
        {
            return col.transform == transform || col.transform.IsChildOf(transform);
        }

        static int GetReflectLayerMask()
        {
            int mask = 0;
            if (LayerID.Terrain >= 0)
                mask |= 1 << LayerID.Terrain;
            if (LayerID.Props >= 0)
                mask |= 1 << LayerID.Props;
            return mask;
        }
    }
}