namespace PahlUnity.Demo
{
    public class FSMDamaged : FiniteStateBase
    {
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
            mEnemy.StopHorizontal();
            mBase.Anim.PlayAnim(AnimStateNameHash.Hit, null, OnEndDamagedAnim);
        }

        void OnEndDamagedAnim()
        {
            EnemyState nextState = mEnemy.IsPlayerInDetectRange() ? EnemyState.Chase : EnemyState.Patrol;
            mEnemy.ChangeState(nextState);
        }
    }
}
