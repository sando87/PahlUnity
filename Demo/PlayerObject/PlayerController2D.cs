using System.Collections.Generic;
using UnityEngine;

namespace PahlUnity.Demo
{
    public enum PlayerState
    {
        Normal,
        Dashing,
        Damaged,
        Dead,
    }

    /// <summary>
    /// 플레이어 캐릭터의 입력을 받아서 이동, 점프, 상호작용 등을 처리하는 컨트롤러 클래스
    /// </summary>
    public class PlayerController2D : MonoBehaviour
    {

        BaseObject mBaseObj = null;
        FiniteStateMachine mFSM = null;
        Dictionary<PlayerState, FiniteStateBase> mStates = new();

        void Awake()
        {
            mBaseObj = this.ExGetBase();
            mFSM = mBaseObj.FSM;
            if (mFSM == null)
                mFSM = mBaseObj.gameObject.AddComponent<FiniteStateMachine>();

            mBaseObj.Body2D.OnTurn += (isRight) =>
            {
                mBaseObj.Render.SetFlipX(!isRight);
            };

            mBaseObj.Health.OnDied += (_) =>
            {
                PlayDeathMotion();
            };
            mBaseObj.Health.OnDamaged += (damageInfo, attacker) =>
            {
                if (damageInfo.Value > 0)
                    PlayDamagedMotion();
            };

            BindStates();
        }

        void Start()
        {
            ChangeState(PlayerState.Normal, true);
        }

        public void PlayDamagedMotion()
        {
            if (IsCurrentState(PlayerState.Dead))
                return;

            ChangeState(PlayerState.Damaged, true);
        }

        public void PlayDeathMotion()
        {
            if (IsCurrentState(PlayerState.Dead))
                return;

            ChangeState(PlayerState.Dead);
        }

        public void OnDamaged()
        {
            PlayDamagedMotion();
        }

        public void OnDied()
        {
            PlayDeathMotion();
        }

        void BindStates()
        {
            FSMPlayerNormal normalState = GetComponentInChildren<FSMPlayerNormal>(true);
            FSMPlayerDash dashState = GetComponentInChildren<FSMPlayerDash>(true);
            FSMPlayerHit hitState = GetComponentInChildren<FSMPlayerHit>(true);
            FSMPlayerDeath deathState = GetComponentInChildren<FSMPlayerDeath>(true);

            mFSM.SetDefaultState(normalState);
            mStates[PlayerState.Normal] = normalState;
            mStates[PlayerState.Dashing] = dashState;
            mStates[PlayerState.Damaged] = hitState;
            mStates[PlayerState.Dead] = deathState;
        }

        public bool ChangeState(PlayerState state, bool forceChange = false)
        {
            return mFSM.TryChangeState(mStates[state], forceChange);
        }

        bool IsCurrentState(PlayerState state)
        {
            return mFSM.CurrentState == mStates[state];
        }

        public bool HandleDash()
        {
            if (!mBaseObj.Input.JustPressed(InputActionNameHash.Dash))
                return false;

            FSMPlayerDash dashState = mStates[PlayerState.Dashing] as FSMPlayerDash;
            if (!dashState.IsDashable())
                return false;

            return ChangeState(PlayerState.Dashing);
        }
    }
}
