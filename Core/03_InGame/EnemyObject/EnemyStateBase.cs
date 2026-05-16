using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;


namespace PahlBit
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
            PlayAnim(AnimStateNameHash.Idle);
            Base.Phy.Velocity = Vector2.zero;
        }
        protected void Turn(float worldDir)
        {
            if (worldDir == 0) return;

            Vector3 front = worldDir > 0 ? Vector3.forward : Vector3.back;
            transform.rotation = Quaternion.LookRotation(front, transform.up);
        }
        protected void Move(float moveHoriVelocity)
        {
            Turn(moveHoriVelocity);
            Base.Phy.VelocityX = moveHoriVelocity;
            PlayAnim(AnimStateNameHash.Run);
        }
    }
}
