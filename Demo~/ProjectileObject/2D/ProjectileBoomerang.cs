using PahlUnity;
using UnityEngine;

namespace PahlUnity.Demo.TwoD
{
    public class ProjectileBoomerang : ProjectileBase2D
    {
        [SerializeField] private float _ReturnReachDistance = 0.5f;

        bool mIsReturning = false;

        protected override void Update()
        {
            if (Stats.Interval > 0)
                HitEventEveryInterval();

            if (Stats.AimToVelocity)
            {
                AimToVelocity();
            }
            else if (Stats.RotateSpeed != 0)
            {
                transform.Rotate(0, 0, Stats.RotateSpeed * Time.deltaTime);
            }

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
            if (mCaster == null)
                return mStartPos;

            ObjectBody2D ownerBody = mCaster.GetComp<ObjectBody2D>();
            if (ownerBody == null)
                return mStartPos;

            return ownerBody.Center;
        }
    }
}
