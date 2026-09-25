namespace PahlUnity.Demo
{
    public class FSMPlayerNormal : FiniteStateBase
    {
        BaseObject mBase;
        PlayerController2D mPlayerCtrl;

        void Awake()
        {
            mBase = this.ExGetBase();
            mPlayerCtrl = mBase.GetComp<PlayerController2D>();
        }

        public override void UpdateState()
        {
            base.UpdateState();

            if (!mPlayerCtrl.EnableWallAttach)
                mPlayerCtrl.SetWallAttached(false);

            mPlayerCtrl.DoDropDownOnInput();

            if (mPlayerCtrl.DoDashOnInput())
                return;

            mPlayerCtrl.DoJumpOnInput();
            if (mPlayerCtrl.EnableWallAttach)
                mPlayerCtrl.DoAttachjWallOnInput();
            mPlayerCtrl.DoMoveOnInput();
        }

        public override void LeaveState()
        {
            base.LeaveState();
            mPlayerCtrl.SetWallAttached(false);
        }
    }
}
