using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

namespace PahlUnity.Demo
{
    /// <summary>
    /// 플레이어 캐릭터의 입력을 받아서 이동, 점프, 상호작용 등을 처리하는 컨트롤러 클래스
    /// </summary>
    public class PlayerController2D : MonoBehaviour
    {
        enum PlayerState
        {
            Normal,
            Dashing,
            Damaged,
            Dead,
        }

        [SerializeField] private float _JumpForce = 12f;
        [SerializeField] private bool _EnableWallAttach = false;
        [SerializeField] private float _WallSlideSpeed = 1.5f;
        [SerializeField] private float _WallAttachMinFallSpeed = 0.5f;
        [SerializeField] private float _WallAttachBlockAfterJump = 0.25f;
        [SerializeField] private Vector2 _WallJumpForce = new Vector2(8f, 12f);
        [SerializeField] private float _DashCooldown = 0.3f;
        [SerializeField] private float _DropDownSpeed = 3f;
        [SerializeField] private float _DropDownMinDuration = 0.35f;
        [SerializeField] private float _DropDownMaxDuration = 0.8f;
        [SerializeField] private Vector2 _GroundCheckSize = new Vector2(0.42f, 0.08f);
        [SerializeField] private Vector2 _WallCheckSize = new Vector2(0.08f, 1.3f);
        [SerializeField] private float _CheckDistance = 0.04f;
        [SerializeField] private float _DashSpeed = 5;
        [SerializeField] private float _DashDistance = 5;

        BaseObject mBaseObj = null;
        RenderController mRender = null;
        ObjectPhysics2D mPhy = null;
        ObjectBody2D mBody = null;
        InputPlayer mPlayerInput = null;
        AnimatorHelper mAnim = null;
        Health mHealth = null;
        SpecBase mSpec = null;
        FiniteStateMachine mFSM = null;
        Dictionary<PlayerState, FiniteStateBase> mStates = new();

        readonly Collider2D[] mOverlapResults = new Collider2D[4];

        int mTerrainLayerMask = 0;
        int mThinPlatformLayerMask = 0;
        int mGroundLayerMask = 0;
        ContactFilter2D mTerrainContactFilter;
        ContactFilter2D mThinPlatformContactFilter;
        ContactFilter2D mGroundContactFilter;
        bool mIsGrounded = false;
        bool mIsWallAttached = false;
        bool mDidDoubleJump = false;
        bool mIsDroppingDown = false;
        float mWallAttachBlockTimer = 0f;
        int mDropDownMotionID = 0;
        float mDashStartTime = 0;

        void Awake()
        {
            mBaseObj = this.ExGetBase();
            mRender = mBaseObj.GetComp<RenderController>();
            mPhy = mBaseObj.GetComp<ObjectPhysics2D>();
            mBody = mBaseObj.GetComp<ObjectBody2D>();
            mPlayerInput = mBaseObj.GetComp<InputPlayer>();
            mAnim = mBaseObj.GetComp<AnimatorHelper>();
            mHealth = mBaseObj.GetComp<Health>();
            mSpec = mBaseObj.GetComp<SpecBase>();
            mFSM = mBaseObj.GetComp<FiniteStateMachine>();
            if (mFSM == null)
                mFSM = mBaseObj.gameObject.AddComponent<FiniteStateMachine>();

            mTerrainLayerMask = GetLayerMask(LayerID.Terrain);
            mThinPlatformLayerMask = GetLayerMask(LayerID.ThinPlatform);
            mGroundLayerMask = mTerrainLayerMask | mThinPlatformLayerMask;
            mTerrainContactFilter = CreateContactFilter(mTerrainLayerMask);
            mThinPlatformContactFilter = CreateContactFilter(mThinPlatformLayerMask);
            mGroundContactFilter = CreateContactFilter(mGroundLayerMask);

            mBody.OnTurn += (isRight) =>
            {
                mRender.SetFlipX(!isRight);
            };

            mHealth.OnDied += (_) =>
            {
                PlayDeathMotion();
            };
            mHealth.OnDamaged += (damageInfo, attacker) =>
            {
                if (damageInfo.Value > 0)
                {
                    PlayDamagedMotion();
                }
            };

            BindStates();
            ChangeState(PlayerState.Normal, true);
        }

