using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlUnity.Demo
{
    public class SkillController : MonoBehaviour
    {
        [SerializeField, NaughtyAttributes.ReadOnly]
        SkillObject[] mSkillSlots = null;

        BaseObject mBaseObject = null;
        SkillObject[] mAllSkillTree = null;

        void Awake()
        {
            mBaseObject = this.ExGetBase();
            mAllSkillTree = GetComponentsInChildren<SkillObject>();
            mSkillSlots = new SkillObject[4];
        }

        public void Init(PlayerData playerSaveData)
        {
            long idCounter = 0;
            bool needSave = false;
            Dictionary<long, SkillSaveData> skillSaveData = playerSaveData.Skills;
            foreach (SkillObject skill in mAllSkillTree)
            {
                if (skillSaveData.ContainsKey(skill.ResourceID))
                {
                    skill.Init(skillSaveData[skill.ResourceID]);
                    if (skill.InstData.SaveData.IsEquipped)
                    {
                        mSkillSlots[skill.InstData.SaveData.PositionIndex] = skill;
                    }
                }
                else
                {
                    SkillSaveData newSaveData = new SkillSaveData(DateTime.Now.Ticks + idCounter, skill.ResourceID);
                    skill.Init(newSaveData);
                    playerSaveData.Skills[skill.ResourceID] = newSaveData;
                    needSave = true;
                    idCounter++;
                }
            }

            if (needSave)
            {
                EventManager.Instance.GlobalEvents.InvokeEvent(new SaveUserPlayData(true));
            }
        }

        void Update()
        {
            DoInputSlot(InputActionNameHash.SkillSlotA, mSkillSlots[0]);
            DoInputSlot(InputActionNameHash.SkillSlotB, mSkillSlots[1]);
            DoInputSlot(InputActionNameHash.SkillSlotC, mSkillSlots[2]);
            DoInputSlot(InputActionNameHash.SkillSlotD, mSkillSlots[3]);
        }

        void DoInputSlot(int inputType, SkillObject skillObject)
        {
            if (skillObject == null)
                return;

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

        public void AddSkillPoint(SkillObject skill)
        {
            skill.AddSkillPoint();

            EventManager.Instance.GlobalEvents.InvokeEvent(new SaveUserPlayData(true));
        }

        public void EquipSkill(SkillObject skill)
        {
            for (int i = 0; i < mSkillSlots.Length; i++)
            {
                if (mSkillSlots[i] == null)
                {
                    mSkillSlots[i] = skill;
                    skill.OnEquip(i);

                    EventManager.Instance.GlobalEvents.InvokeEvent(new SaveUserPlayData(true));
                    break;
                }
            }
        }

        public void UnequipSkill(SkillObject skill)
        {
            for (int i = 0; i < mSkillSlots.Length; i++)
            {
                if (mSkillSlots[i] == skill)
                {
                    mSkillSlots[i] = null;
                    skill.OnUnequip();

                    EventManager.Instance.GlobalEvents.InvokeEvent(new SaveUserPlayData(true));
                    break;
                }
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
            IsEquipped = skill.InstData.IsEquipped;
        }
    }
}