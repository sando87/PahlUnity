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
            mBase.Anim.CancelPreviousAnim(0);
            mBase.Anim.CancelPreviousAnim(1);
            mBase.Physics2D.LockGravity = false;
            mBase.Physics2D.VelocityX = 0f;
            mBase.Anim.SetParamBool(AnimatorParams.DoNextCombo, false);
            RunDamagedAsync().Forget();
        }

        async UniTaskVoid RunDamagedAsync()
        {
            if (mBase.FSM.CurrentState != this)
                return;

            await mBase.Anim.PlayAnimWaitEnd(AnimStateNameHash.Hit);

            mPlayerCtrl.ChangeState(PlayerState.Normal);
        }
    }
}
