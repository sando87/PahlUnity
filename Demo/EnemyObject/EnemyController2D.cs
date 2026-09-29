using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PahlUnity;
using UnityEngine;

namespace PahlUnity.Demo
{
    public class EnemyController2D : MonoBehaviour
    {
        enum EnemyState
        {
            Idle,
            Patrol,
            Detect,
            Chase,
            Attack,
            Damaged,
            Death,
        }

        [SerializeField] private float _PatrolDistance = 4f;
        [SerializeField] private float _IdleDuration = 1f;
        [SerializeField] private float _DetectDuration = 0.35f;
        [SerializeField] private float _DetectRange = 6f;
        [SerializeField] private float _LoseRange = 8f;
        [SerializeField] private float _AttackRange = 1.2f;
        [SerializeField] private Vector2 _GroundCheckSize = new Vector2(0.42f, 0.08f);
        [SerializeField] private Vector2 _WallCheckSize = new Vector2(0.08f, 1.3f);
        [SerializeField] private float _CheckDistance = 0.08f;
        [SerializeField] private float _GroundCheckMinHeight = 0.2f;
        [SerializeField] private float _PatrolTurnCooldown = 0.35f;
        [SerializeField] private float _SameHeightTolerance = 2f;
        [SerializeField] private ProjectileBase _ProjectilePrefab = null;

        BaseObject mBaseObj = null;
        ObjectPhysics2D mPhy = null;
        ObjectBody2D mBody = null;
        AnimatorHelper mAnim = null;
        Health mHealth = null;
        Health mPlayerHealth = null;
        ObjectBody2D mPlayerBody = null;
        Transform mPlayerTransform = null;
        FiniteStateMachine mFSM = null;
        SpecBase mSpec = null;
        Dictionary<EnemyState, FiniteStateBase> mStates = new();

        readonly Collider2D[] mOverlapResults = new Collider2D[4];

        int mTerrainLayerMask = 0;
        int mGroundLayerMask = 0;
        ContactFilter2D mTerrainContactFilter;
        ContactFilter2D mGroundContactFilter;
        Vector2 mSpawnPosition = Vector2.zero;
        bool mIsGrounded = false;
        int mPatrolDir = 1;
        int mAttackMotionID = 0;
        float mNextAttackTime = 0f;
        float mStateEnterTime = 0f;
        float mNextPatrolTurnTime = 0f;
        bool mIsStateStarted = false;
        EnemyState mCurrentState = EnemyState.Idle;

        void Awake()
        {
            mBaseObj = this.ExGetBase();
            mPhy = mBaseObj.GetComp<ObjectPhysics2D>();
            mBody = mBaseObj.GetComp<ObjectBody2D>();
            mAnim = mBaseObj.GetComp<AnimatorHelper>();
            mHealth = mBaseObj.GetComp<Health>();
            mSpec = mBaseObj.GetComp<SpecBase>();
            mFSM = mBaseObj.GetComp<FiniteStateMachine>();
            if (mFSM == null)
                mFSM = mBaseObj.gameObject.AddComponent<FiniteStateMachine>();

            mTerrainLayerMask = GetLayerMask(LayerID.Terrain);
            mGroundLayerMask = mTerrainLayerMask | GetLayerMask(LayerID.ThinPlatform);
            mTerrainContactFilter = CreateContactFilter(mTerrainLayerMask);
            mGroundContactFilter = CreateContactFilter(mGroundLayerMask);
            mSpawnPosition = transform.position;

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
            if (mHealth == null)
                return;

            mHealth.OnDied += (_) =>
            {
                PlayDeathMotion();
            };
            mHealth.OnDamaged += (_damage, attacker) =>
            {
                if (_damage.Value > 0)
                    PlayDamagedMotion();
            };
        }

