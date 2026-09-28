using Cysharp.Threading.Tasks;

namespace PahlUnity.Demo
{
    public class FSMPlayerHit : FiniteStateBase
    {
        BaseObject mBase;
        PlayerController2D mPlayerCtrl;

        void Awake()
        {
            mBase = this.ExGetBase();
            mPlayerCtrl = mBase.GetComp<PlayerController2D>();
        }

        public override void EnterState()
        {
            base.EnterState();
            mBase.Physics2D.LockGravity = false;
            mBase.Physics2D.VelocityX = 0f;
            mBase.Anim.PlayAnim(AnimStateNameHash.Hit, null, OnEndAnimHit, 0);
        }

        void OnEndAnimHit()
        {
            mPlayerCtrl.ChangeState(PlayerState.Normal);
        }
    }
}
