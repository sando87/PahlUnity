using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PahlBit
{

    public class PlayerUnitAI : MonoBehaviour
    {
        [SerializeField] VirtualInput _VirtualInput = null;

        public VirtualInput VirtualInput => _VirtualInput;

        [Button("Jump")]
        void Jump()
        {
            VirtualInput.Tap(PlayerUnitInputType.Jump);
        }

        [Button("Move Right")]
        void MoveRight()
        {
            VirtualInput.SetValue(PlayerUnitInputType.Move, new Vector2(1, 0));
        }

        [Button("Move Left")]
        void MoveLeft()
        {
            VirtualInput.SetValue(PlayerUnitInputType.Move, new Vector2(-1, 0));
        }

        [Button("Move Stop")]
        void MoveStop()
        {
            VirtualInput.SetValue(PlayerUnitInputType.Move, new Vector2(0, 0));
        }

        [Button("AttackA")]
        void AttackA()
        {
            VirtualInput.Tap(PlayerUnitInputType.SkillSlotA);
        }
        [Button("AttackB")]
        void AttackB()
        {
            VirtualInput.Tap(PlayerUnitInputType.SkillSlotB);
        }
        [Button("AttackStartC")]
        void AttackStartC()
        {
            VirtualInput.Press(PlayerUnitInputType.SkillSlotC);
        }
        [Button("AttackStopC")]
        void AttackStopC()
        {
            VirtualInput.Release(PlayerUnitInputType.SkillSlotC);
        }
    }

}