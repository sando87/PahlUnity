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

            mPlayerCtrl.HandleDropDown();

            if (mPlayerCtrl.HandleDash())
                return;

            mPlayerCtrl.HandleJump();
            if (mPlayerCtrl.EnableWallAttach)
                mPlayerCtrl.HandleWallAttach();
            mPlayerCtrl.DoMoveOnInput();
        }

        public override void LeaveState()
        {
            base.LeaveState();
            mPlayerCtrl.SetWallAttached(false);
        }
    }
}
