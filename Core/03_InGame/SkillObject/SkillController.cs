using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PahlUnity
{
    public class SkillController : MonoBehaviour
    {
        [SerializeField] Dictionary<string, SkillBase> mAllSkills = new Dictionary<string, SkillBase>();
        [SerializeField] SkillBase[] SkillSlots = null;

        [Foldout("Events")]
        public UnityEvent<SkillBase> OnEquipSkill = new UnityEvent<SkillBase>();
        [Foldout("Events")]
        public UnityEvent<SkillBase> OnUnEquipSkill = new UnityEvent<SkillBase>();

        BaseObject mBaseObj = null;
        CharSaveData mPlayerStateData = null;
        SkillBase mCurrentInputSkill = null;

        public int RemainSkillPoint => mPlayerStateData.RemainSkillPoint;
        public int CurrentLevel => PlayerGrowth.GetLevelFromAccExp(mPlayerStateData.CurrentExp);

        void Awake()
        {
            mBaseObj = GetComponentInParent<BaseObject>();
        }

        public void InitSkills(CharacterSaveData charSaveData)
        {
            mPlayerStateData = charSaveData.Stats;

            SkillBase[] allSkills = GetComponentsInChildren<SkillBase>();
            foreach (SkillBase skillObj in allSkills)
            {
                skillObj.InitSkillInfo(charSaveData);
                mAllSkills[skillObj.ResourceID] = skillObj;

                if (!skillObj.IsLearned)
                {
                    skillObj.gameObject.SetActive(false);
                    continue;
                }

                if (skillObj.IsEquipped)
                {
                    SkillSlots[skillObj.PositionIndex] = skillObj;
                }
            }
        }

        public void Update()
        {
            if (mCurrentInputSkill != null)
            {
                mCurrentInputSkill.OnPressingInput();
            }
        }

        public void JustPressedSkillSlot(int slotIndex)
        {
            if (SkillSlots[slotIndex] == null)
                return;

            if (mCurrentInputSkill != null)
            {
                mCurrentInputSkill.OnReleasedInput();
            }

            mCurrentInputSkill = SkillSlots[slotIndex];
            mCurrentInputSkill.OnPressedInput();
        }
        public void JustReleasedSkillSlot(int slotIndex)
        {
            if (SkillSlots[slotIndex] == null)
                return;

            if (mCurrentInputSkill == SkillSlots[slotIndex])
            {
                mCurrentInputSkill.OnReleasedInput();
                mCurrentInputSkill = null;
            }
        }
        public void ReleaseAllSkillSlot()
        {
            if (mCurrentInputSkill != null)
            {
                mCurrentInputSkill.OnReleasedInput();
                mCurrentInputSkill = null;
            }
        }


        public SkillBase GetSkill(string skillID)
        {
            return mAllSkills.GetValueOrDefault(skillID);
        }
        public SkillBase GetEquipSkill(int slotIndex)
        {
            return SkillSlots[slotIndex];
        }
        public bool IsLockedSkill(string skillResID)
        {
            SkillBase skill = mAllSkills[skillResID];
            return skill.UnlockLevel > CurrentLevel;
        }
        public void LearnNewSkill(string skillResID)
        {
            mPlayerStateData.RemainSkillPoint--;
            mPlayerStateData.RemainSkillPoint.ExSetMinimum(0);
            SkillBase skill = mAllSkills[skillResID];
            skill.gameObject.SetActive(true);
            skill.OnLearnedSkill();
            EventManager.Instance.GlobalEvents.InvokeEvent(new SaveUserPlayData(true));
        }
        public void LevelupSkill(string skillResID)
        {
            mPlayerStateData.RemainSkillPoint--;
            mPlayerStateData.RemainSkillPoint.ExSetMinimum(0);
            SkillBase skill = mAllSkills[skillResID];
            skill.OnLevelupSkill();
            EventManager.Instance.GlobalEvents.InvokeEvent(new SaveUserPlayData(true));
        }

        public void EquipSkill(string skillResID, int slotIndex)
        {
            SkillBase skill = mAllSkills[skillResID];
            SkillSlots[slotIndex] = skill;
            SkillSlots[slotIndex].OnEquipedSkill(slotIndex);
            OnEquipSkill?.Invoke(skill);
        }

        public void UnEquipSkill(string skillResID, int slotIndex)
        {
            SkillBase skill = mAllSkills[skillResID];
            OnUnEquipSkill?.Invoke(skill);
            SkillSlots[slotIndex].OnUnEquipedSkill();
            SkillSlots[slotIndex] = null;
        }
        public int FindEmptySkillSlotIndex()
        {
            for (int i = 0; i < SkillSlots.Length; ++i)
            {
                if (SkillSlots[i] == null)
                    return i;
            }
            return -1;
        }

    }
}