using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

namespace PahlUnity
{
    /// <summary>
    /// 플레이어 캐릭터의 입력을 받아서 이동, 점프, 상호작용 등을 처리하는 컨트롤러 클래스
    /// 주요기능 : 복잡한 주변의 상황과 입력에 따라 그에 맞는 FSM 상태를 전환시킨다.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] float _JumpForce = 25f;

        public bool IsGrounded { get => mBaseObj.Phy.IsGrounded; }

        public bool LockMove { get; set; } = false;
        public bool LockJump { get; set; } = false;
        public bool LockDash { get; set; } = false;
        public bool LockSkill { get; set; } = false;
        public bool LockAll
        {
            get { return LockMove && LockJump && LockDash && LockSkill; }
            set { LockMove = value; LockJump = value; LockDash = value; LockSkill = value; }
        }

        BaseObject mBaseObj = null;
        SpecPlayer mSpec = null;
        PlayerUnitInput mPlayerInput = null;
        FiniteStateMachine mFSM = null;
        SkillController mSkillCtrl = null;

        PlayerStateIdle mFsmIdle = null;
        PlayerStateWalk mFsmWalk = null;
        PlayerStateFloating mFsmFloat = null;
        bool mIsSecondJump = false;

        private void Awake()
        {
            mBaseObj = GetComponentInParent<BaseObject>();
            mSpec = mBaseObj.GetComponentInChildren<SpecPlayer>();
            mPlayerInput = mBaseObj.GetComponentInChildren<PlayerUnitInput>();
            mSkillCtrl = mBaseObj.GetComponentInChildren<SkillController>();

            mFSM = mBaseObj.StateMachine;
            mFsmIdle = mFSM.FindState<PlayerStateIdle>();
            mFsmWalk = mFSM.FindState<PlayerStateWalk>();
            mFsmFloat = mFSM.FindState<PlayerStateFloating>();
        }
        void Start()
        {
        }
        private void Update()
        {
            DoMovement();
            Jump();
            DropDown();
            Dash();
            SkillInput();
        }

        void DoMovement()
        {
            if (LockMove)
                return;

            if (IsGrounded)
            {
                float moveX = mPlayerInput.MoveX;
                if (!moveX.ExIsAlmostZero())
                {
                    mFSM.TryChangeState(mFsmWalk);
                }
                else
                {
                    mFSM.TryChangeState(mFsmIdle);
                }
            }
            else
            {
                mFSM.TryChangeState(mFsmFloat);
            }

        }
        void Jump()
        {
            if (LockJump)
                return;

            if (mPlayerInput.JustPressed(PlayerUnitInputType.Jump)
            && mPlayerInput.MoveY >= 0)
            {
                if (IsGrounded)
                {
                    mSkillCtrl.ReleaseAllSkillSlot();
                    mIsSecondJump = false;
                    // SimulateJumpPoints();
                    mBaseObj.Phy.DoJump(_JumpForce);
                    mFSM.ForceChangeState(mFsmFloat);
                }
                else
                {
                    if (!mIsSecondJump)
                    {
                        mSkillCtrl.ReleaseAllSkillSlot();
                        mIsSecondJump = true;
                        mBaseObj.Phy.DoJump(_JumpForce);
                        mFSM.ForceChangeState(mFsmFloat);
                    }
                }
            }
            else if (mPlayerInput.JustReleased(PlayerUnitInputType.Jump))
            {
                mBaseObj.Phy.StopJump();
            }
        }
        void DropDown()
        {
            if (LockJump)
                return;

            if (mPlayerInput.JustPressed(PlayerUnitInputType.Jump)
            && mPlayerInput.MoveY < 0
            && IsGrounded)
            {
                mSkillCtrl.ReleaseAllSkillSlot();
                mBaseObj.Body.LockThinPlatform = true;
                this.ExDelayedCoroutine(0.2f, () => mBaseObj.Body.LockThinPlatform = false);
            }
        }
        void Dash()
        {
            if (LockDash)
                return;

            if (mPlayerInput.JustPressed(PlayerUnitInputType.Dash))
            {
                mSkillCtrl.ReleaseAllSkillSlot();
                mFSM.TryChangeState<PlayerStateDash>();
            }
        }
        void SkillInput()
        {
            if (LockSkill)
            {
                mSkillCtrl.ReleaseAllSkillSlot();
                return;
            }

            if (mPlayerInput.JustPressed(PlayerUnitInputType.SkillSlotA))
                mSkillCtrl.JustPressedSkillSlot(0);
            else if (mPlayerInput.JustReleased(PlayerUnitInputType.SkillSlotA))
                mSkillCtrl.JustReleasedSkillSlot(0);

            if (mPlayerInput.JustPressed(PlayerUnitInputType.SkillSlotB))
                mSkillCtrl.JustPressedSkillSlot(1);
            else if (mPlayerInput.JustReleased(PlayerUnitInputType.SkillSlotB))
                mSkillCtrl.JustReleasedSkillSlot(1);

            if (mPlayerInput.JustPressed(PlayerUnitInputType.SkillSlotC))
                mSkillCtrl.JustPressedSkillSlot(2);
            else if (mPlayerInput.JustReleased(PlayerUnitInputType.SkillSlotC))
                mSkillCtrl.JustReleasedSkillSlot(2);

            if (mPlayerInput.JustPressed(PlayerUnitInputType.SkillSlotD))
                mSkillCtrl.JustPressedSkillSlot(3);
            else if (mPlayerInput.JustReleased(PlayerUnitInputType.SkillSlotD))
                mSkillCtrl.JustReleasedSkillSlot(3);
        }

        [Button("Simulate Jump Points")]
        public void SimulateJumpPoints()
        {
            JumpSimulationTable.DrawSimulationPoints(mBaseObj.transform.position, _JumpForce);
        }

    }
}