using System.Collections.Generic;
using NaughtyAttributes;
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




        [Header("===== Editor Area =====")]
        [SerializeField, NaughtyAttributes.ReadOnly]
        List<SkillViewerOnInspector> _SkillList = new List<SkillViewerOnInspector>();
        [Button("UpdateSkillList")]
        public void UpdateSkillList()
        {
            if (!Application.isPlaying)
                return;

            _SkillList.Clear();
            SkillObject[] skillObjects = GetComponentsInChildren<SkillObject>();

            foreach (SkillObject skill in skillObjects)
            {
                if (skill.InstData == null || skill.InstData.SpecData == null)
                    continue;

                SkillViewerOnInspector viewer = new SkillViewerOnInspector();
                viewer.Bind(skill, this.ExGetBase());
                _SkillList.Add(viewer);
            }
        }
    }

    [System.Serializable]
    public class SkillViewerOnInspector
    {
        public string Name;
        public int SkillPoint;
        public bool IsEquipped;
        public SkillObject SkillObj;
        public BaseObject BaseObj;

        public void Bind(SkillObject skill, BaseObject baseObj)
        {
            SkillObj = skill;
            BaseObj = baseObj;
            Name = skill.InstData.SpecData.SkillID;
            SkillPoint = skill.InstData.Level;
            IsEquipped = skill.IsEquipped;
        }
    }
}