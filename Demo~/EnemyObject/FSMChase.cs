namespace PahlUnity.Demo
{
    public class FSMChase : FiniteStateBase
    {
        const float ChaseSpeedScale = 1.5f;

        BaseObject mBase;
        EnemyController2D mEnemy;

        void Awake()
        {
            mBase = this.ExGetBase();
            mEnemy = mBase.GetComp<EnemyController2D>();
        }

        public override void EnterState()
        {
            base.EnterState();
            mEnemy.TurnToPlayer();
        }

        public override void UpdateState()
        {
            base.UpdateState();

            if (!mEnemy.IsPlayerInDetectRange() || mEnemy.IsPlayerLost())
            {
                mEnemy.ChangeState(EnemyState.Patrol);
                return;
            }

            if (mEnemy.IsPlayerInAttackRange())
            {
                mEnemy.StopHorizontal();
                if (mEnemy.CanAttack())
                    mEnemy.ChangeState(EnemyState.Attack);
                return;
            }

            int moveDir = mEnemy.GetDirToPlayer();
            if (moveDir == 0 || !mEnemy.HasGroundAhead(moveDir) || mEnemy.HasWallAhead(moveDir))
            {
                mEnemy.StopHorizontal();
                return;
            }

            mEnemy.MoveHorizontal(moveDir, mBase.Spec[SpecFields.MoveSpeed] * ChaseSpeedScale);
        }

        public override void LeaveState()
        {
            base.LeaveState();
            mEnemy.StopHorizontal();
        }
    }
}