        void BindStates()
        {
            FiniteStateBase idleState = new();
            idleState.EventEnter += EnterIdleState;
            idleState.EventUpdate += UpdateIdleState;
            idleState.EventLeave += LeaveMoveState;
            mFSM.SetDefaultState(idleState);
            mStates[EnemyState.Idle] = idleState;

            FiniteStateBase patrolState = new();
            patrolState.EventEnter += EnterPatrolState;
            patrolState.EventUpdate += UpdatePatrolState;
            patrolState.EventLeave += LeaveMoveState;
            mStates[EnemyState.Patrol] = patrolState;

            FiniteStateBase detectState = new();
            detectState.EventEnter += EnterDetectState;
            detectState.EventUpdate += UpdateDetectState;
            detectState.EventLeave += LeaveMoveState;
            mStates[EnemyState.Detect] = detectState;

            FiniteStateBase chaseState = new();
            chaseState.EventEnter += EnterChaseState;
            chaseState.EventUpdate += UpdateChaseState;
            chaseState.EventLeave += LeaveMoveState;
            mStates[EnemyState.Chase] = chaseState;

            FiniteStateBase attackState = new();
            attackState.EventEnter += EnterAttackState;
            attackState.EventLeave += LeaveAttackState;
            mStates[EnemyState.Attack] = attackState;

            FiniteStateBase damagedState = new();
            damagedState.EventEnter += EnterDamagedState;
            mStates[EnemyState.Damaged] = damagedState;

            FiniteStateBase deathState = new();
            deathState.EventEnter += EnterDeathState;
            mStates[EnemyState.Death] = deathState;
        }

        FiniteStateBase GetState(EnemyState state)
        {
            return mStates[state];
        }

        bool ChangeState(EnemyState state, bool forceChange = false)
        {
            if (!forceChange && IsCurrentState(state))
                return false;

            EnemyState previousState = mCurrentState;
            float previousStateEnterTime = mStateEnterTime;
            mCurrentState = state;
            mStateEnterTime = Time.time;

            if (!mFSM.TryChangeState(GetState(state), forceChange))
            {
                mCurrentState = previousState;
                mStateEnterTime = previousStateEnterTime;
                return false;
            }

            return true;
        }

        bool IsCurrentState(EnemyState state)
        {
            return mCurrentState == state;
        }

        void EnterIdleState()
        {
            mPhy.VelocityX = 0f;
        }

        void UpdateIdleState()
        {
            if (TryDetectPlayer())
                return;

            if (Time.time >= GetStateStartTime(EnemyState.Idle) + _IdleDuration)
                ChangeState(EnemyState.Patrol);
        }

        void EnterPatrolState()
        {
            if (mPatrolDir == 0)
                mPatrolDir = 1;
            mBody.Turn(mPatrolDir);
        }

        void UpdatePatrolState()
        {
            if (TryDetectPlayer())
                return;

            if (CanTurnPatrol() && ShouldTurnPatrol())
                TurnPatrol();

            MoveHorizontal(mPatrolDir, mSpec[SpecFields.MoveSpeed]);
        }

        void EnterDetectState()
        {
            mPhy.VelocityX = 0f;
            TurnToPlayer();
        }

        void UpdateDetectState()
        {
            TurnToPlayer();

            if (!IsPlayerInDetectRange())
            {
                ChangeState(EnemyState.Patrol);
                return;
            }

            if (Time.time >= GetStateStartTime(EnemyState.Detect) + _DetectDuration)
                ChangeState(CanAttack() && IsPlayerInAttackRange() ? EnemyState.Attack : EnemyState.Chase);
        }

        void EnterChaseState()
        {
            TurnToPlayer();
        }

        void UpdateChaseState()
        {
            if (!IsPlayerInDetectRange() || IsPlayerLost())
            {
                ChangeState(EnemyState.Patrol);
                return;
            }

            if (IsPlayerInAttackRange())
            {
                mPhy.VelocityX = 0f;
                if (CanAttack())
                    ChangeState(EnemyState.Attack);
                return;
            }

            int moveDir = GetDirToPlayer();
            if (moveDir == 0 || !HasGroundAhead(moveDir) || HasWallAhead(moveDir))
            {
                mPhy.VelocityX = 0f;
                return;
            }

            MoveHorizontal(moveDir, mSpec[SpecFields.MoveSpeed] * 1.5f);
        }

