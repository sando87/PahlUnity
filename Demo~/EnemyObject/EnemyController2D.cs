using System.Collections.Generic;
using UnityEngine;

namespace PahlUnity.Demo
{
    public enum EnemyState
    {
        Idle,
        Patrol,
        Detect,
        Chase,
        Attack,
        Damaged,
        Death,
    }

    public class EnemyController2D : MonoBehaviour
    {
        [SerializeField] private float _DetectRange = 6f;
        [SerializeField] private float _LoseRange = 8f;
        [SerializeField] private float _AttackRange = 1.2f;
        [SerializeField] private Vector2 _GroundCheckSize = new Vector2(0.42f, 0.08f);
        [SerializeField] private Vector2 _WallCheckSize = new Vector2(0.08f, 1.3f);
        [SerializeField] private float _CheckDistance = 0.08f;
        [SerializeField] private float _GroundCheckMinHeight = 0.2f;
        [SerializeField] private float _SameHeightTolerance = 2f;

        BaseObject mBaseObj = null;
        FiniteStateMachine mFSM = null;
        Health mPlayerHealth = null;
        ObjectBody2D mPlayerBody = null;
        Transform mPlayerTransform = null;
        Dictionary<EnemyState, FiniteStateBase> mStates = new();

        readonly Collider2D[] mOverlapResults = new Collider2D[4];

        int mTerrainLayerMask = 0;
        int mGroundLayerMask = 0;
        ContactFilter2D mTerrainContactFilter;
        ContactFilter2D mGroundContactFilter;
        bool mIsGrounded = false;
        float mNextAttackTime = 0f;
        float mStateEnterTime = 0f;
        bool mIsStateStarted = false;
        EnemyState mCurrentState = EnemyState.Idle;

        public float StateEnterTime => mStateEnterTime;

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

            mTerrainLayerMask = GetLayerMask(LayerID.Terrain);
            mGroundLayerMask = mTerrainLayerMask | GetLayerMask(LayerID.ThinPlatform);
            mTerrainContactFilter = CreateContactFilter(mTerrainLayerMask);
            mGroundContactFilter = CreateContactFilter(mGroundLayerMask);

            CachePlayer();
            BindHealthEvents();
            BindStates();
        }

        void Start()
        {
            CachePlayer();
            ChangeState(EnemyState.Idle, true);
            mIsStateStarted = true;
        }

        void Update()
        {
            if (!mIsStateStarted)
                return;

            if (mPlayerTransform == null || mPlayerHealth == null)
                CachePlayer();

            UpdateContactState();
            UpdateAnimatorParams();
        }

        public void PlayDamagedMotion()
        {
            if (IsCurrentState(EnemyState.Death))
                return;

            ChangeState(EnemyState.Damaged, true);
        }

        public void PlayDeathMotion()
        {
            if (IsCurrentState(EnemyState.Death))
                return;

            ChangeState(EnemyState.Death);
        }

        public bool ChangeState(EnemyState state, bool forceChange = false)
        {
            if (!forceChange && IsCurrentState(state))
                return false;

            EnemyState previousState = mCurrentState;
            float previousStateEnterTime = mStateEnterTime;
            mCurrentState = state;
            mStateEnterTime = Time.time;

            if (!mFSM.TryChangeState(mStates[state], forceChange))
            {
                mCurrentState = previousState;
                mStateEnterTime = previousStateEnterTime;
                return false;
            }

            return true;
        }

        public bool IsCurrentState(EnemyState state)
        {
            return mCurrentState == state;
        }

        public void StopHorizontal()
        {
            mBaseObj.Physics2D.VelocityX = 0f;
        }

        public void StopMoving()
        {
            mBaseObj.Physics2D.StopMoving();
            mBaseObj.Physics2D.LockGravity = false;
        }

        public void Turn(int dir)
        {
            if (dir != 0)
                mBaseObj.Body2D.Turn(dir);
        }

        public void MoveHorizontal(int dir, float speed)
        {
            if (dir == 0)
            {
                StopHorizontal();
                return;
            }

            mBaseObj.Body2D.Turn(dir);
            mBaseObj.Physics2D.VelocityX = dir * speed;
        }

        public void TurnToPlayer()
        {
            Turn(GetDirToPlayer());
        }

        public int GetDirToPlayer()
        {
            if (mPlayerTransform == null)
                return 0;

            float deltaX = GetPlayerCenter().x - mBaseObj.Body2D.Center.x;
            if (Mathf.Abs(deltaX) < 0.05f)
                return 0;

            return deltaX > 0f ? 1 : -1;
        }

        public bool TryDetectPlayer()
        {
            if (!IsPlayerInDetectRange())
                return false;

            ChangeState(EnemyState.Detect);
            return true;
        }

        public bool IsPlayerInDetectRange()
        {
            if (mPlayerTransform == null || mPlayerHealth == null || mPlayerHealth.IsDead)
                return false;

            Vector2 toPlayer = GetPlayerCenter() - mBaseObj.Body2D.Center;
            if (Mathf.Abs(toPlayer.y) > _SameHeightTolerance)
                return false;

            if (Mathf.Abs(toPlayer.x) > _DetectRange)
                return false;

            return true;
        }

