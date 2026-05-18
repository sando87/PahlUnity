using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

namespace PahlUnity
{
    public class EnemyAI : MonoBehaviour
    {
        public enum EnemyState
        {
            None,
            Idle,
            Patrol,
            Chase,
            Attack,
            Recover,
            Damaged,
            Death,
        }
        public enum MovementPattern
        {
            None,
            DontMove, // 현재 위치 고정(움직이지 않음)
            FindMinPath, // 플레이어가 있는 노드를 향해 최단경로로 움직이는 패턴
            FindAroundPath, // 플레이어가 있는 노드를 중심으로 일정 범위 내에서 움직이는 패턴
            KeepCurrentNode, // 현재 노드에서 벗어나지 않고 왔다갔다 하는 패턴
            KeepDistance, // 플레이어가 오면 일정 거리 멀어지는 패턴(플레이어로부터 도망가는 패턴)
        }

        protected BaseObject mBase = null;
        protected SpecEnemy mSpec = null;
        protected BaseObject mSignaledTarget = null; // 주변 같은 적으로터 감지된 플레이어 정보가 전달됨
        public BaseObject PlayerTarget { get; protected set; } = null;

        protected CancellationTokenSource mAI_CTS;
        protected CancellationTokenSource mStateCTS;
        protected CancellationTokenSource mMoveCTS = null;

        protected EnemyState mState = EnemyState.Idle;
        protected float mAttackTime = 0;
        public bool IsCooltime => (Time.time - mAttackTime) < mSpec.AttackInterval;

        [SerializeField] float _AttackDegree = 0;
        [SerializeField] protected float _ThinkInterval = 0.5f;
        [SerializeField] bool _AttackOnSameNode = true;
        [SerializeField] MovementPattern _MovementPattern = MovementPattern.KeepCurrentNode;
        [ShowIf(nameof(ShowAroundNodeRange))]
        [SerializeField] int _AroundNodeRange = 7;
        bool ShowAroundNodeRange() { return _MovementPattern == MovementPattern.FindAroundPath; }
        [SerializeField] UnityEvent<BaseObject> OnAttackFire = null;

        void Awake()
        {
            mBase = this.ExGetBase();
        }

        protected virtual void Start()
        {
            mSpec = mBase.EnemyObj.Spec;

            mBase.Health.OnDamaged.AddListener(ChangeDamagedState);
            mBase.Health.OnDied.AddListener(ChangeDeathState);

            mBase.Interactor.OnInteractSignal.AddListener(OnSignaledTarget);
        }

        void OnEnable()
        {
            StartAI();
        }

        void OnDisable()
        {
            StopAI();
        }

        public void StartAI()
        {
            mAI_CTS?.Cancel();
            mAI_CTS?.Dispose();
            mAI_CTS = new CancellationTokenSource();

            mStateCTS?.Cancel();
            mStateCTS?.Dispose();
            mStateCTS = CancellationTokenSource.CreateLinkedTokenSource(mAI_CTS.Token);

            mState = EnemyState.Idle;

            MainLoop(mAI_CTS.Token).Forget();
        }
        public void StopAI()
        {
            Stop();

            mStateCTS?.Cancel();
            mStateCTS?.Dispose();

            // 모든 상태 종료
            mAI_CTS?.Cancel();
            mAI_CTS?.Dispose();
        }

