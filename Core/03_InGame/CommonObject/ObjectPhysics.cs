using System;
using DG.Tweening;
using UnityEngine;

namespace PahlBit
{
    public class ObjectPhysics : MonoBehaviour
    {
        public float VelocityX { get { return mRB2D.linearVelocity.x; } set { mRB2D.linearVelocity = new Vector2(value, mRB2D.linearVelocity.y); } }
        public float VelocityY { get { return mRB2D.linearVelocity.y; } set { mRB2D.linearVelocity = new Vector2(mRB2D.linearVelocity.x, value); } }
        public Vector2 Velocity { get { return mRB2D.linearVelocity; } set { mRB2D.linearVelocity = value; } }
        public bool LockGravity { get => mRB2D.gravityScale == 0; set => mRB2D.gravityScale = value ? 0 : mOriGravityScale; }
        public bool LockMovement { get => mRB2D.bodyType != RigidbodyType2D.Dynamic; set => mRB2D.bodyType = value ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic; }
        public bool IsGrounded { get => mIsGround && VelocityY <= 0.1f; }

        private BaseObject mBase = null;
        private Rigidbody2D mRB2D = null;
        private float mOriGravityScale = 1;
        private bool mIsGround = false;

        private void Awake()
        {
            mBase = this.ExGetBase();
            mRB2D = GetComponent<Rigidbody2D>();
            mOriGravityScale = mRB2D.gravityScale;
        }

        void FixedUpdate()
        {
            UpdateGroundState();
        }

        public void AddForce(Vector2 force, ForceMode2D mode = ForceMode2D.Impulse)
        {
            mRB2D.AddForce(force, mode);
        }

        public void MoveFootPosition(Vector3 pos)
        {
            mRB2D.transform.position = pos;
        }


        public void MoveHorizontally(float moveHoriVelocity)
        {
            TurnToInput(moveHoriVelocity);
            VelocityX = moveHoriVelocity;
        }
        public void SetMoveSpeedOnly(float moveSpeedX)
        {
            float curDir = mBase.transform.right.x > 0 ? 1 : -1;
            VelocityX = curDir * Mathf.Abs(moveSpeedX);
        }
        public void StopMoving()
        {
            Velocity = Vector2.zero;
        }
        public void TurnToWorldDir(int rightDir)
        {
            if (rightDir == 0) return;

            Vector3 front = rightDir > 0 ? Vector3.forward : Vector3.back;
            transform.rotation = Quaternion.LookRotation(front, transform.up);
        }
        public void TurnToInput(float moveX)
        {
            if (moveX.ExIsAlmostZero()) return;

            int rightDir = moveX > 0 ? 1 : -1;
            TurnToWorldDir(rightDir);
        }
        public void TurnToTarget(Transform target)
        {
            if (target == null) return;

            int rightDir = target.position.x > mBase.transform.position.x ? 1 : -1;
            TurnToWorldDir(rightDir);
        }

        public void DoJump(float jumpForce)
        {
            // 수직 속도 초기화 후 점프력 적용
            VelocityY = 0;
            AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        public void StopJump()
        {
            // 수직 속도 초기화 후 점프력 적용
            if (VelocityY > 5)
                VelocityY = 5;
        }

        private void UpdateGroundState()
        {
            int layerMask = MyLayerMask.Ground;
            Vector2 footPos = mBase.Body.Foot;

            bool isOverlapped = Physics2D.OverlapCircle(footPos + new Vector2(0, 0.1f), 0.05f, layerMask);

            Vector2 bodySize = mBase.Body.Size;
            Rect box = new Rect();
            box.size = new Vector2(bodySize.x, 0.1f);
            box.center = footPos + new Vector2(0, 0.05f);
            bool isCasted = Physics2D.BoxCast(box.center, box.size, 0, Vector2.down, 0.1f, layerMask);

            mIsGround = !isOverlapped && isCasted;
        }
    }
}