        void Update()
        {
            UpdateBlockTimers();
            UpdateContactState();
            UpdateAnimatorParams();
        }

        public void PlayDamagedMotion()
        {
            if (IsCurrentState(PlayerState.Dead))
                return;

            ChangeState(PlayerState.Damaged, true);
        }

        public void PlayDeathMotion()
        {
            if (IsCurrentState(PlayerState.Dead))
                return;

            ChangeState(PlayerState.Dead);
        }

        public void OnDamaged()
        {
            PlayDamagedMotion();
        }

        public void OnDied()
        {
            PlayDeathMotion();
        }

        void BindStates()
        {
            FiniteStateBase normalState = new();
            normalState.EventEnter += EnterNormalState;
            normalState.EventUpdate += UpdateNormalState;
            normalState.EventLeave += LeaveNormalState;
            mFSM.SetDefaultState(normalState);
            mStates[PlayerState.Normal] = normalState;

            FiniteStateBase dashState = new();
            dashState.EventEnter += EnterDashState;
            dashState.EventLeave += LeaveDashState;
            mStates[PlayerState.Dashing] = dashState;

            FiniteStateBase damagedState = new();
            damagedState.EventEnter += EnterDamagedState;
            damagedState.EventLeave += LeaveDamagedState;
            mStates[PlayerState.Damaged] = damagedState;

            FiniteStateBase deathState = new();
            deathState.EventEnter += EnterDeathState;
            mStates[PlayerState.Dead] = deathState;
        }

        FiniteStateBase GetState(PlayerState state)
        {
            return mStates[state];
        }

        bool ChangeState(PlayerState state, bool forceChange = false)
        {
            return mFSM.TryChangeState(GetState(state), forceChange);
        }

        bool IsCurrentState(PlayerState state)
        {
            return mFSM.CurrentState == GetState(state);
        }

        void EnterNormalState()
        {
        }

        void UpdateNormalState()
        {
            if (!_EnableWallAttach)
                SetWallAttached(false);

            HandleDropDown();

            if (TryEnterDashState())
                return;

            HandleJump();
            if (_EnableWallAttach)
                HandleWallAttach();
            HandleMove();
        }

        void LeaveNormalState()
        {
            SetWallAttached(false);
        }

        void EnterDashState()
        {
            mAnim.CancelPreviousAnim(0);
            mAnim.CancelPreviousAnim(1);
            SetWallAttached(false);
            RunDashAsync().Forget();
        }

        void LeaveDashState()
        {
            mDashStartTime = Time.time;
            mPhy.LockGravity = false;
            mAnim.SetParamBool(AnimatorParams.LockNormal, false);
        }

        void EnterDamagedState()
        {
            mAnim.CancelPreviousAnim(0);
            mAnim.CancelPreviousAnim(1);
            InterruptDropDown();
            SetWallAttached(false);
            mPhy.LockGravity = false;
            mPhy.VelocityX = 0f;
            mAnim.SetParamBool(AnimatorParams.DoNextCombo, false);
            RunDamagedAsync().Forget();
        }

        void LeaveDamagedState()
        {
        }

        void EnterDeathState()
        {
            mAnim.CancelPreviousAnim(0);
            mAnim.CancelPreviousAnim(1);
            InterruptDropDown();
            SetWallAttached(false);
            mPhy.StopMoving();
            mPhy.LockGravity = false;
            mPlayerInput.LockPlayerInput = true;
            mAnim.SetParamBool(AnimatorParams.DoNextCombo, false);
            RunDeathAsync().Forget();
        }

