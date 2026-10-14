using Cysharp.Threading.Tasks;

namespace PahlUnity.Demo
{
    public class FSMPlayerDeath : FiniteStateBase
    {
        BaseObject mBase;

        void Awake()
        {
            mBase = this.ExGetBase();
        }

        public override void EnterState()
        {
            base.EnterState();
            mBase.Physics2D.StopMoving();
            mBase.Physics2D.LockGravity = false;
            mBase.Input.LockPlayerInput = true;
            mBase.Anim.PlayAnim(AnimStateNameHash.Death);
        }
    }
}
