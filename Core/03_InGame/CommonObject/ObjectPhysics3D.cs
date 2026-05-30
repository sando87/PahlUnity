using UnityEngine;

namespace PahlUnity
{
    public class ObjectPhysics3D : MonoBehaviour
    {
        [SerializeField] private float _GravityScale = 1f;
        [SerializeField] private float _GroundStickVelocity = -2f;
        [SerializeField] private float _StopJumpVelocity = 5f;

        private CharacterController mCC = null;
        private BaseObject mBase = null;
        private Vector3 mVelocity = Vector3.zero;
        private Vector3 mDashVelocity = Vector3.zero;
        private float mDashRemainTime = 0f;
        private bool mLockGravity = false;
        private bool mLockMovement = false;

        public CharacterController Controller => mCC;
        public bool IsGrounded => mCC != null && mCC.isGrounded;
        public Vector3 Position { get => transform.position; set => Teleport(value); }
        public Vector3 Velocity { get => mVelocity; set => mVelocity = value; }
        public float VelocityX { get => mVelocity.x; set => mVelocity.x = value; }
        public float VelocityY { get => mVelocity.y; set => mVelocity.y = value; }
        public float VelocityZ { get => mVelocity.z; set => mVelocity.z = value; }
        public Vector3 DashVelocity => mDashVelocity;
        public bool IsDashing => mDashRemainTime > 0f;
        public bool LockGravity { get => mLockGravity; set => mLockGravity = value; }
        public bool LockMovement { get => mLockMovement; set => mLockMovement = value; }

        private void Awake()
        {
            mBase = this.ExGetBase();
            mCC = GetComponent<CharacterController>();
        }

        private void Update()
        {
            Simulate(Time.deltaTime);
        }

        public void Move(Vector3 worldVelocity)
        {
            mVelocity.x = worldVelocity.x;
            mVelocity.z = worldVelocity.z;
        }

        public void Move(Vector3 worldDir, float moveSpeed)
        {
            if (worldDir.sqrMagnitude <= 0.0001f)
            {
                StopMoving();
                return;
            }

            Vector3 moveDir = worldDir.normalized;
            Move(moveDir * moveSpeed);
        }

        public void AddVelocity(Vector3 velocity)
        {
            mVelocity += velocity;
        }

        public void DoDash(Vector3 worldDir, float dashSpeed, float duration)
        {
            if (worldDir.sqrMagnitude <= 0.0001f || dashSpeed <= 0f || duration <= 0f)
                return;

            mDashVelocity = worldDir.normalized * dashSpeed;
            mDashRemainTime = duration;
        }

        public void StopDash()
        {
            mDashVelocity = Vector3.zero;
            mDashRemainTime = 0f;
        }

        public void StopMoving()
        {
            mVelocity.x = 0f;
            mVelocity.z = 0f;
            StopDash();
        }

        public void DoJump(float jumpForce)
        {
            mVelocity.y = jumpForce;
        }

        public void StopJump()
        {
            if (mVelocity.y > _StopJumpVelocity)
                mVelocity.y = _StopJumpVelocity;
        }

        public void Teleport(Vector3 position)
        {
            bool wasEnabled = mCC.enabled;
            mCC.enabled = false;
            transform.position = position;
            mCC.enabled = wasEnabled;
        }

        private void Simulate(float deltaTime)
        {
            if (mCC == null || deltaTime <= 0f)
                return;

            if (mLockMovement)
                return;

            ApplyGravity(deltaTime);
            UpdateDash(deltaTime);

            Vector3 frameVelocity = mVelocity + mDashVelocity;
            mCC.Move(frameVelocity * deltaTime);
        }

        private void ApplyGravity(float deltaTime)
        {
            if (mLockGravity)
            {
                mVelocity.y = 0f;
                return;
            }

            if (IsGrounded && mVelocity.y < 0f)
                mVelocity.y = _GroundStickVelocity;

            mVelocity += Physics.gravity * _GravityScale * deltaTime;
        }

        private void UpdateDash(float deltaTime)
        {
            if (mDashRemainTime <= 0f)
                return;

            mDashRemainTime -= deltaTime;
            if (mDashRemainTime <= 0f)
                StopDash();
        }
    }
}