        async UniTask MainLoop(CancellationToken ct)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: ct);

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    switch (mState)
                    {
                        case EnemyState.Idle:
                            ChangeState(await IdleMode(mStateCTS.Token));
                            break;

                        case EnemyState.Patrol:
                            ChangeState(await PatrolMode(mStateCTS.Token));
                            break;

                        case EnemyState.Chase:
                            ChangeState(await ChaseMode(mStateCTS.Token));
                            break;

                        case EnemyState.Attack:
                            ChangeState(await AttackMode(mStateCTS.Token));
                            break;

                        case EnemyState.Recover:
                            ChangeState(await RecoverMode(mStateCTS.Token));
                            break;

                        case EnemyState.Damaged:
                            ChangeState(await DamagedMode(mStateCTS.Token));
                            break;

                        case EnemyState.Death:
                            ChangeState(await DeathMode(mStateCTS.Token));
                            break;

                        case EnemyState.None:
                        default:
                            return;
                    }
                }
                catch (OperationCanceledException)
                {
                }
            }
        }
        public void ChangeState(EnemyState enemyState)
        {
            Stop();

            mStateCTS?.Cancel();
            mStateCTS?.Dispose();
            mStateCTS = CancellationTokenSource.CreateLinkedTokenSource(mAI_CTS.Token);

            mState = enemyState;
        }

        protected virtual async UniTask<EnemyState> IdleMode(CancellationToken ctx)
        {
            Stop();
            PlayerTarget = null;

            try
            {
                PlayerTarget = await DetectTarget(ctx);
                if (PlayerTarget != null)
                {
                    DispatchDetectSignal();

                    if (IsAttackable())
                    {
                        return EnemyState.Attack;
                    }
                    else
                    {
                        return EnemyState.Chase;
                    }
                }
            }
            finally
            {
                // ===== EXIT =====
                Stop();
            }
            return EnemyState.Idle;
        }
        protected virtual async UniTask<EnemyState> PatrolMode(CancellationToken ctx)
        {
            // ===== ENTER =====
            Stop();
            PlayerTarget = null;

            try
            {
                DoPatrolMoving(ctx).Forget();
                PlayerTarget = await DetectTarget(ctx);
                if (PlayerTarget != null)
                {
                    DispatchDetectSignal();

                    if (IsAttackable())
                    {
                        return EnemyState.Attack;
                    }
                    else
                    {
                        return EnemyState.Chase;
                    }
                }
            }
            finally
            {
                // ===== EXIT =====
                Stop();
            }

            return EnemyState.Patrol;
        }
        protected virtual async UniTask<EnemyState> ChaseMode(CancellationToken ctx)
        {
            try
            {
                if (_MovementPattern == MovementPattern.KeepCurrentNode)
                    DoChaseMovingOnlyCurrentNode(ctx).Forget();
                else if (_MovementPattern == MovementPattern.FindAroundPath)
                    DoChaseMovingAroundPath(ctx).Forget();
                else if (_MovementPattern == MovementPattern.FindMinPath)
                    DoChaseMovingMinPath(ctx).Forget();

                int returnIdx = await UniTask.WhenAny(IsAttackableTarget(ctx), IsLostTarget(ctx));
                if (returnIdx == 0)
                    return EnemyState.Attack;
                else if (returnIdx == 1)
                    return EnemyState.Patrol;
            }
            finally
            {
            }
            return EnemyState.Patrol;
        }
        protected virtual async UniTask<EnemyState> AttackMode(CancellationToken ctx)
        {
            try
            {
                Stop();
                TurnToPlayer();

                AnimEventState animEventState = mBase.AnimHelper.PlayAnimWithEvent(AnimStateNameHash.Attack);
                await UniTask.WaitUntil(() => animEventState.IsFired, cancellationToken: ctx);
                DoFireAttack();
                await UniTask.WaitUntil(() => animEventState.IsEnd, cancellationToken: ctx);
                OnEndAttack();
                return EnemyState.Recover;
            }
            finally
            {
                // EXIT
                // 공격 후 정리 (히트박스 off 등)
            }
        }
        protected virtual async UniTask<EnemyState> RecoverMode(CancellationToken ctx)
        {
            try
            {
                float recoverTime = MyUtils.RandomFloat(0.1f, 0.5f);
                await UniTask.Delay(TimeSpan.FromSeconds(recoverTime), cancellationToken: ctx);

                if (!IsTargetValid())
                    return EnemyState.Patrol;
                else if (IsAttackable())
                    return EnemyState.Attack;
                else if (IsTargetInRange(mSpec.DetectLossRange))
                    return EnemyState.Chase;
                else
                    return EnemyState.Patrol;
            }
            finally
            {
            }
        }
        void ChangeDamagedState(DamagedResultInfo retInfo)
        {
            // 받은 데미지가 전체 체력의 10%미만이면 피격모션 없고,
            // 10%~60% 사이이면 피격모션 일정 확률로 발생(추가로 HitChance 스펙계수 곱해짐)
            // 60% 이상이면 기본적으로 피격모션 발생하나 HitChance스펙계수가 추가로 곱해짐
            float damageRate = retInfo.DeltaHealth / (float)retInfo.MaxHealth;
            float hitRate = (damageRate - 0.1f) * 2f;
            int hitPercent = (int)(Mathf.Clamp(hitRate, 0, 1) * 100);
            hitPercent = (hitPercent * mSpec.HitChance).ToInt();
            if (MyUtils.IsPercentHit(hitPercent))
            {
                ChangeState(EnemyState.Damaged);
            }
        }
        protected virtual async UniTask<EnemyState> DamagedMode(CancellationToken ctx)
        {
            try
            {
                AnimEventState animEventState = mBase.AnimHelper.PlayAnimWithEvent(AnimStateNameHash.Hert);
                await UniTask.WaitUntil(() => animEventState.IsEnd, cancellationToken: ctx);
                return EnemyState.Recover;
            }
            finally
            {
            }
        }
        void ChangeDeathState()
        {
            ChangeState(EnemyState.Death);
        }
        protected virtual async UniTask<EnemyState> DeathMode(CancellationToken ctx)
        {
            try
            {
                AnimEventState animEventState = mBase.AnimHelper.PlayAnimWithEvent(AnimStateNameHash.Death);
                await UniTask.WaitUntil(() => animEventState.IsEnd, cancellationToken: ctx);
                return EnemyState.None;
            }
            finally
            {
            }
        }


        protected async UniTask<BaseObject> DetectTarget(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                if (mSignaledTarget != null && !mSignaledTarget.Health.IsDead)
                {
                    BaseObject target = mSignaledTarget;
                    mSignaledTarget = null;
                    return target;
                }
                else
                {
                    BaseObject target = DetectPlayerAround(mSpec.DetectRange);
                    if (target != null)
                    {
                        return target;
                    }
                    else
                    {
                        await UniTask.Delay(TimeSpan.FromSeconds(_ThinkInterval), cancellationToken: ct);
                    }
                }
            }
            return null;
        }
        protected bool IsTargetInRange(float range)
        {
            if (PlayerTarget == null)
                return false;

            float distSqr = Vector2.SqrMagnitude(mBase.Body.Center - PlayerTarget.Body.Center);
            return distSqr <= range * range;
        }
        protected bool IsTargetValid()
        {
            return PlayerTarget != null && !PlayerTarget.Health.IsDead;
        }
        protected virtual bool IsAttackable()
        {
            if (!IsTargetValid())
                return false;

            if (!mBase.Phy.IsGrounded)
                return false;

            if (IsCooltime)
                return false;

            if (_AttackOnSameNode && !IsStandOnSameNode())
                return false;

            float range = mSpec.AttackRange;
            float distSqr = Vector2.SqrMagnitude(mBase.Body.Center - PlayerTarget.Body.Center);
            if (distSqr > range * range)
                return false;

            if (_AttackDegree > 0)
            {
                Vector2 dirToPlayer = PlayerTarget.Body.Center - mBase.Body.Center;
                float dot = Vector2.Dot(mBase.transform.right, dirToPlayer.normalized);
                if (dot <= 0 || dot < Mathf.Cos(_AttackDegree * Mathf.Deg2Rad))
                    return false;
            }

            return true;
        }
        protected async UniTask IsAttackableTarget(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                if (IsAttackable())
                {
                    break;
                }
                else
                {
                    float thinkInterval = MyUtils.RandomFloat(_ThinkInterval - 0.5f, _ThinkInterval + 0.5f);
                    thinkInterval.ExSetMinimum(0.1f);
                    await UniTask.Delay(TimeSpan.FromSeconds(thinkInterval), cancellationToken: ct);
                }
            }
        }
        protected async UniTask IsLostTarget(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                if (!IsTargetValid())
                {
                    break;
                }
                else if (!IsTargetInRange(mSpec.DetectLossRange))
                {
                    break;
                }
                else
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(_ThinkInterval), cancellationToken: ct);
                }
            }
        }


        async UniTask DoPatrolMoving(CancellationToken ct)
        {
            try
            {
                int curDir = mBase.Body.FrontDirInt;
                while (!ct.IsCancellationRequested)
                {
                    NodeNav node = GetCurrentNodeNav(mBase);
                    if (node != null)
                    {
                        NodeNavGroup nodeNavGroup = node.ParentGroup;
                        curDir *= -1;
                        Vector2 desPos = curDir > 0 ? nodeNavGroup.MostRightNode.CenterTopPos : nodeNavGroup.MostLeftNode.CenterTopPos;
                        MoveToDestPosition(mSpec.MoveSpeed, desPos).Forget();
                    }
                    await UniTask.Delay(TimeSpan.FromSeconds(MyUtils.RandomFloat(2f, 5f)), cancellationToken: ct);
                }
            }
            catch (OperationCanceledException)
            {
                // LOG.trace(ex.Message);
            }
        }

        PathInfo FindPathAround(int aroundNodeRange)
        {
            NodeNav node = GetCurrentNodeNav(mBase);
            if (node == null)
                return null;

            PathInfo path = InGameEngine.Inst.Pathfinder.FindPathAround(node, mSpec.MoveSpeed, aroundNodeRange);
            return path;
        }
        PathInfo FindMinPath()
        {
            NodeNav node = GetCurrentNodeNav(mBase);
            if (node == null)
                return null;

            PathInfo path = InGameEngine.Inst.Pathfinder.FindMinPath(node, mSpec.MoveSpeed);
            return path;
        }

        NodeNav GetCurrentNodeNav(BaseObject baseObject)
        {
            if (!baseObject.Phy.IsGrounded)
                return null;

            NodeNav node = InGameEngine.Inst.Pathfinder.GetCurrentGroundNode(baseObject.Body.Rect);
            if (node != null)
                return node;

            return null;
        }

        protected bool IsNoWayToMove()
        {
            Vector2 pos = mBase.Body.FootFront + new Vector2(mBase.transform.right.x * 0.2f, 0);
            NodeNav frontNode = InGameEngine.Inst.Pathfinder.GetNode(pos);
            if (frontNode != null && !frontNode.IsThin) // IsObstacled
                return true;

            pos.y -= 0.2f;
            NodeNav frontGroundNode = InGameEngine.Inst.Pathfinder.GetNode(pos);
            if (frontGroundNode == null) // No ground ahead
                return true;

            return false;
        }

        async UniTask GotoPathDestPosition(PathInfo path, CancellationToken ct)
        {
            try
            {
                Stop();
                if (path.Transition.TransitionType == NodeTransitionType.JustJumpUp)
                {
                    if (path.IsNoNeedToMove)
                    {
                        await JustJumpUp(path.JumpForce);
                    }
                    else
                    {
                        Vector2 worldWayPos = path.Transition.StartNode.CenterTopPos;
                        await MoveToDestPosition(mSpec.MoveSpeed, worldWayPos);
                        await UniTask.Delay(TimeSpan.FromSeconds(0.02f), cancellationToken: ct);
                        await JustJumpUp(path.JumpForce);
                    }
                }
                else if (path.Transition.TransitionType == NodeTransitionType.DropDown)
                {
                    if (path.IsNoNeedToMove)
                    {
                        await DropDown();
                    }
                    else
                    {
                        Vector2 worldWayPos = path.Transition.StartNode.CenterTopPos;
                        await MoveToDestPosition(mSpec.MoveSpeed, worldWayPos);
                        await UniTask.Delay(TimeSpan.FromSeconds(0.02f), cancellationToken: ct);
                        await DropDown();
                    }
                }
                else if (path.Transition.TransitionType == NodeTransitionType.MovingJump)
                {
                    Vector2 worldWayPos = path.Transition.StartNode.CenterTopPos;
                    Vector2 worldDestPos = path.Transition.EndNode.CenterTopPos;
                    await MoveToDestPosition(mSpec.MoveSpeed, worldWayPos);
                    await UniTask.Delay(TimeSpan.FromSeconds(0.02f), cancellationToken: ct);
                    await JumpMoving(path.JumpForce, mSpec.MoveSpeed, worldDestPos);
                }
                else if (path.Transition.TransitionType == NodeTransitionType.JumpAndMove)
                {
                    Vector2 worldWayPos = path.Transition.StartNode.CenterTopPos;
                    Vector2 worldDestPos = path.Transition.EndNode.CenterTopPos;
                    await MoveToDestPosition(mSpec.MoveSpeed, worldWayPos);
                    await UniTask.Delay(TimeSpan.FromSeconds(0.02f), cancellationToken: ct);
                    await JumpAndMove(path.JumpForce, mSpec.MoveSpeed, worldDestPos);
                }
                else if (path.Transition.TransitionType == NodeTransitionType.WalkAndFall)
                {
                    Vector2 worldWayPos = path.Transition.StartNode.CenterTopPos;
                    Vector2 worldDestPos = path.Transition.EndNode.CenterTopPos;
                    await MoveToDestPosition(mSpec.MoveSpeed, worldWayPos);
                    await UniTask.Delay(TimeSpan.FromSeconds(0.02f), cancellationToken: ct);
                    await MoveAndFall(mSpec.MoveSpeed, worldDestPos);
                }
                Stop();
            }
            catch (OperationCanceledException)
            {
                // LOG.trace(ex.Message);
            }
        }

        protected async UniTask DoChaseMovingMinPath(CancellationToken ct)
        {
            try
            {
                await UniTask.Yield(cancellationToken: ct);
                Stop();
                while (!ct.IsCancellationRequested && PlayerTarget != null)
                {
                    if (IsStandOnSameNode())
                    {
                        if (!IsTargetInRange(mSpec.AttackRange))
                        {
                            int curDir = mBase.Body.Center.x < PlayerTarget.Body.Center.x ? 1 : -1;
                            Turn(curDir);
                            StartMoving(curDir * mSpec.MoveSpeed);
                        }
                        // else
                        // {
                        //     int curDir = mBase.Body.FrontDirInt;
                        //     curDir = IsNoWayToMove() ? -curDir : ((MyUtils.RandomInt(0, 2) * 2) - 1);
                        //     Turn(curDir);
                        //     StartMoving(curDir * mSpec.MoveSpeed);
                        // }
                        await UniTask.Delay(TimeSpan.FromSeconds(MyUtils.RandomFloat(0.5f, 3.5f)), cancellationToken: ct);
                    }
                    else
                    {
                        PathInfo path = FindMinPath();
                        if (path != null)
                        {
                            await GotoPathDestPosition(path, ct);
                        }

                        await UniTask.Delay(TimeSpan.FromSeconds(0.02f), cancellationToken: ct);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // LOG.trace(ex.Message);
            }
        }

        protected async UniTask DoChaseMovingAroundPath(CancellationToken ct)
        {
            try
            {
                await UniTask.Yield(cancellationToken: ct);
                Stop();
                int stayChanceOnSameNode = 50;
                while (!ct.IsCancellationRequested && PlayerTarget != null)
                {
                    if (IsStandOnAroundNode() && MyUtils.IsPercentHit(stayChanceOnSameNode))
                    {
                        stayChanceOnSameNode = 50;
                        // int curDir = MyUtils.RandomInt(0, 2) == 0 ? 1 : -1;
                        int curDir = mBase.Body.Center.x < PlayerTarget.Body.Center.x ? 1 : -1;
                        Turn(curDir);
                        StartMoving(curDir * mSpec.MoveSpeed);
                        await UniTask.Delay(TimeSpan.FromSeconds(MyUtils.RandomFloat(0.5f, 3.5f)), cancellationToken: ct);
                    }
                    else
                    {
                        stayChanceOnSameNode = 100;
                        PathInfo path = FindPathAround(_AroundNodeRange);
                        if (path != null)
                        {
                            await GotoPathDestPosition(path, ct);
                        }

                        await UniTask.Delay(TimeSpan.FromSeconds(0.02f), cancellationToken: ct);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // LOG.trace(ex.Message);
            }
        }

        protected async UniTask DoChaseMovingOnlyCurrentNode(CancellationToken ct)
        {
            try
            {
                await UniTask.Yield(cancellationToken: ct);
                Stop();
                while (!ct.IsCancellationRequested && PlayerTarget != null)
                {
                    int curDir = mBase.Body.FrontDirInt;
                    curDir = IsNoWayToMove() ? -curDir : ((MyUtils.RandomInt(0, 2) * 2) - 1);
                    Turn(curDir);
                    StartMoving(curDir * mSpec.MoveSpeed);
                    await UniTask.Delay(TimeSpan.FromSeconds(MyUtils.RandomFloat(0.5f, 3.5f)), cancellationToken: ct);
                }
            }
            catch (OperationCanceledException)
            {
                // LOG.trace(ex.Message);
            }
        }

        bool IsStandOnAroundNode()
        {
            if (!IsTargetValid())
                return false;

            // NodeNav playerNode = GetCurrentNodeNav(mPlayerTarget);
            // if (playerNode == null)
            //     return false;

            NodeNav baseNode = GetCurrentNodeNav(mBase);
            if (baseNode == null)
                return false;

            // if (playerNode.ParentGroup == baseNode.ParentGroup)
            //     return true;

            float minWeight = InGameManager.Instance.Engine.Pathfinder.GetMinWeight(baseNode.ParentGroup);
            return minWeight <= _AroundNodeRange;
        }
        bool IsStandOnSameNode()
        {
            if (!IsTargetValid())
                return false;

            NodeNav playerNode = GetCurrentNodeNav(PlayerTarget);
            if (playerNode == null)
                return false;

            NodeNav baseNode = GetCurrentNodeNav(mBase);
            if (baseNode == null)
                return false;

            return playerNode.ParentGroup == baseNode.ParentGroup;
        }

        BaseObject DetectPlayerAround(float range)
        {
            Vector2Int nodePos = Vector2Int.RoundToInt(mBase.Body.Center);
            PlayerDepthInfo playerDepthInfo = InGameEngine.Inst.DepthManager.GetPlayerDepthInfoAtPos(nodePos);
            if (playerDepthInfo == null || playerDepthInfo.GetWeight() > range)
            {
                return null;
            }

            List<BaseObject> rets = TemporaryList<BaseObject>.GetTempList();
            UtilitiesPhy2D.OverlapCircleAll(mBase.Body.Center, range, 1 << LayerID.Player, InteractMask.Unit, rets);
            float minDistSqr = float.PositiveInfinity;
            BaseObject closestPlayer = null;
            foreach (BaseObject player in rets)
            {
                if (player.Health == null || player.Health.IsDead)
                {
                    continue;
                }

                float distSqr = Vector2.SqrMagnitude(mBase.Body.Center - player.Body.Center);
                if (distSqr < minDistSqr)
                {
                    minDistSqr = distSqr;
                    closestPlayer = player;
                }
            }
            rets.Clear();
            return closestPlayer;
        }


        protected void CancelMoveCTS()
        {
            if (mMoveCTS != null)
            {
                mMoveCTS.Cancel();
                mMoveCTS.Dispose();
                mMoveCTS = null;
            }
        }
        protected void Stop()
        {
            CancelMoveCTS();

            if (mBase != null)
            {
                // mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.Idle);
                mBase.Phy.Velocity = Vector2.zero;
            }
        }
        protected void Turn(float worldDir)
        {
            if (worldDir == 0) return;

            Vector3 front = worldDir > 0 ? Vector3.forward : Vector3.back;
            transform.rotation = Quaternion.LookRotation(front, transform.up);
        }
        protected void TurnTo(Vector2 targetPos)
        {
            int curDir = mBase.Body.Center.x < targetPos.x ? 1 : -1;
            Turn(curDir);
        }
        protected void TurnToPlayer()
        {
            if (PlayerTarget != null)
            {
                int curDir = mBase.Body.Center.x < PlayerTarget.Body.Center.x ? 1 : -1;
                Turn(curDir);
            }
        }
        protected void StartMoving(float velocity)
        {
            CancelMoveCTS();
            mMoveCTS = new CancellationTokenSource();

            MoveToEnd(mMoveCTS.Token, velocity).Forget();
        }

        async UniTask MoveToEnd(CancellationToken ct, float v)
        {
            mBase.AnimHelper.PlayAnim(AnimStateNameHash.Run);

            while (!ct.IsCancellationRequested)
            {
                mBase.Phy.VelocityX = v;
                await UniTask.Yield(ct);
                if (IsNoWayToMove())
                    break;
            }

            mBase.Phy.Velocity = Vector2.zero;
            mBase.AnimHelper.PlayAnim(AnimStateNameHash.Idle);
        }

        async UniTask MoveToDestPosition(float velocityX, Vector2 destPos)
        {
            CancelMoveCTS();
            mMoveCTS = new CancellationTokenSource();

            mBase.AnimHelper.PlayAnim(AnimStateNameHash.Run);
            Vector2 startPos = mBase.Body.Foot;
            int startDir = startPos.x < destPos.x ? 1 : -1;
            Turn(startDir);

            while (!mMoveCTS.Token.IsCancellationRequested)
            {
                if (IsArrivedDestPosition(destPos, startDir))
                    break;

                mBase.Phy.VelocityX = velocityX * startDir;
                await UniTask.Yield(mMoveCTS.Token);
            }

            mBase.Phy.Velocity = Vector2.zero;
            mBase.AnimHelper.PlayAnim(AnimStateNameHash.Idle);
        }

        async UniTask MoveAndFall(float velocityX, Vector2 destPos)
        {
            CancelMoveCTS();
            mMoveCTS = new CancellationTokenSource();

            mBase.AnimHelper.PlayAnim(AnimStateNameHash.Run);
            Vector2 startPos = mBase.Body.Foot;
            int startDir = startPos.x < destPos.x ? 1 : -1;
            Turn(startDir);

            // 앞으로 그냥 걸어감(낙하될때까지)
            while (!mMoveCTS.Token.IsCancellationRequested)
            {
                mBase.Phy.VelocityX = velocityX * startDir;
                await UniTask.Yield(mMoveCTS.Token);

                if (IsArrivedDestPosition(destPos + new Vector2(0.5f * startDir, 0), startDir))
                    break;
            }

            await UniTask.WaitUntil(() => mBase.Phy.IsGrounded, cancellationToken: mMoveCTS.Token);

            // 착지
            mBase.Phy.Velocity = Vector2.zero;
            mBase.AnimHelper.PlayAnim(AnimStateNameHash.Idle);

        }

        async UniTask JustJumpUp(float jumpForce)
        {
            CancelMoveCTS();
            mMoveCTS = new CancellationTokenSource();

            mBase.AnimHelper.PlayAnim(AnimStateNameHash.Jump);
            mBase.Phy.DoJump(jumpForce);
            await UniTask.WaitUntil(() => mBase.Phy.IsGrounded, cancellationToken: mMoveCTS.Token);

            mBase.Phy.Velocity = Vector2.zero;
            mBase.AnimHelper.PlayAnim(AnimStateNameHash.Idle);
        }
        async UniTask DropDown()
        {
            CancelMoveCTS();
            mMoveCTS = new CancellationTokenSource();

            mBase.AnimHelper.PlayAnim(AnimStateNameHash.Jump);

            mBase.Body.LockThinPlatformMomentarily();
            await UniTask.WaitUntil(() => !mBase.Body.LockThinPlatform && mBase.Phy.IsGrounded, cancellationToken: mMoveCTS.Token);

            mBase.Phy.Velocity = Vector2.zero;
            mBase.AnimHelper.PlayAnim(AnimStateNameHash.Idle);
        }
        async UniTask JumpMoving(float jumpForce, float velocityX, Vector2 destPos)
        {
            CancelMoveCTS();
            mMoveCTS = new CancellationTokenSource();

            mBase.AnimHelper.PlayAnim(AnimStateNameHash.Jump);
            Vector2 startPos = mBase.Body.Foot;
            int startDir = startPos.x < destPos.x ? 1 : -1;
            Turn(startDir);
            mBase.Phy.DoJump(jumpForce);

            while (!mMoveCTS.Token.IsCancellationRequested)
            {
                mBase.Phy.VelocityX = velocityX * startDir;
                await UniTask.Yield(mMoveCTS.Token);

                if (mBase.Phy.IsGrounded)
                    break;
            }

            mBase.Phy.Velocity = Vector2.zero;
            mBase.AnimHelper.PlayAnim(AnimStateNameHash.Idle);
        }

        async UniTask JumpAndMove(float jumpForce, float velocityX, Vector2 destPos)
        {
            CancelMoveCTS();
            mMoveCTS = new CancellationTokenSource();

            mBase.AnimHelper.PlayAnim(AnimStateNameHash.Jump);
            Vector2 startPos = mBase.Body.Foot;
            int startDir = startPos.x < destPos.x ? 1 : -1;
            Turn(startDir);
            mBase.Phy.DoJump(jumpForce);

            while (!mMoveCTS.Token.IsCancellationRequested)
            {
                if (mBase.Phy.VelocityY < 0)
                {
                    if (!IsArrivedDestPosition(destPos, startDir))
                        mBase.Phy.VelocityX = velocityX * startDir;
                    else
                        mBase.Phy.VelocityX = 0;
                }

                await UniTask.Yield(mMoveCTS.Token);

                if (mBase.Phy.IsGrounded)
                    break;
            }

            mBase.Phy.Velocity = Vector2.zero;
            mBase.AnimHelper.PlayAnim(AnimStateNameHash.Idle);
        }

        bool IsArrivedDestPosition(Vector2 destPos, int startDir)
        {
            Vector2 curPos = mBase.Body.Foot;
            if (Mathf.Abs(curPos.x - destPos.x) <= 0.2f)
                return true;

            int currentDir = curPos.x < destPos.x ? 1 : -1;
            return startDir != currentDir;
        }

        protected void OnEndAttack()
        {
        }

        protected virtual void DoFireAttack()
        {
            mAttackTime = Time.time;
            OnAttackFire?.Invoke(PlayerTarget);
        }

        void DispatchDetectSignal()
        {
            List<BaseObject> aroundEnemies = new List<BaseObject>();
            aroundEnemies.Add(mBase);
            int startIndex = 0;
            int count = 1;
            while (true)
            {
                int newAddedCount = SearchAroundEnemies(aroundEnemies, startIndex, count);
                if (newAddedCount <= 0)
                    break;

                startIndex += count;
                count = newAddedCount;
            }

            foreach (BaseObject enemy in aroundEnemies)
            {
                if (enemy == mBase)
                    continue;

                enemy.Interactor.InvokeInteractSignal(mBase, InteractMask.DetectSignal);
            }
        }
        int SearchAroundEnemies(List<BaseObject> retEnemies, int startIndex, int count)
        {
            int newAddedCount = 0;
            List<BaseObject> arEnemies = TemporaryList<BaseObject>.GetTempList();
            for (int i = startIndex; i < startIndex + count; i++)
            {
                arEnemies.Clear();
                BaseObject enemy = retEnemies[i];
                UtilitiesPhy2D.OverlapCircleAll(enemy.Body.Center, 5, 1 << LayerID.Enemy, InteractMask.Unit, arEnemies);
                foreach (BaseObject arEnemy in arEnemies)
                {
                    if (arEnemy == enemy)
                        continue;

                    if (!retEnemies.Contains(arEnemy))
                    {
                        newAddedCount++;
                        retEnemies.Add(arEnemy);
                    }
                }
            }
            arEnemies.Clear();
            return newAddedCount;
        }
        void OnSignaledTarget(BaseObject invoker, InteractMask signal)
        {
            // 적 한마리가 플레이어를 감지하면 그 신호를 주변 다른 적들도 받아서 플레이어를 타겟으로 삼게 함
            if (signal.HasFlag(InteractMask.DetectSignal))
            {
                EnemyAI sender = invoker.GetComponentInChildren<EnemyAI>();
                if (sender != null && sender.PlayerTarget != null)
                {
                    mSignaledTarget = sender.PlayerTarget;
                }
            }
        }
        public void SetTarget(BaseObject target)
        {
            mSignaledTarget = target;
        }

    }
}