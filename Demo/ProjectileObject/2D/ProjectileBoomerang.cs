using PahlUnity;
using UnityEngine;

namespace PahlUnity.Demo.TwoD
{
    public class ProjectileBoomerang : ProjectileBase2D
    {
        [SerializeField] private float _ReturnReachDistance = 0.5f;

        BaseObject mOwner = null;
        bool mIsReturning = false;

        public static ProjectileBoomerang Create(ProjectileBoomerang prefab, Vector2 position, Vector2 direction, int targetLayerMask, BaseObject owner)
        {
            ProjectileBase2D obj = ProjectileBase2D.Create(prefab, position, direction, targetLayerMask);
            ProjectileBoomerang boomerang = obj as ProjectileBoomerang;
            boomerang.mOwner = owner;
            return boomerang;
        }

        protected override void Update()
        {
            base.Update();

            UpdateBoomerangMovement();
        }

        void UpdateBoomerangMovement()
        {
            if (!mIsReturning)
            {
                float travelDistance = (mStartPos - transform.position).magnitude;
                if (Stats.AttackRange > 0f && travelDistance >= Stats.AttackRange)
                {
                    mIsReturning = true;
                }
                else
                {
                    return;
                }
            }

            Vector2 returnTargetPos = GetReturnTargetPos();
            Vector2 toOwner = returnTargetPos - (Vector2)transform.position;
            if (toOwner.sqrMagnitude <= _ReturnReachDistance * _ReturnReachDistance)
            {
                DoEndProjectile();
                return;
            }

            Vector2 returnDir = toOwner.normalized;
            Vector2 returnVel = returnDir * Stats.MoveSpeed;
            mPhy.VelocityX = returnVel.x;
            mPhy.VelocityY = returnVel.y;
        }

        Vector2 GetReturnTargetPos()
        {
            if (mOwner == null)
                return mStartPos;

            ObjectBody2D ownerBody = mOwner.GetComp<ObjectBody2D>();
            if (ownerBody == null)
                return mStartPos;

            return ownerBody.Center;
        }
    }
}
