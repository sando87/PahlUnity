using UnityEngine;

namespace PahlUnity.Demo
{
    public class FSMIdle : FiniteStateBase
    {
        [SerializeField] private float _IdleDuration = 1f;

        EnemyController2D mEnemy;

        void Awake()
        {
            mEnemy = this.ExGetBase().GetComp<EnemyController2D>();
        }

        public override void EnterState()
        {
            base.EnterState();
            mEnemy.StopHorizontal();
        }

        public override void UpdateState()
        {
            base.UpdateState();

            if (mEnemy.TryDetectPlayer())
                return;

            if (Time.time >= mEnemy.StateEnterTime + _IdleDuration)
                mEnemy.ChangeState(EnemyState.Patrol);
        }

        public override void LeaveState()
        {
            base.LeaveState();
            mEnemy.StopHorizontal();
        }
    }
}
