using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR;

namespace PahlUnity
{
    public class FiniteStateMachine : MonoBehaviour
    {
        FiniteStateBase mCurrentState = null;
        FiniteStateBase mDefaultState = null;
        bool mIsJustChanged = false;

        public FiniteStateBase CurrentState { get { return mCurrentState; } }
        public bool IsDefaultState { get { return mCurrentState == mDefaultState; } }

        void Update()
        {
            if (mIsJustChanged)
            {
                // State막 바뀐 다음 상태에서는 UpdateState를 호출하지 않음
                // EnterState() 함수와 UpdateState() 함수가 같은 프레임에 호출되면 오작동 발생 가능성 높음
                mIsJustChanged = false;
                return;
            }
            else
            {
                mCurrentState?.UpdateState();
            }
        }

        public bool TryChangeState(FiniteStateBase newState, bool forceChange = false)
        {
            // 현재 상태와 바꾸려는 상태가 동일하면 전환시키지 않음
            if (!forceChange && mCurrentState == newState)
                return false;

            mCurrentState?.LeaveState();
            mCurrentState = newState;
            mCurrentState.EnterState();

            mIsJustChanged = true;
            return true;
        }

        public void ChangeDefaultState()
        {
            LOG.errorif(mDefaultState == null, "Default state is not set");
            TryChangeState(mDefaultState);
        }

        public void SetDefaultState(FiniteStateBase state)
        {
            mDefaultState = state;
        }
    }

}