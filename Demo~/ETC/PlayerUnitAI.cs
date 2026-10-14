using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PahlUnity.Demo
{

    public class PlayerUnitAI : MonoBehaviour
    {
        [SerializeField] VirtualInput _VirtualInput = null;

        public VirtualInput VirtualInput => _VirtualInput;

        [Button("Jump")]
        void Jump()
        {
            VirtualInput.Tap(InputActionNameHash.Jump);
        }

        [Button("Move Right")]
        void MoveRight()
        {
            VirtualInput.SetValue(InputActionNameHash.Move, new Vector2(1, 0));
        }

        [Button("Move Left")]
        void MoveLeft()
        {
            VirtualInput.SetValue(InputActionNameHash.Move, new Vector2(-1, 0));
        }

        [Button("Move Stop")]
        void MoveStop()
        {
            VirtualInput.SetValue(InputActionNameHash.Move, new Vector2(0, 0));
        }

        [Button("AttackA")]
        void AttackA()
        {
            VirtualInput.Tap(InputActionNameHash.SkillSlotA);
        }
        [Button("AttackB")]
        void AttackB()
        {
            VirtualInput.Tap(InputActionNameHash.SkillSlotB);
        }
        [Button("AttackStartC")]
        void AttackStartC()
        {
            VirtualInput.Press(InputActionNameHash.SkillSlotC);
        }
        [Button("AttackStopC")]
        void AttackStopC()
        {
            VirtualInput.Release(InputActionNameHash.SkillSlotC);
        }
    }

}