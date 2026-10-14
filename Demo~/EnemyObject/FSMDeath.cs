namespace PahlUnity.Demo
{
    public class FSMDeath : FiniteStateBase
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
            mEnemy.StopMoving();
            mBase.Interactor.LockInteract = true;
            mBase.Anim.PlayAnim(AnimStateNameHash.Death);
        }
    }
}
