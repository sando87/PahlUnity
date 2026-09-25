using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PahlUnity.Demo
{
    public class FSMPlayerNormal : FiniteStateBase
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

        BaseObject mBase;
        PlayerController2D mPlayerCtrl;
        FSMPlayerDash mDash;
        FSMPlayerDeath mDeath;

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

        public bool IsDroppingDown => mIsDroppingDown;

        void Awake()
        {
            mBase = this.ExGetBase();
            mPlayerCtrl = mBase.GetComp<PlayerController2D>();
            mDash = mBase.GetComp<FSMPlayerDash>();
            mDeath = mBase.GetComp<FSMPlayerDeath>();

            mTerrainLayerMask = GetLayerMask(LayerID.Terrain);
            mThinPlatformLayerMask = GetLayerMask(LayerID.ThinPlatform);
            mGroundLayerMask = mTerrainLayerMask | mThinPlatformLayerMask;
            mTerrainContactFilter = CreateContactFilter(mTerrainLayerMask);
            mThinPlatformContactFilter = CreateContactFilter(mThinPlatformLayerMask);
            mGroundContactFilter = CreateContactFilter(mGroundLayerMask);
        }

        public override void UpdateState()
        {
            base.UpdateState();

            UpdateBlockTimers();
            UpdateContactState();
            UpdateAnimatorParams();

            if (!_EnableWallAttach)
                SetWallAttached(false);

            HandleDropDown();

            if (mPlayerCtrl.HandleDash())
                return;

            HandleJump();
            if (_EnableWallAttach)
                HandleWallAttach();
            HandleMove();
        }

        public override void LeaveState()
        {
            base.LeaveState();
            SetWallAttached(false);
        }

        public void SetWallAttached(bool isAttached)
        {
            if (mIsWallAttached == isAttached)
                return;

            mIsWallAttached = isAttached;
            mBase.Physics2D.LockGravity = isAttached;
        }

        public void InterruptDropDown()
        {
            ++mDropDownMotionID;
            mBase.Body2D.LockThinPlatform = false;
            mIsDroppingDown = false;
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

        void HandleMove()
        {
            if (mIsWallAttached)
                return;

            float moveX = GetMoveX();
            if (Mathf.Abs(moveX) > 0f)
            {
                mBase.Body2D.Turn(moveX);
                mBase.Physics2D.VelocityX = moveX * mBase.Spec[SpecFields.MoveSpeed];
            }
            else
            {
                mBase.Physics2D.VelocityX = 0f;
            }
        }

        void HandleJump()
        {
            if (!mBase.Input.JustPressed(InputActionNameHash.Jump) || mBase.Input.MoveY < -0.5f)
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
                mBase.Physics2D.DoJump(_JumpForce);
                mBase.Anim.PlayAnim(AnimStateNameHash.Jump);
            }
            else if (!mDidDoubleJump)
            {
                mDidDoubleJump = true;
                mWallAttachBlockTimer = _WallAttachBlockAfterJump;
                mBase.Physics2D.DoJump(_JumpForce);
                mBase.Anim.PlayAnim(AnimStateNameHash.Jump);
            }
        }

        void HandleWallAttach()
        {
            bool canAttachWall = !mIsGrounded
                              && mBase.Physics2D.VelocityY <= -_WallAttachMinFallSpeed
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
            mBase.Physics2D.VelocityX = 0f;
            mBase.Physics2D.VelocityY = -_WallSlideSpeed;
        }

        void DoWallJump()
        {
            int wallDir = mBase.Body2D.FrontDirInt;
            int jumpDir = -wallDir;

            SetWallAttached(false);
            mWallAttachBlockTimer = _WallAttachBlockAfterJump;
            mDidDoubleJump = false;
            mBase.Body2D.Turn(jumpDir);
            mBase.Physics2D.Velocity = new Vector2(jumpDir * _WallJumpForce.x, _WallJumpForce.y);
            mBase.Anim.PlayAnim(AnimStateNameHash.Jump);
        }

        void HandleDropDown()
        {
            if (!mBase.Input.JustPressed(InputActionNameHash.Jump) || mBase.Input.MoveY >= -0.5f)
                return;

            RunDropDownAsync().Forget();
        }

        async UniTaskVoid RunDropDownAsync()
        {
            if (mIsDroppingDown || mBase.FSM.CurrentState != this)
                return;

            if (!mIsGrounded || !CheckThinPlatformGrounded())
                return;

            int motionID = ++mDropDownMotionID;
            mIsDroppingDown = true;
            mDidDoubleJump = false;
            mBase.Body2D.LockThinPlatform = true;
            mBase.Physics2D.VelocityY = -_DropDownSpeed;

            float elapsedTime = 0f;
            while (motionID == mDropDownMotionID
                && mBase.FSM.CurrentState != mDeath
                && elapsedTime < _DropDownMaxDuration
                && (elapsedTime < _DropDownMinDuration || CheckThinPlatformGrounded()))
            {
                elapsedTime += Time.deltaTime;
                await UniTask.Yield();
            }

            if (motionID == mDropDownMotionID)
            {
                mBase.Body2D.LockThinPlatform = false;
                mIsDroppingDown = false;
            }
        }

        void UpdateAnimatorParams()
        {
            float moveX = GetMoveX();
            bool isMoving = Mathf.Abs(moveX) > 0f
                         && mIsGrounded
                         && !mIsWallAttached
                         && mBase.FSM.CurrentState == this;

            mBase.Anim.SetParamFloat(AnimatorParams.MoveSpeed, Mathf.Abs(moveX));
            mBase.Anim.SetParamBool(AnimatorParams.IsGrounded, mIsGrounded);
            mBase.Anim.SetParamBool(AnimatorParams.IsMoving, isMoving);
            mBase.Anim.SetParamBool(AnimatorParams.IsWallAttached, mIsWallAttached);
        }

        bool CheckGrounded()
        {
            Vector2 checkCenter = mBase.Body2D.Foot.ExToVector2() + Vector2.down * _CheckDistance;
            return Physics2D.OverlapBox(checkCenter, _GroundCheckSize, 0f, mGroundContactFilter, mOverlapResults) > 0;
        }

        bool CheckTerrainGrounded()
        {
            Vector2 checkCenter = mBase.Body2D.Foot.ExToVector2() + Vector2.down * _CheckDistance;
            return Physics2D.OverlapBox(checkCenter, _GroundCheckSize, 0f, mTerrainContactFilter, mOverlapResults) > 0;
        }

        bool CheckThinPlatformGrounded()
        {
            Vector2 checkCenter = mBase.Body2D.Foot.ExToVector2() + Vector2.down * _CheckDistance;
            return Physics2D.OverlapBox(checkCenter, _GroundCheckSize, 0f, mThinPlatformContactFilter, mOverlapResults) > 0;
        }

        bool CheckWall()
        {
            Vector2 checkCenter = mBase.Body2D.Center.ExToVector2() + Vector2.right * (mBase.Body2D.FrontDirInt * (mBase.Body2D.Size.x * 0.5f + _CheckDistance));
            return Physics2D.OverlapBox(checkCenter, _WallCheckSize, 0f, mTerrainContactFilter, mOverlapResults) > 0;
        }

        bool IsPressingToWall()
        {
            float moveX = GetMoveX();
            return Mathf.Abs(moveX) > 0f && Mathf.Sign(moveX) == mBase.Body2D.FrontDirInt;
        }

        float GetMoveX()
        {
            float moveX = mBase.Input.MoveX;
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
