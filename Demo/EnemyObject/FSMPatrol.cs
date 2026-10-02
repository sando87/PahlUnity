using UnityEngine;

namespace PahlUnity.Demo
{
    public class FSMPatrol : FiniteStateBase
    {
        [SerializeField] private float _PatrolDistance = 4f;
        [SerializeField] private float _PatrolTurnCooldown = 0.35f;

        BaseObject mBase;
        EnemyController2D mEnemy;
        Vector2 mSpawnPosition = Vector2.zero;
        int mPatrolDir = 1;
        float mNextPatrolTurnTime = 0f;

        void Awake()
        {
            mBase = this.ExGetBase();
            mEnemy = mBase.GetComp<EnemyController2D>();
            mSpawnPosition = mBase.transform.position;
        }

        public override void EnterState()
        {
            base.EnterState();
            if (mPatrolDir == 0)
                mPatrolDir = 1;
            mEnemy.Turn(mPatrolDir);
        }

        public override void UpdateState()
        {
            base.UpdateState();

            if (mEnemy.TryDetectPlayer())
                return;

            if (CanTurnPatrol() && ShouldTurnPatrol())
                TurnPatrol();

            mEnemy.MoveHorizontal(mPatrolDir, mBase.Spec[SpecFields.MoveSpeed]);
        }

        public override void LeaveState()
        {
            base.LeaveState();
            mEnemy.StopHorizontal();
        }

        bool CanTurnPatrol()
        {
            return Time.time >= mNextPatrolTurnTime;
        }

        bool ShouldTurnPatrol()
        {
            return ReachedPatrolEdge() || !mEnemy.HasGroundAhead() || mEnemy.HasWallAhead();
        }

        bool ReachedPatrolEdge()
        {
            float offsetX = mBase.transform.position.x - mSpawnPosition.x;
            return offsetX >= _PatrolDistance && mPatrolDir > 0
                || offsetX <= -_PatrolDistance && mPatrolDir < 0;
        }

        void TurnPatrol()
        {
            mPatrolDir *= -1;
            mNextPatrolTurnTime = Time.time + _PatrolTurnCooldown;
            mEnemy.Turn(mPatrolDir);
        }
    }
}