        public bool IsPlayerLost()
        {
            if (mPlayerTransform == null)
                return true;

            Vector2 toPlayer = GetPlayerCenter() - mBaseObj.Body2D.Center;
            return Mathf.Abs(toPlayer.x) > _LoseRange || Mathf.Abs(toPlayer.y) > _SameHeightTolerance;
        }

        public bool IsPlayerInAttackRange()
        {
            if (mPlayerTransform == null || mPlayerHealth == null || mPlayerHealth.IsDead)
                return false;

            Vector2 toPlayer = GetPlayerCenter() - mBaseObj.Body2D.Center;
            return Mathf.Abs(toPlayer.x) <= _AttackRange
                && Mathf.Abs(toPlayer.y) <= _SameHeightTolerance;
        }

        public bool CanAttack()
        {
            return Time.time >= mNextAttackTime;
        }

        public void SetAttackCooldown(float duration)
        {
            mNextAttackTime = Time.time + duration;
        }

        public bool HasGroundAhead()
        {
            return HasGroundAhead(mBaseObj.Body2D.FrontDirInt);
        }

        public bool HasGroundAhead(int dir)
        {
            Vector2 checkPosition = mBaseObj.Body2D.Foot.ExToVector2() + Vector2.right * (dir * (mBaseObj.Body2D.Size.x * 0.5f + _CheckDistance));
            return CheckGroundAt(checkPosition);
        }

        public bool HasWallAhead()
        {
            return HasWallAhead(mBaseObj.Body2D.FrontDirInt);
        }

        public bool HasWallAhead(int dir)
        {
            Vector2 checkCenter = mBaseObj.Body2D.Center.ExToVector2() + Vector2.right * (dir * (mBaseObj.Body2D.Size.x * 0.5f + _CheckDistance));
            return Physics2D.OverlapBox(checkCenter, _WallCheckSize, 0f, mTerrainContactFilter, mOverlapResults) > 0;
        }

        void BindStates()
        {
            FSMIdle idleState = GetComponentInChildren<FSMIdle>(true);
            FSMPatrol patrolState = GetComponentInChildren<FSMPatrol>(true);
            FSMDetect detectState = GetComponentInChildren<FSMDetect>(true);
            FSMChase chaseState = GetComponentInChildren<FSMChase>(true);
            FSMAttack attackState = GetComponentInChildren<FSMAttack>(true);
            FSMDamaged damagedState = GetComponentInChildren<FSMDamaged>(true);
            FSMDeath deathState = GetComponentInChildren<FSMDeath>(true);

            mFSM.SetDefaultState(idleState);
            mStates[EnemyState.Idle] = idleState;
            mStates[EnemyState.Patrol] = patrolState;
            mStates[EnemyState.Detect] = detectState;
            mStates[EnemyState.Chase] = chaseState;
            mStates[EnemyState.Attack] = attackState;
            mStates[EnemyState.Damaged] = damagedState;
            mStates[EnemyState.Death] = deathState;
        }

        void CachePlayer()
        {
            if (InGameManager.Instance.Engine == null || InGameManager.Instance.Engine.Player == null)
                return;

            PlayerObject player = InGameManager.Instance.Engine.Player;
            mPlayerTransform = player.transform;
            mPlayerBody = player.ExGetBase().Body2D;
            mPlayerHealth = player.ExGetBase().Health;
        }

        void BindHealthEvents()
        {
            if (mBaseObj.Health == null)
                return;

            mBaseObj.Health.OnDied += (_) =>
            {
                PlayDeathMotion();
            };
            mBaseObj.Health.OnDamaged += (damage, attacker) =>
            {
                if (damage.Value > 0)
                    PlayDamagedMotion();
            };
        }

        void UpdateContactState()
        {
            mIsGrounded = CheckGroundAt(mBaseObj.Body2D.Foot);
        }

        void UpdateAnimatorParams()
        {
            float velocityX = mBaseObj.Physics2D.VelocityX;
            bool isMoving = Mathf.Abs(velocityX) > 0.01f
                         && mIsGrounded
                         && (IsCurrentState(EnemyState.Patrol) || IsCurrentState(EnemyState.Chase));

            mBaseObj.Anim.SetParamFloat(AnimatorParams.MoveSpeed, Mathf.Abs(velocityX));
            mBaseObj.Anim.SetParamBool(AnimatorParams.IsGrounded, mIsGrounded);
            mBaseObj.Anim.SetParamBool(AnimatorParams.IsMoving, isMoving);
        }

        bool CheckGroundAt(Vector2 footPosition)
        {
            Vector2 checkSize = new Vector2(_GroundCheckSize.x, Mathf.Max(_GroundCheckSize.y, _GroundCheckMinHeight));
            Vector2 checkCenter = footPosition + Vector2.down * (_CheckDistance * 0.5f);
            return Physics2D.OverlapBox(checkCenter, checkSize, 0f, mGroundContactFilter, mOverlapResults) > 0;
        }

        Vector3 GetPlayerCenter()
        {
            if (mPlayerBody != null)
                return mPlayerBody.Center;

            if (mPlayerTransform != null)
                return mPlayerTransform.position;

            return mBaseObj.Body2D.Center;
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
