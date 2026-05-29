using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace PahlUnity
{
    public class SkillBase : MonoBehaviour
    {
        [SerializeField] Sprite _Icon = null;
        public Sprite Icon => _Icon;

        [SerializeField]
        [Dropdown(nameof(IDList))]
        string _ResourceID = "";
        public string ResourceID => _ResourceID;
        List<string> IDList { get => TableDataContainer<SkillResourceData>.Instance.GetAllInfo().Select(info => info.SkillID).ToList(); }

        public SkillController Controller { get => GetComponentInParent<SkillController>(); }

        public bool IsEquipped => mSkillSaveData.IsEquipped;
        public bool IsLearned => mSkillSaveData != null && mSkillSaveData.IsLearned;
        public int PositionIndex => mSkillSaveData.PositionIndex;
        public int UnlockLevel => Spec.ResourceData._UnlockLevel;
        public int Level => mSkillSaveData.Level;
        public bool IsLocked => Controller.IsLockedSkill(ResourceID);
        public int CurrentSubStep => mSkillSaveData.SubStep;
        public int MaxSubStep => Spec.ResourceData._UpgradeStep[mSkillSaveData.LevelIndex];
        public bool IsCooltime => Spec.Cooltime == 0 ? false : Time.time - mCooltime < Spec.Cooltime;
        public float CooltimeRate => IsCooltime ? (1f - ((Time.time - mCooltime) / Spec.Cooltime)) : 0f;
        public bool IsEnoughMana => Spec.ManaUse <= mHealth.CurrentMana;
        public int EnforceCost => 0; // IsLearned ? (20 + (mSkillSaveData.LevelIndex * 15) + ((CurrentSubStep + 1) * 5)) : 20;

        protected BaseObject mBaseObj = null;
        protected Health mHealth = null;

        private SkillSaveData mSkillSaveData = null;
        private float mCooltime = 0;

        public SpecSkill Spec { get; private set; } = null;
        protected void StartCooltime() { mCooltime = Time.time; }
        protected void UseMana() { mHealth.UseMana(Spec.ManaUse); }

        void Awake()
        {
            mBaseObj = this.ExGetBase();
            mHealth = mBaseObj.ExGetCompInBase<Health>();
        }

        public void InitSkillInfo(CharacterSaveData charSaveData)
        {
            var saveDataAllSkills = charSaveData.Skills;
            if (!saveDataAllSkills.ContainsKey(_ResourceID))
            {
                saveDataAllSkills[_ResourceID] = new SkillSaveData(_ResourceID);
            }

            mSkillSaveData = saveDataAllSkills[_ResourceID];

            Spec = GetComponentInChildren<SpecSkill>();
            Spec.Init(charSaveData, _ResourceID);
        }

        public virtual bool IsCastable()
        {
            return IsEnoughMana && !IsCooltime;
        }
        public virtual void StartCasting()
        {
        }
        public virtual void DoFire()
        {
        }
        public virtual void EndSkill()
        {
        }

        public virtual void OnLevelupSkill()
        {
            int newSubStep = CurrentSubStep + 1;
            if (MaxSubStep <= newSubStep)
            {
                mSkillSaveData.SubStep = 0;
                mSkillSaveData.Level++;
            }
            else
            {
                mSkillSaveData.SubStep++;
            }

            Spec.UpdateBasicStat();
            EventManager.Instance.GlobalEvents.InvokeEvent(new SaveUserPlayData(true));
        }
        public virtual void OnLearnedSkill()
        {
            mSkillSaveData.IsLearned = true;
            EventManager.Instance.GlobalEvents.InvokeEvent(new SaveUserPlayData(true));
        }
        public virtual void OnEquipedSkill(int slotIndex)
        {
            mSkillSaveData.IsEquipped = true;
            mSkillSaveData.PositionIndex = slotIndex;
            EventManager.Instance.GlobalEvents.InvokeEvent(new SaveUserPlayData(true));
        }
        public virtual void OnPressedInput()
        {
        }
        public virtual void OnPressingInput()
        {
        }
        public virtual void OnReleasedInput()
        {
        }
        public virtual void OnUnEquipedSkill()
        {
            mSkillSaveData.IsEquipped = false;
            mSkillSaveData.PositionIndex = -1;
            EventManager.Instance.GlobalEvents.InvokeEvent(new SaveUserPlayData(true));
        }

        protected void ApplySkillStatsToProjectile(ProjectileBase proj)
        {
            proj.Stats.MoveSpeed = Spec.ProjectileSpeed;
            proj.Stats.FireAngle = 0;
            proj.Stats.AttackRange = Spec.AttackRange;
            proj.Stats.SplashRange = Spec.SplashRange;
            proj.Stats.Duration = Spec.Duration;
            proj.Stats.Interval = Spec.Interval;
            proj.Stats.StartDelay = Spec.StartDelay;
        }
    }
}