        async UniTaskVoid RunDropDownAsync()
        {
            if (mIsDroppingDown || !IsCurrentState(PlayerState.Normal))
                return;

            if (!mIsGrounded || !CheckThinPlatformGrounded())
                return;

            int motionID = ++mDropDownMotionID;
            mIsDroppingDown = true;
            mDidDoubleJump = false;
            mBody.LockThinPlatform = true;
            mPhy.VelocityY = -_DropDownSpeed;

            float elapsedTime = 0f;
            while (motionID == mDropDownMotionID
                && !IsCurrentState(PlayerState.Dead)
                && elapsedTime < _DropDownMaxDuration
                && (elapsedTime < _DropDownMinDuration || CheckThinPlatformGrounded()))
            {
                elapsedTime += Time.deltaTime;
                await UniTask.Yield();
            }

            if (motionID == mDropDownMotionID)
            {
                mBody.LockThinPlatform = false;
                mIsDroppingDown = false;
            }
        }

        async UniTaskVoid RunDashAsync()
        {
            if (!IsCurrentState(PlayerState.Dashing))
                return;

            float moveX = GetMoveX();
            if (Mathf.Abs(moveX) > 0f)
                mBody.Turn(moveX);

            float dashSpeed = _DashSpeed;
            float dashDistance = _DashDistance;
            float dashDuration = dashDistance / dashSpeed;

            mPhy.LockGravity = true;
            mPhy.Velocity = new Vector2(mBody.FrontDirInt * dashSpeed, 0f);

            mAnim.SetParamBool(AnimatorParams.LockNormal, true);
            mAnim.PlayAnim(AnimStateNameHash.Dash);
            await UniTask.Delay((int)(dashDuration * 1000f));

            if (IsCurrentState(PlayerState.Dashing))
                ChangeState(PlayerState.Normal);
        }

        async UniTaskVoid RunDamagedAsync()
        {
            if (!IsCurrentState(PlayerState.Damaged))
                return;

            await mAnim.PlayAnimWaitEnd(AnimStateNameHash.Hit);

            if (IsCurrentState(PlayerState.Damaged))
                ChangeState(PlayerState.Normal);
        }

        async UniTaskVoid RunDeathAsync()
        {
            if (!IsCurrentState(PlayerState.Dead))
                return;

            await mAnim.PlayAnimWaitEnd(AnimStateNameHash.Death);
        }

        void InterruptDropDown()
        {
            ++mDropDownMotionID;
            mBody.LockThinPlatform = false;
            mIsDroppingDown = false;
        }

        void UpdateBlockTimers()
        {
            float deltaTime = Time.deltaTime;

            if (mWallAttachBlockTimer > 0f)
                mWallAttachBlockTimer -= deltaTime;
        }

        void UpdateContactState()
        {
            mIsGrounded = mIsDroppingDown ? CheckTerrainGrounded() : CheckGrounded();
            if (mIsGrounded)
                mDidDoubleJump = false;
        }

        void HandleMove()
        {
            if (mIsWallAttached)
                return;

            float moveX = GetMoveX();
            if (Mathf.Abs(moveX) > 0f)
            {
                mBody.Turn(moveX);
                mPhy.VelocityX = moveX * mSpec[SpecFields.MoveSpeed];
            }
            else
            {
                mPhy.VelocityX = 0f;
            }
        }

        void HandleJump()
        {
            if (!mPlayerInput.JustPressed(InputActionNameHash.Jump) || mPlayerInput.MoveY < -0.5f)
                return;

            if (_EnableWallAttach && mIsWallAttached)
            {
                DoWallJump();
                return;
            }

            if (mIsGrounded)
            {
                mDidDoubleJump = false;
                mWallAttachBlockTimer = _WallAttachBlockAfterJump;
                mPhy.DoJump(_JumpForce);
                mAnim.PlayAnim(AnimStateNameHash.Jump);
            }
            else if (!mDidDoubleJump)
            {
                mDidDoubleJump = true;
                mWallAttachBlockTimer = _WallAttachBlockAfterJump;
                mPhy.DoJump(_JumpForce);
                mAnim.PlayAnim(AnimStateNameHash.Jump);
            }
        }

        void HandleWallAttach()
        {
            bool canAttachWall = !mIsGrounded
                              && mPhy.VelocityY <= -_WallAttachMinFallSpeed
                              && mWallAttachBlockTimer <= 0f
                              && IsPressingToWall()
                              && CheckWall();

            if (!canAttachWall)
            {
                SetWallAttached(false);
                return;
            }

            SetWallAttached(true);
            mDidDoubleJump = false;
            mPhy.VelocityX = 0f;
            mPhy.VelocityY = -_WallSlideSpeed;
        }

