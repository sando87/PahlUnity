using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;


namespace PahlUnity
{
    public enum FiniteStateType
    {
        None,
        Idle,
        Walk,
        Run,
        Floating,
        Dash,
        Attack,
        Damaged,
        Stuned,
        Death,
        Respawn,
    }
    public class FiniteStateBase : MonoBehaviour
    {
        [SerializeField][Range(0, 10)] int _Layer = 0;
        [SerializeField][Range(0, 10)] int _Priority = 0;
        [SerializeField] FiniteStateType _StateType = FiniteStateType.None;

        public int Layer { get { return _Layer; } }
        public int Priority { get { return _Priority; } }
        public FiniteStateType StateType { get { return _StateType; } }

        public BaseObject Base { get; private set; }

        [Foldout("Events")]
        public UnityEvent EventEnter = new UnityEvent();
        [Foldout("Events")]
        public UnityEvent EventLeave = new UnityEvent();

        private Dictionary<int, UnityAction> mEventsEnter = new Dictionary<int, UnityAction>();
        private Dictionary<int, UnityAction<int>> mEventsMiddle = new Dictionary<int, UnityAction<int>>();
        private Dictionary<int, UnityAction> mEventsLeave = new Dictionary<int, UnityAction>();

        public bool IsJustEntered { get; set; } = false; // 처음 딱 현재 State모션 진입했을때는 UpdateState호출 안해주기 위한 장치
        public bool IsStateCancelable { get; protected set; } = true; // 점프나 대쉬로 모션 캔슬 가능한지 여부

        public virtual void InitState()
        {
            Base = GetComponentInParent<BaseObject>();
        }

        public virtual void HandleInput()
        {
            // 입력 처리 코드
        }

        public virtual void EnterState(object param)
        {
            // 상태에 진입할 때 실행되는 코드
            EventEnter?.Invoke();
        }
        public virtual void UpdateState()
        {
            // 매 프레임마다 실행되는 코드
        }
        public virtual void FixedUpdateState()
        {
            // 매 프레임마다 실행되는 코드
        }
        public virtual void LeaveState()
        {
            // 상태에서 벗어날 때 실행되는 코드
            RemoveAllEvents();
            StopAllCoroutines();
            EventLeave?.Invoke();
        }

        protected void ChangeStateToIdle()
        {
            Base.StateMachine.TryChangeStateToIdle(Layer);
        }
        protected void ChangeStateToThis()
        {
            Base.StateMachine.TryChangeState(this);
        }
        protected void PlayAnim(string stateName)
        {
            Base.AnimHelper.CrossFadeToState(stateName, Layer);
        }
        protected PlayAnimEnvent PlayAnim(AnimStateNameHash stateHashName)
        {
            Base.AnimHelper.CrossFadeToState(stateHashName, Layer);
            return new PlayAnimEnvent(this, stateHashName);
        }
        public bool IsCurrentThisState()
        {
            return Base.StateMachine.GetCurrentState(Layer) == this;
        }
        protected FiniteStateBase GetCurrentState()
        {
            return Base.StateMachine.GetCurrentState(Layer);
        }
        public bool IsChangable()
        {
            FiniteStateBase currentState = Base.StateMachine.GetCurrentState(Layer);
            if (currentState == this)
                return false;

            if (this.Priority < currentState.Priority)
                return false;

            return true;
        }

        protected void AddEventEnter(AnimStateNameHash stateHash, UnityAction handler)
        {
            if (mEventsEnter.ContainsKey(stateHash))
                return;

            mEventsEnter[stateHash] = () => { if (IsCurrentThisState()) handler.Invoke(); };
            Base.AnimHelper.AddEventEnter(stateHash, mEventsEnter[stateHash]);
        }
        protected void RemoveEventEnter(AnimStateNameHash stateHash)
        {
            if (!mEventsEnter.ContainsKey(stateHash))
                return;

            Base.AnimHelper.RemoveEventEnter(stateHash, mEventsEnter[stateHash]);
            mEventsEnter.Remove(stateHash);
        }
        protected void AddEventMiddle(AnimStateNameHash stateHash, UnityAction<int> handler)
        {
            if (mEventsMiddle.ContainsKey(stateHash))
                return;

            mEventsMiddle[stateHash] = (index) => { if (IsCurrentThisState()) handler.Invoke(index); };
            Base.AnimHelper.AddEventMiddle(stateHash, mEventsMiddle[stateHash]);
        }
        protected void RemoveEventMiddle(AnimStateNameHash stateHash)
        {
            if (!mEventsMiddle.ContainsKey(stateHash))
                return;

            Base.AnimHelper.RemoveEventMiddle(stateHash, mEventsMiddle[stateHash]);
            mEventsMiddle.Remove(stateHash);
        }
        protected void AddEventLeave(AnimStateNameHash stateHash, UnityAction handler)
        {
            if (mEventsLeave.ContainsKey(stateHash))
                return;

            mEventsLeave[stateHash] = () => { if (IsCurrentThisState()) handler.Invoke(); };
            Base.AnimHelper.AddEventLeave(stateHash, mEventsLeave[stateHash]);
        }
        protected void RemoveEventLeave(AnimStateNameHash stateHash)
        {
            if (!mEventsLeave.ContainsKey(stateHash))
                return;

            Base.AnimHelper.RemoveEventLeave(stateHash, mEventsLeave[stateHash]);
            mEventsLeave.Remove(stateHash);
        }
        public void RemoveAllEvents()
        {
            foreach (var handler in mEventsEnter)
            {
                Base.AnimHelper.RemoveEventEnter(handler.Key, handler.Value);
            }
            mEventsEnter.Clear();
            foreach (var handler in mEventsMiddle)
            {
                Base.AnimHelper.RemoveEventMiddle(handler.Key, handler.Value);
            }
            mEventsMiddle.Clear();
            foreach (var handler in mEventsLeave)
            {
                Base.AnimHelper.RemoveEventLeave(handler.Key, handler.Value);
            }
            mEventsLeave.Clear();
        }



        public struct PlayAnimEnvent
        {
            FiniteStateBase mFsmBase;
            AnimStateNameHash mStateHash;
            public PlayAnimEnvent(FiniteStateBase _fsm, AnimStateNameHash _hash)
            {
                mFsmBase = _fsm;
                mStateHash = _hash;
            }
            public PlayAnimEnvent OnFire(UnityAction<int> _onFire)
            {
                mFsmBase.AddEventMiddle(mStateHash, _onFire);
                return new PlayAnimEnvent(mFsmBase, mStateHash);
            }
            public PlayAnimEnvent OnEnd(UnityAction _onEnd)
            {
                mFsmBase.AddEventLeave(mStateHash, _onEnd);
                return new PlayAnimEnvent(mFsmBase, mStateHash);
            }
        }
    }
}
