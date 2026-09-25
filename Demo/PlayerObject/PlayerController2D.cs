using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PahlUnity.Demo
{
    public enum PlayerState
    {
        Normal,
        Dashing,
        Damaged,
        Dead,
    }

    /// <summary>
    /// 플레이어 캐릭터의 입력을 받아서 이동, 점프, 상호작용 등을 처리하는 컨트롤러 클래스
    /// </summary>
    public class PlayerController2D : MonoBehaviour
    {
        [SerializeField] private float _JumpForce = 12f;
        [SerializeField] private bool _EnableWallAttach = false;
        [SerializeField] private float _WallSlideSpeed = 1.5f;
        [SerializeField] private float _WallAttachMinFallSpeed = 0.5f;
        [SerializeField] private float _WallAttachBlockAfterJump = 0.25f;
        [SerializeField] private Vector2 _WallJumpForce = new Vector2(8f, 12f);
        [SerializeField] private float _DropDownSpeed = 3f;
        [SerializeField] private float _DropDownMinDuration = 0.35f;
        [SerializeField] private float _DropDownMaxDuration = 0.8f;
        [SerializeField] private Vector2 _GroundCheckSize = new Vector2(0.42f, 0.08f);
        [SerializeField] private Vector2 _WallCheckSize = new Vector2(0.08f, 1.3f);
        [SerializeField] private float _CheckDistance = 0.04f;

        BaseObject mBaseObj = null;
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

        public bool EnableWallAttach => _EnableWallAttach;
        public bool IsGrounded => mIsGrounded;
        public bool IsWallAttached => mIsWallAttached;
        public bool IsDroppingDown => mIsDroppingDown;

        void Awake()
        {
            mBaseObj = this.ExGetBase();
            mFSM = mBaseObj.FSM;
            if (mFSM == null)
                mFSM = mBaseObj.gameObject.AddComponent<FiniteStateMachine>();

            mBaseObj.Body2D.OnTurn += (isRight) =>
            {
                mBaseObj.Render.SetFlipX(!isRight);
            };

            mBaseObj.Health.OnDied += (_) =>
            {
                PlayDeathMotion();
            };
            mBaseObj.Health.OnDamaged += (damageInfo, attacker) =>
            {
                if (damageInfo.Value > 0)
                    PlayDamagedMotion();
            };

            mTerrainLayerMask = GetLayerMask(LayerID.Terrain);
            mThinPlatformLayerMask = GetLayerMask(LayerID.ThinPlatform);
            mGroundLayerMask = mTerrainLayerMask | mThinPlatformLayerMask;
            mTerrainContactFilter = CreateContactFilter(mTerrainLayerMask);
            mThinPlatformContactFilter = CreateContactFilter(mThinPlatformLayerMask);
            mGroundContactFilter = CreateContactFilter(mGroundLayerMask);

            BindStates();
        }

        void Update()
        {
            UpdateBlockTimers();
            UpdateContactState();
            UpdateAnimatorParams();
        }

        void Start()
        {
            ChangeState(PlayerState.Normal, true);
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
            FSMPlayerNormal normalState = GetComponentInChildren<FSMPlayerNormal>(true);
            FSMPlayerDash dashState = GetComponentInChildren<FSMPlayerDash>(true);
            FSMPlayerHit hitState = GetComponentInChildren<FSMPlayerHit>(true);
            FSMPlayerDeath deathState = GetComponentInChildren<FSMPlayerDeath>(true);

            mFSM.SetDefaultState(normalState);
            mStates[PlayerState.Normal] = normalState;
            mStates[PlayerState.Dashing] = dashState;
            mStates[PlayerState.Damaged] = hitState;
            mStates[PlayerState.Dead] = deathState;
        }

        public bool ChangeState(PlayerState state, bool forceChange = false)
        {
            return mFSM.TryChangeState(mStates[state], forceChange);
        }

        bool IsCurrentState(PlayerState state)
        {
            return mFSM.CurrentState == mStates[state];
        }

        public bool HandleDash()
        {
            if (!mBaseObj.Input.JustPressed(InputActionNameHash.Dash))
                return false;

            FSMPlayerDash dashState = mStates[PlayerState.Dashing] as FSMPlayerDash;
            if (!dashState.IsDashable() || mIsDroppingDown)
                return false;

            return ChangeState(PlayerState.Dashing);
        }

        public void DoMoveOnInput()
        {
            if (mIsWallAttached)
                return;

            float moveX = GetMoveX();
            if (Mathf.Abs(moveX) > 0f)
            {
                mBaseObj.Body2D.Turn(moveX);
                mBaseObj.Physics2D.VelocityX = moveX * mBaseObj.Spec[SpecFields.MoveSpeed];
            }
            else
            {
                mBaseObj.Physics2D.VelocityX = 0f;
            }
        }

        public void HandleJump()
        {
            if (!mBaseObj.Input.JustPressed(InputActionNameHash.Jump) || mBaseObj.Input.MoveY < -0.5f)
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
                mBaseObj.Physics2D.DoJump(_JumpForce);
                mBaseObj.Anim.PlayAnim(AnimStateNameHash.Jump);
            }
            else if (!mDidDoubleJump)
            {
                mDidDoubleJump = true;
                mWallAttachBlockTimer = _WallAttachBlockAfterJump;
                mBaseObj.Physics2D.DoJump(_JumpForce);
                mBaseObj.Anim.PlayAnim(AnimStateNameHash.Jump);
            }
        }

        public void HandleWallAttach()
        {
            bool canAttachWall = !mIsGrounded
                              && mBaseObj.Physics2D.VelocityY <= -_WallAttachMinFallSpeed
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
            mBaseObj.Physics2D.VelocityX = 0f;
            mBaseObj.Physics2D.VelocityY = -_WallSlideSpeed;
        }

        public void HandleDropDown()
        {
            if (!mBaseObj.Input.JustPressed(InputActionNameHash.Jump) || mBaseObj.Input.MoveY >= -0.5f)
                return;

            RunDropDownAsync().Forget();
        }

        public void SetWallAttached(bool isAttached)
        {
            if (mIsWallAttached == isAttached)
                return;

            mIsWallAttached = isAttached;
            mBaseObj.Physics2D.LockGravity = isAttached;
        }

        public void InterruptDropDown()
        {
            ++mDropDownMotionID;
            mBaseObj.Body2D.LockThinPlatform = false;
            mIsDroppingDown = false;
        }

        void DoWallJump()
        {
            int wallDir = mBaseObj.Body2D.FrontDirInt;
            int jumpDir = -wallDir;

            SetWallAttached(false);
            mWallAttachBlockTimer = _WallAttachBlockAfterJump;
            mDidDoubleJump = false;
            mBaseObj.Body2D.Turn(jumpDir);
            mBaseObj.Physics2D.Velocity = new Vector2(jumpDir * _WallJumpForce.x, _WallJumpForce.y);
            mBaseObj.Anim.PlayAnim(AnimStateNameHash.Jump);
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
            mBaseObj.Body2D.LockThinPlatform = true;
            mBaseObj.Physics2D.VelocityY = -_DropDownSpeed;

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
                mBaseObj.Body2D.LockThinPlatform = false;
                mIsDroppingDown = false;
            }
        }

        void UpdateBlockTimers()
        {
            if (mWallAttachBlockTimer > 0f)
                mWallAttachBlockTimer -= Time.deltaTime;
        }

        void UpdateContactState()
        {
            mIsGrounded = mIsDroppingDown ? CheckTerrainGrounded() : CheckGrounded();
            if (mIsGrounded)
                mDidDoubleJump = false;
        }

        void UpdateAnimatorParams()
        {
            float moveX = GetMoveX();
            bool isMoving = Mathf.Abs(moveX) > 0f
                         && mIsGrounded
                         && !mIsWallAttached
                         && IsCurrentState(PlayerState.Normal);

            mBaseObj.Anim.SetParamFloat(AnimatorParams.MoveSpeed, Mathf.Abs(moveX));
            mBaseObj.Anim.SetParamBool(AnimatorParams.IsGrounded, mIsGrounded);
            mBaseObj.Anim.SetParamBool(AnimatorParams.IsMoving, isMoving);
            mBaseObj.Anim.SetParamBool(AnimatorParams.IsWallAttached, mIsWallAttached);
        }

        bool CheckGrounded()
        {
            Vector2 checkCenter = mBaseObj.Body2D.Foot.ExToVector2() + Vector2.down * _CheckDistance;
            return Physics2D.OverlapBox(checkCenter, _GroundCheckSize, 0f, mGroundContactFilter, mOverlapResults) > 0;
        }

        bool CheckTerrainGrounded()
        {
            Vector2 checkCenter = mBaseObj.Body2D.Foot.ExToVector2() + Vector2.down * _CheckDistance;
            return Physics2D.OverlapBox(checkCenter, _GroundCheckSize, 0f, mTerrainContactFilter, mOverlapResults) > 0;
        }

        bool CheckThinPlatformGrounded()
        {
            Vector2 checkCenter = mBaseObj.Body2D.Foot.ExToVector2() + Vector2.down * _CheckDistance;
            return Physics2D.OverlapBox(checkCenter, _GroundCheckSize, 0f, mThinPlatformContactFilter, mOverlapResults) > 0;
        }

        bool CheckWall()
        {
            Vector2 checkCenter = mBaseObj.Body2D.Center.ExToVector2() + Vector2.right * (mBaseObj.Body2D.FrontDirInt * (mBaseObj.Body2D.Size.x * 0.5f + _CheckDistance));
            return Physics2D.OverlapBox(checkCenter, _WallCheckSize, 0f, mTerrainContactFilter, mOverlapResults) > 0;
        }

        bool IsPressingToWall()
        {
            float moveX = GetMoveX();
            return Mathf.Abs(moveX) > 0f && Mathf.Sign(moveX) == mBaseObj.Body2D.FrontDirInt;
        }

        float GetMoveX()
        {
            float moveX = mBaseObj.Input.MoveX;
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
