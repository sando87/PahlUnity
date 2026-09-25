using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PahlUnity.Demo
{
    public class FSMPlayerDash : FiniteStateBase
    {
        [SerializeField] private float _DashCooldown = 0.3f;
        [SerializeField] private float _DashSpeed = 5f;
        [SerializeField] private float _DashDistance = 5f;

        BaseObject mBase;
        PlayerController2D mPlayerCtrl;
        float mDashStartTime = 0f;

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
            RunDashAsync().Forget();
        }

        public override void LeaveState()
        {
            base.LeaveState();
            mDashStartTime = Time.time;
            mBase.Physics2D.LockGravity = false;
            mBase.Anim.SetParamBool(AnimatorParams.LockNormal, false);
        }

        async UniTaskVoid RunDashAsync()
        {
            if (mBase.FSM.CurrentState != this)
                return;

            float moveX = mBase.Input.MoveX;
            if (Mathf.Abs(moveX) >= 0.01f)
                mBase.Body2D.Turn(Mathf.Sign(moveX));

            float dashDuration = _DashDistance / _DashSpeed;

            mBase.Physics2D.LockGravity = true;
            mBase.Physics2D.Velocity = new Vector2(mBase.Body2D.FrontDirInt * _DashSpeed, 0f);

            mBase.Anim.SetParamBool(AnimatorParams.LockNormal, true);
            mBase.Anim.PlayAnim(AnimStateNameHash.Dash);
            await UniTask.Delay((int)(dashDuration * 1000f));

            mPlayerCtrl.ChangeState(PlayerState.Normal);
        }

        public bool IsDashable()
        {
            return MyUtils.IsCooltimeOver(mDashStartTime, _DashCooldown);
        }
    }
}
