using UnityEngine;

namespace PahlUnity.Demo
{
    /// <summary>
    /// 플레이어 캐릭터의 입력을 받아서 이동, 점프, 대쉬 등을 처리하는 컨트롤러 클래스
    /// </summary>
    public class PlayerController3D : MonoBehaviour
    {
        [SerializeField] float _JumpForce = 12f;
        [SerializeField] float _DashSpeed = 15f;
        [SerializeField] float _DashDuration = 0.2f;
        [SerializeField] string _AnimParamMoveSpeed = "MoveSpeed";
        [SerializeField] string _AnimParamIsGrounded = "IsGrounded";
        [SerializeField] string _AnimParamDash = "Dash";

        public bool IsGrounded { get => mBaseObj.Physics3D != null && mBaseObj.Physics3D.IsGrounded; }

        public bool LockMove { get; set; } = false;
        public bool LockJump { get; set; } = false;
        public bool LockDash { get; set; } = false;
        public bool LockAll
        {
            get { return LockMove && LockJump && LockDash; }
            set { LockMove = value; LockJump = value; LockDash = value; }
        }

        BaseObject mBaseObj = null;

        int mAnimParamMoveSpeed = 0;
        int mAnimParamIsGrounded = 0;
        int mAnimParamDash = 0;
        bool mIsSecondJump = false;

        private void Awake()
        {
            mBaseObj = this.ExGetBase();

            mAnimParamMoveSpeed = Animator.StringToHash(_AnimParamMoveSpeed);
            mAnimParamIsGrounded = Animator.StringToHash(_AnimParamIsGrounded);
            mAnimParamDash = Animator.StringToHash(_AnimParamDash);
        }

        private void Update()
        {
            DoMovement();
            Jump();
            DropDown();
            Dash();

            mBaseObj.Anim.SetParamBool(mAnimParamIsGrounded, IsGrounded);
        }

        void DoMovement()
        {
            if (LockMove)
            {
                mBaseObj.Physics3D.Move(Vector3.zero);
                return;
            }

            if (TryGetMoveInput(out Vector3 moveDir))
            {
                mBaseObj.Physics3D.Move(moveDir, mBaseObj.Spec[SpecFields.MoveSpeed]);
                mBaseObj.Body3D.Turn(moveDir);
                mBaseObj.Anim.SetParamFloat(mAnimParamMoveSpeed, moveDir.magnitude);
            }
            else
            {
                mBaseObj.Physics3D.StopMoving();
                mBaseObj.Anim.SetParamFloat(mAnimParamMoveSpeed, 0);
            }
        }

        void Jump()
        {
            if (LockJump)
                return;

            if (mBaseObj.Input.JustPressed(InputActionNameHash.Jump)
            && mBaseObj.Input.MoveY >= 0)
            {
                if (IsGrounded)
                {
                    mIsSecondJump = false;
                    mBaseObj.Physics3D.DoJump(_JumpForce);
                }
                else
                {
                    if (!mIsSecondJump)
                    {
                        mIsSecondJump = true;
                        mBaseObj.Physics3D.DoJump(_JumpForce);
                    }
                }
            }
            else if (mBaseObj.Input.JustReleased(InputActionNameHash.Jump))
            {
                mBaseObj.Physics3D.StopJump();
            }
        }

        void DropDown()
        {
            if (LockJump)
                return;

            if (mBaseObj.Input.JustPressed(InputActionNameHash.Jump)
            && mBaseObj.Input.MoveY < 0
            && IsGrounded)
            {
                mBaseObj.Body3D.LockThinPlatform = true;
                this.ExDelayedCoroutine(0.2f, () => mBaseObj.Body3D.LockThinPlatform = false);
            }
        }

        void Dash()
        {
            if (LockDash)
                return;

            if (mBaseObj.Input.JustPressed(InputActionNameHash.Dash))
            {
                Vector3 dashDir = mBaseObj.Body3D.FrontDirVec3;
                mBaseObj.Physics3D.DoDash(dashDir, _DashSpeed, _DashDuration);
                mBaseObj.Anim.SetParamTrigger(mAnimParamDash);
            }
        }

        bool TryGetMoveInput(out Vector3 moveDir)
        {
            Vector2 moveInput = mBaseObj.Input.MoveXY;
            if (moveInput.sqrMagnitude <= 0.0001f)
            {
                moveDir = Vector3.zero;
                return false;
            }

            moveDir = new Vector3(moveInput.x, 0f, moveInput.y);
            moveDir = moveDir.normalized;
            return true;
        }
    }
}