        void EnterAttackState()
        {
            ++mAttackMotionID;
            mPhy.VelocityX = 0f;
            TurnToPlayer();
            mAnim.PlayAnim(AnimStateNameHash.Attack, OnFireAttackAnim, OnEndAttackAnim);
        }

        void OnFireAttackAnim(int idx)
        {
            TryDamagePlayer();
            mNextAttackTime = Time.time + 1.5f; //mSpec[SpecFields.Cooltime];
        }

        void OnEndAttackAnim()
        {
            if (IsPlayerInDetectRange())
                ChangeState(EnemyState.Chase);
            else
                ChangeState(EnemyState.Patrol);
        }

        void LeaveAttackState()
        {
            ++mAttackMotionID;
            mAnim.SetParamFloat(AnimatorParams.AttackSpeed, 1f);
        }

        void EnterDamagedState()
        {
            ++mAttackMotionID;
            mPhy.VelocityX = 0f;
            mAnim.PlayAnim(AnimStateNameHash.Hit, null, () =>
            {
                ChangeState(IsPlayerInDetectRange() ? EnemyState.Chase : EnemyState.Patrol);
            });
        }

        void EnterDeathState()
        {
            ++mAttackMotionID;
            mPhy.StopMoving();
            mPhy.LockGravity = false;
            mBody.LockBody = true;
            mAnim.PlayAnim(AnimStateNameHash.Death);
        }

        void LeaveMoveState()
        {
            mPhy.VelocityX = 0f;
        }

        bool TryDetectPlayer()
        {
            if (!IsPlayerInDetectRange())
                return false;

            ChangeState(EnemyState.Detect);
            return true;
        }

        bool IsPlayerInDetectRange()
        {
            if (mPlayerTransform == null || mPlayerHealth == null || mPlayerHealth.IsDead)
                return false;

            Vector2 toPlayer = GetPlayerCenter() - mBody.Center;
            if (Mathf.Abs(toPlayer.y) > _SameHeightTolerance)
                return false;

            if (Mathf.Abs(toPlayer.x) > _DetectRange)
                return false;

            return true;
        }

        bool IsPlayerLost()
        {
            if (mPlayerTransform == null)
                return true;

            Vector2 toPlayer = GetPlayerCenter() - mBody.Center;
            return Mathf.Abs(toPlayer.x) > _LoseRange || Mathf.Abs(toPlayer.y) > _SameHeightTolerance;
        }

        bool IsPlayerInAttackRange()
        {
            if (mPlayerTransform == null || mPlayerHealth == null || mPlayerHealth.IsDead)
                return false;

            Vector2 toPlayer = GetPlayerCenter() - mBody.Center;
            return Mathf.Abs(toPlayer.x) <= _AttackRange
                && Mathf.Abs(toPlayer.y) <= _SameHeightTolerance;
        }

        bool CanAttack()
        {
            return Time.time >= mNextAttackTime;
        }

        void TryDamagePlayer()
        {
            DoAttackFire();
        }

        void TurnToPlayer()
        {
            int dir = GetDirToPlayer();
            if (dir != 0)
                mBody.Turn(dir);
        }

        int GetDirToPlayer()
        {
            if (mPlayerTransform == null)
                return 0;

            float deltaX = GetPlayerCenter().x - mBody.Center.x;
            if (Mathf.Abs(deltaX) < 0.05f)
                return 0;

            return deltaX > 0f ? 1 : -1;
        }

        void MoveHorizontal(int dir, float speed)
        {
            if (dir == 0)
            {
                mPhy.VelocityX = 0f;
                return;
            }

            mBody.Turn(dir);
            mPhy.VelocityX = dir * speed;
        }

        bool ReachedPatrolEdge()
        {
            float offsetX = transform.position.x - mSpawnPosition.x;
            return offsetX >= _PatrolDistance && mPatrolDir > 0
                || offsetX <= -_PatrolDistance && mPatrolDir < 0;
        }

        bool CanTurnPatrol()
        {
            return Time.time >= mNextPatrolTurnTime;
        }

