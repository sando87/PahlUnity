using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;


namespace PahlUnity
{
    public class PlayerStateBase : FiniteStateBase
    {
        public InputPlayer PlayerInput { get => GetComponentInParent<InputPlayer>(); }
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
            // if (!Base.Ctrl.IsGrounded)
            // {
            //     Base.StateMachine.TryChangeState<PlayerStateFloating>();
            // }
            // else
            // {
            //     float moveX = PlayerInput.MoveX;
            //     if (moveX.ExIsAlmostZero())
            //     {
            //         Base.StateMachine.TryChangeState<PlayerStateIdle>();
            //     }
            //     else
            //     {
            //         Base.StateMachine.TryChangeState<PlayerStateWalk>();
            //     }
            // }
        }


    }
}
