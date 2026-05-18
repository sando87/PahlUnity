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
    }
}