        bool ShouldTurnPatrol()
        {
            return ReachedPatrolEdge() || !HasGroundAhead() || HasWallAhead();
        }

        void TurnPatrol()
        {
            mPatrolDir *= -1;
            mNextPatrolTurnTime = Time.time + _PatrolTurnCooldown;
            mBody.Turn(mPatrolDir);
        }

        void UpdateContactState()
        {
            mIsGrounded = CheckGrounded();
        }

        void UpdateAnimatorParams()
        {
            bool isMoving = Mathf.Abs(mPhy.VelocityX) > 0.01f
                         && mIsGrounded
                         && (IsCurrentState(EnemyState.Patrol) || IsCurrentState(EnemyState.Chase));

            mAnim.SetParamFloat(AnimatorParams.MoveSpeed, Mathf.Abs(mPhy.VelocityX));
            mAnim.SetParamBool(AnimatorParams.IsGrounded, mIsGrounded);
            mAnim.SetParamBool(AnimatorParams.IsMoving, isMoving);
        }

        bool CheckGrounded()
        {
            return CheckGroundAt(mBody.Foot);
        }

        bool HasGroundAhead()
        {
            return HasGroundAhead(mBody.FrontDirInt);
        }

        bool HasGroundAhead(int dir)
        {
            Vector2 checkPosition = mBody.Foot.ExToVector2() + Vector2.right * (dir * (mBody.Size.x * 0.5f + _CheckDistance));
            return CheckGroundAt(checkPosition);
        }

        bool CheckGroundAt(Vector2 footPosition)
        {
            Vector2 checkSize = new Vector2(_GroundCheckSize.x, Mathf.Max(_GroundCheckSize.y, _GroundCheckMinHeight));
            Vector2 checkCenter = footPosition + Vector2.down * (_CheckDistance * 0.5f);
            return Physics2D.OverlapBox(checkCenter, checkSize, 0f, mGroundContactFilter, mOverlapResults) > 0;
        }

        bool HasWallAhead()
        {
            return HasWallAhead(mBody.FrontDirInt);
        }

        bool HasWallAhead(int dir)
        {
            Vector2 checkCenter = mBody.Center.ExToVector2() + Vector2.right * (dir * (mBody.Size.x * 0.5f + _CheckDistance));
            return Physics2D.OverlapBox(checkCenter, _WallCheckSize, 0f, mTerrainContactFilter, mOverlapResults) > 0;
        }

        int GetLayerMask(int layerID)
        {
            if (layerID < 0)
                return 0;

            return 1 << layerID;
        }

        Vector3 GetPlayerCenter()
        {
            if (mPlayerBody != null)
                return mPlayerBody.Center;

            if (mPlayerTransform != null)
                return mPlayerTransform.position;

            return mBody.Center;
        }

        float GetStateStartTime(EnemyState state)
        {
            return IsCurrentState(state) ? mStateEnterTime : Time.time;
        }

        ContactFilter2D CreateContactFilter(int layerMask)
        {
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.useTriggers = false;
            contactFilter.SetLayerMask(layerMask);
            return contactFilter;
        }
        void DoAttackFire()
        {
            // 스킬 오브젝트 생성
            Vector2 startPos = mBody.Center.ExToVector2() + (mBody.FrontDirVec2 * 0.5f);
            Vector2 attackDir = mBody.FrontDirVec2;
            int targetLayerMask = 0; //mBaseObj.gameObject.layer.GetAttackableLayerMask();
            ProjectileBase obj = ProjectileBase.Create(_ProjectilePrefab, startPos, attackDir, targetLayerMask);
            obj.OnHit += (col) =>
            {
                // 충돌 시 처리할 내용
                Health health = col.ExGetCompInBase<Health>();
                if (health != null)
                {
                    DamageInfo damageInfo = new DamageInfo(mSpec[SpecFields.Attack]);
                    health.GetDamaged(damageInfo, mBaseObj);
                }
            };

            obj.OnEnd += () => obj.ExGetBase().DestroyObj();
        }
    }
}
