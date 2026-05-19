using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;


namespace PahlUnity
{
    public class EnemyStateBase : FiniteStateBase
    {
        public override void EnterState(object param)
        {
            base.EnterState(param);
        }

        public override void LeaveState()
        {
            base.LeaveState();
        }

        protected void Stop()
        {
            Base.AnimHelper.PlayAnim(AnimStateNameHash.Idle);
            Base.Phy.Velocity = Vector2.zero;
        }
        protected void Move(float moveHoriVelocity)
        {
            Base.Body.Turn(moveHoriVelocity);
            Base.Phy.VelocityX = moveHoriVelocity;
            Base.AnimHelper.PlayAnim(AnimStateNameHash.Run);
        }
    }
}
