using PahlUnity;
using UnityEngine;

namespace PahlUnity.Demo.TwoD
{
    public class ProjectileShuriken : ProjectileBase2D
    {
        [SerializeField] private float _EnemyDetectDistance = 5f;
        [SerializeField] private float _EnemyDetectHalfAngle = 30f;
        [SerializeField] private float _ReflectOffset = 0.1f;

        int mTerrainLayerMask = 0;
        int mEnemyLayerMask = 0;
        ContactFilter2D mEnemyDetectFilter;
        readonly Collider2D[] mDetectResults = new Collider2D[16];

        public static ProjectileShuriken Create(ProjectileShuriken prefab, Vector2 position, Vector2 direction, int targetLayerMask, int terrainLayerMask, int enemyLayerMask)
        {
            ProjectileBase2D obj = ProjectileBase2D.Create(prefab, position, direction, targetLayerMask);
            ProjectileShuriken shuriken = obj as ProjectileShuriken;
            shuriken.mTerrainLayerMask = terrainLayerMask;
            shuriken.mEnemyLayerMask = enemyLayerMask;
            return shuriken;
        }

        protected override void Start()
        {
            OnHit += HandleHit;
            mEnemyDetectFilter = new ContactFilter2D();
            mEnemyDetectFilter.useTriggers = true;
            mEnemyDetectFilter.SetLayerMask(mEnemyLayerMask);
            base.Start();
        }

        public override void StartProjectile()
        {
            // mPhy.LockGravity = true;
            base.StartProjectile();
        }

        protected override void Update()
        {
            base.Update();

            UpdateHoming();
        }

        void UpdateHoming()
        {
            Vector2 moveDir = mPhy.Velocity;
            if (moveDir.sqrMagnitude < 0.01f)
                return;

            moveDir.Normalize();
            float detectDistance = _EnemyDetectDistance > 0f ? _EnemyDetectDistance : Stats.AttackRange;
            if (detectDistance <= 0f)
                return;

            Collider2D targetCollider = FindEnemyInFan((Vector2)transform.position, moveDir, detectDistance);
            if (targetCollider == null)
                return;

            ObjectBody2D enemyBody = targetCollider.ExGetCompInBase<ObjectBody2D>();
            Vector2 targetPos = enemyBody != null ? enemyBody.Center : targetCollider.bounds.center;
            Vector2 toEnemy = targetPos - (Vector2)transform.position;
            if (toEnemy.sqrMagnitude < 0.01f)
                return;

            SetMoveDirection(toEnemy.normalized);
        }

        Collider2D FindEnemyInFan(Vector2 origin, Vector2 moveDir, float detectDistance)
        {
            float detectDistanceSqr = detectDistance * detectDistance;
            int hitCount = Physics2D.OverlapCircle(origin, detectDistance, mEnemyDetectFilter, mDetectResults);

            Collider2D closestTarget = null;
            float closestDistanceSqr = detectDistanceSqr;

            for (int i = 0; i < hitCount; ++i)
            {
                Collider2D col = mDetectResults[i];
                if (col == null)
                    continue;

                Health health = col.ExGetCompInBase<Health>();
                if (health == null)
                    continue;

                ObjectBody2D enemyBody = col.ExGetCompInBase<ObjectBody2D>();
                Vector2 targetPos = enemyBody != null ? enemyBody.Center : col.bounds.center;
                Vector2 toTarget = targetPos - origin;
                float distanceSqr = toTarget.sqrMagnitude;
                if (distanceSqr > detectDistanceSqr || distanceSqr < 0.01f)
                    continue;

                float angle = Vector2.Angle(moveDir, toTarget);
                if (angle > _EnemyDetectHalfAngle)
                    continue;

                if (distanceSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;
                    closestTarget = col;
                }
            }

            return closestTarget;
        }

        void HandleHit(Collider2D col)
        {
            if (!IsTerrain(col))
                return;

            ReflectOnTerrain(col);
        }

        bool IsTerrain(Collider2D col)
        {
            int layerMask = 1 << col.gameObject.layer;
            return (layerMask & mTerrainLayerMask) != 0;
        }

        void ReflectOnTerrain(Collider2D col)
        {
            Vector2 pos = transform.position;
            Vector2 vel = mPhy.Velocity;
            if (vel.sqrMagnitude < 0.01f)
                return;

            Vector2 closest = col.ClosestPoint(pos);
            Vector2 normal = pos - closest;
            if (normal.sqrMagnitude < 0.01f)
                normal = -vel.normalized;
            else
                normal.Normalize();

            Vector2 reflected = Vector2.Reflect(vel, normal);
            SetMoveDirection(reflected.normalized);
            transform.position = pos + normal * _ReflectOffset;
        }

        void SetMoveDirection(Vector2 dir)
        {
            mPhy.Velocity = dir * Stats.MoveSpeed;
            // transform.right = dir;
        }
    }
}
