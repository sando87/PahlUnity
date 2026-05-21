using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

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
        public bool IsEnoughMana => Spec.ManaUse <= mBaseObj.Health.CurrentMana;
        public int EnforceCost => 0; // IsLearned ? (20 + (mSkillSaveData.LevelIndex * 15) + ((CurrentSubStep + 1) * 5)) : 20;

        protected BaseObject mBaseObj = null;
        protected InputPlayer mInput = null;

        private SkillSaveData mSkillSaveData = null;
        private float mCooltime = 0;

        public SpecSkill Spec { get; private set; } = null;
        protected void StartCooltime() { mCooltime = Time.time; }
        protected void UseMana() { mBaseObj.Health.UseMana(Spec.ManaUse); }

        void Awake()
        {
            mBaseObj = this.ExGetBase();
            mInput = mBaseObj.Input;
        }

        public void InitSkillInfo(int characterID)
        {
            // UserSaveData saveData = SaveFileManager<UserSaveData>.Load();
            // var saveDataAllSkills = saveData.Characters[characterID].Skills;
            // if (!saveDataAllSkills.ContainsKey(_ResourceID))
            // {
            //     saveDataAllSkills[_ResourceID] = new SkillSaveData(_ResourceID);
            // }

            // mSkillSaveData = saveDataAllSkills[_ResourceID];

            Spec = GetComponentInChildren<SpecSkill>();
            Spec.Init(characterID, _ResourceID);
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
            GameSystem.DoSave_UserSaveData();
        }
        public virtual void OnLearnedSkill()
        {
            mSkillSaveData.IsLearned = true;
            GameSystem.DoSave_UserSaveData();
        }
        public virtual void OnEquipedSkill(int slotIndex)
        {
            mSkillSaveData.IsEquipped = true;
            mSkillSaveData.PositionIndex = slotIndex;
            GameSystem.DoSave_UserSaveData();
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
            GameSystem.DoSave_UserSaveData();
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