        void SetWallAttached(bool isAttached)
        {
            if (mIsWallAttached == isAttached)
                return;

            mIsWallAttached = isAttached;
            mPhy.LockGravity = isAttached;
        }

        void DoWallJump()
        {
            int wallDir = mBody.FrontDirInt;
            int jumpDir = -wallDir;

            SetWallAttached(false);
            mWallAttachBlockTimer = _WallAttachBlockAfterJump;
            mDidDoubleJump = false;
            mBody.Turn(jumpDir);
            mPhy.Velocity = new Vector2(jumpDir * _WallJumpForce.x, _WallJumpForce.y);
            mAnim.PlayAnim(AnimStateNameHash.Jump);
        }

        bool TryEnterDashState()
        {
            if (!mPlayerInput.JustPressed(InputActionNameHash.Dash))
                return false;

            if (!MyUtils.IsCooltimeOver(mDashStartTime, _DashCooldown) || mIsDroppingDown)
                return false;

            return ChangeState(PlayerState.Dashing);
        }

        void HandleDropDown()
        {
            if (!mPlayerInput.JustPressed(InputActionNameHash.Jump) || mPlayerInput.MoveY >= -0.5f)
                return;

            RunDropDownAsync().Forget();
        }

        void UpdateAnimatorParams()
        {
            float moveX = GetMoveX();
            bool isMoving = Mathf.Abs(moveX) > 0f
                         && mIsGrounded
                         && !mIsWallAttached
                         && IsCurrentState(PlayerState.Normal);

            mAnim.SetParamFloat(AnimatorParams.MoveSpeed, Mathf.Abs(moveX));
            mAnim.SetParamBool(AnimatorParams.IsGrounded, mIsGrounded);
            mAnim.SetParamBool(AnimatorParams.IsMoving, isMoving);
            mAnim.SetParamBool(AnimatorParams.IsWallAttached, mIsWallAttached);
        }

        bool CheckGrounded()
        {
            Vector2 checkCenter = mBody.Foot.ExToVector2() + Vector2.down * _CheckDistance;
            return Physics2D.OverlapBox(checkCenter, _GroundCheckSize, 0f, mGroundContactFilter, mOverlapResults) > 0;
        }

        bool CheckTerrainGrounded()
        {
            Vector2 checkCenter = mBody.Foot.ExToVector2() + Vector2.down * _CheckDistance;
            return Physics2D.OverlapBox(checkCenter, _GroundCheckSize, 0f, mTerrainContactFilter, mOverlapResults) > 0;
        }

        bool CheckThinPlatformGrounded()
        {
            Vector2 checkCenter = mBody.Foot.ExToVector2() + Vector2.down * _CheckDistance;
            return Physics2D.OverlapBox(checkCenter, _GroundCheckSize, 0f, mThinPlatformContactFilter, mOverlapResults) > 0;
        }

        bool CheckWall()
        {
            Vector2 checkCenter = mBody.Center.ExToVector2() + Vector2.right * (mBody.FrontDirInt * (mBody.Size.x * 0.5f + _CheckDistance));
            return Physics2D.OverlapBox(checkCenter, _WallCheckSize, 0f, mTerrainContactFilter, mOverlapResults) > 0;
        }

        bool IsPressingToWall()
        {
            float moveX = GetMoveX();
            return Mathf.Abs(moveX) > 0f && Mathf.Sign(moveX) == mBody.FrontDirInt;
        }

        float GetMoveX()
        {
            float moveX = mPlayerInput.MoveX;
            if (Mathf.Abs(moveX) < 0.01f)
                return 0f;

            return Mathf.Sign(moveX);
        }

        int GetLayerMask(int layerID)
        {
            if (layerID < 0)
                return 0;

            return 1 << layerID;
        }

        ContactFilter2D CreateContactFilter(int layerMask)
        {
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.useTriggers = false;
            contactFilter.SetLayerMask(layerMask);
            return contactFilter;
        }

    }
}