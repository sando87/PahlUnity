using UnityEngine;

namespace PahlUnity
{
    /// <summary>
    /// 플레이어 캐릭터의 입력을 받아서 이동, 점프, 대쉬 등을 처리하는 컨트롤러 클래스
    /// </summary>
    public class PlayerController3D : MonoBehaviour
    {
        [SerializeField] float _JumpForce = 12f;
        [SerializeField] float _DashSpeed = 15f;
        [SerializeField] float _DashDuration = 0.2f;

        public bool IsGrounded { get => mPhy != null && mPhy.IsGrounded; }

        public bool LockMove { get; set; } = false;
        public bool LockJump { get; set; } = false;
        public bool LockDash { get; set; } = false;
        public bool LockAll
        {
            get { return LockMove && LockJump && LockDash; }
            set { LockMove = value; LockJump = value; LockDash = value; }
        }

        BaseObject mBaseObj = null;
        ObjectPhysics3D mPhy = null;
        ObjectBody3D mBody = null;
        InputPlayer mPlayerInput = null;
        SpecBase mSpec = null;

        bool mIsSecondJump = false;

        private void Awake()
        {
            mBaseObj = GetComponentInParent<BaseObject>();
            mPhy = mBaseObj.GetComp<ObjectPhysics3D>();
            mBody = mBaseObj.GetComp<ObjectBody3D>();
            mPlayerInput = mBaseObj.GetComp<InputPlayer>();
            mSpec = mBaseObj.GetComp<SpecBase>();
        }

        private void Update()
        {
            DoMovement();
            Jump();
            DropDown();
            Dash();
        }

        void DoMovement()
        {
            if (LockMove)
            {
                mPhy.Move(Vector3.zero);
                return;
            }

            Vector3 moveDir = GetMoveDirection();
            mPhy.Move(moveDir, mSpec.MoveSpeed);

            if (moveDir.sqrMagnitude > 0.0001f)
                mBody.Turn(moveDir);
        }

        void Jump()
        {
            if (LockJump)
                return;

            if (mPlayerInput.JustPressed(InputActionName.Jump)
            && mPlayerInput.MoveY >= 0)
            {
                if (IsGrounded)
                {
                    mIsSecondJump = false;
                    mPhy.DoJump(_JumpForce);
                }
                else
                {
                    if (!mIsSecondJump)
                    {
                        mIsSecondJump = true;
                        mPhy.DoJump(_JumpForce);
                    }
                }
            }
            else if (mPlayerInput.JustReleased(InputActionName.Jump))
            {
                mPhy.StopJump();
            }
        }

        void DropDown()
        {
            if (LockJump)
                return;

            if (mPlayerInput.JustPressed(InputActionName.Jump)
            && mPlayerInput.MoveY < 0
            && IsGrounded)
            {
                mBody.LockThinPlatform = true;
                this.ExDelayedCoroutine(0.2f, () => mBody.LockThinPlatform = false);
            }
        }

        void Dash()
        {
            if (LockDash)
                return;

            if (mPlayerInput.JustPressed(InputActionName.Dash))
            {
                Vector3 dashDir = GetMoveDirection();
                if (dashDir.sqrMagnitude <= 0.0001f)
                    dashDir = mBody.FrontDirVec3;

                mPhy.DoDash(dashDir, _DashSpeed, _DashDuration);
            }
        }

        Vector3 GetMoveDirection()
        {
            Vector2 moveInput = mPlayerInput.MoveXY;
            if (moveInput.sqrMagnitude <= 0.0001f)
                return Vector3.zero;

            Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y);
            return moveDir.normalized;
        }
    }
}