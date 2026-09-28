using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlUnity.Demo
{
    public class SkillController : MonoBehaviour
    {
        [SerializeField] SkillObject[] _SkillSlots = null;

        BaseObject mBaseObject = null;

        void Awake()
        {
            mBaseObject = this.ExGetBase();
        }

        void Update()
        {
            DoInputSlot(InputActionNameHash.SkillSlotA, _SkillSlots[0]);
            DoInputSlot(InputActionNameHash.SkillSlotB, _SkillSlots[1]);
            DoInputSlot(InputActionNameHash.SkillSlotC, _SkillSlots[2]);
            DoInputSlot(InputActionNameHash.SkillSlotD, _SkillSlots[3]);
        }

        void DoInputSlot(int inputType, SkillObject skillObject)
        {
            if (mBaseObject.Input.JustPressed(inputType))
            {
                skillObject.OnInputDown();
            }
            else if (mBaseObject.Input.JustReleased(inputType))
            {
                skillObject.OnInputUp();
            }
            else if (mBaseObject.Input.IsPressing(inputType))
            {
                skillObject.OnInputPressing();
            }
        }
    }
}