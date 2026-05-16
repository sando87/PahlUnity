using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;


namespace PahlBit
{
    public class PlayerStateBase : FiniteStateBase
    {
        public PlayerUnitInput PlayerInput { get => GetComponentInParent<PlayerUnitInput>(); }
        public PlayerMain PlayerMain { get => GetComponentInParent<PlayerMain>(); }

        public override void EnterState(object param)
        {
            base.EnterState(param);
        }

        public override void LeaveState()
        {
            base.LeaveState();
        }

        public void DoLeaveCurrentState()
        {
            if (!Base.Ctrl.IsGrounded)
            {
                Base.StateMachine.TryChangeState<PlayerStateFloating>();
            }
            else
            {
                float moveX = PlayerInput.MoveX;
                if (moveX.ExIsAlmostZero())
                {
                    Base.StateMachine.TryChangeState<PlayerStateIdle>();
                }
                else
                {
                    Base.StateMachine.TryChangeState<PlayerStateWalk>();
                }
            }
        }


    }
}
