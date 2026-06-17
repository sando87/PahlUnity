using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR;

namespace PahlUnity
{
    public class FiniteStateMachine : MonoBehaviour
    {
        List<FiniteStateBase> mAllStates = new();
        FiniteStateBase mPreviousState = null;
        FiniteStateBase mCurrentState = null;
        bool mIsJustChanged = false;

        public FiniteStateBase PreviousState { get { return mPreviousState; } }
        public FiniteStateBase CurrentState { get { return mCurrentState; } }

        public void AddState(FiniteStateBase state)
        {
            mAllStates.Add(state);
        }

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
                if (mCurrentState != null)
                    mCurrentState.UpdateState();
            }
        }

        public bool TryChangeState(FiniteStateBase newState, bool forceChange = false)
        {
            // 현재 상태와 바꾸려는 상태가 동일하면 전환시키지 않음
            if (!forceChange && mCurrentState == newState)
                return false;

            if (mPreviousState != null)
                mPreviousState.LeaveState();

            mCurrentState = newState;
            mCurrentState.EnterState();

            mPreviousState = mCurrentState;
            mIsJustChanged = true;
            return true;
        }

        public T FindState<T>() where T : FiniteStateBase
        {
            return mAllStates.Find(state => state is T) as T;
        }
        public FiniteStateBase FindState(FiniteStateBase state)
        {
            return mAllStates.Find(s => s == state);
        }
    }

}