using UnityEngine;

namespace PahlUnity.Demo
{
    public class FSMDetect : FiniteStateBase
    {
        [SerializeField] private float _DetectDuration = 0.35f;

        EnemyController2D mEnemy;

        void Awake()
        {
            mEnemy = this.ExGetBase().GetComp<EnemyController2D>();
        }

        public override void EnterState()
        {
            base.EnterState();
            mEnemy.StopHorizontal();
            mEnemy.TurnToPlayer();
        }

        public override void UpdateState()
        {
            base.UpdateState();
            mEnemy.TurnToPlayer();

            if (!mEnemy.IsPlayerInDetectRange())
            {
                mEnemy.ChangeState(EnemyState.Patrol);
                return;
            }

            if (Time.time >= mEnemy.StateEnterTime + _DetectDuration)
            {
                EnemyState nextState = mEnemy.CanAttack() && mEnemy.IsPlayerInAttackRange()
                    ? EnemyState.Attack
                    : EnemyState.Chase;
                mEnemy.ChangeState(nextState);
            }
        }

        public override void LeaveState()
        {
            base.LeaveState();
            mEnemy.StopHorizontal();
        }
    }
}
