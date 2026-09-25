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
            mBase.Anim.CancelPreviousAnim(0);
            mBase.Anim.CancelPreviousAnim(1);
            mBase.Physics2D.StopMoving();
            mBase.Physics2D.LockGravity = false;
            mBase.Input.LockPlayerInput = true;
            mBase.Anim.SetParamBool(AnimatorParams.DoNextCombo, false);
            RunDeathAsync().Forget();
        }

        async UniTaskVoid RunDeathAsync()
        {
            if (mBase.FSM.CurrentState != this)
                return;

            await mBase.Anim.PlayAnimWaitEnd(AnimStateNameHash.Death);
        }
    }
}
