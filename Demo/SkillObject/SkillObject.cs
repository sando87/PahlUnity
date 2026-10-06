using System;
using UnityEngine;

namespace PahlUnity.Demo
{
    public enum SkillInputState { None, JustDown, Pressing, JustUp }

    public class SkillObject : MonoBehaviour
    {
        [SerializeField] SkillSpecData _SkillSpecData = null;

        protected BaseObject mBaseObject;
        protected SpecBase mSkillSpec;
        protected SkillInstData mSkillInstData;
        protected PlayerController2D mPlayerCtrl = null;

        public long ResourceID => _SkillSpecData.ResourceID;
        public SkillInstData InstData => mSkillInstData;
        protected SkillInputState InputState { get; private set; } = SkillInputState.None;

        protected virtual void Awake()
        {
            mBaseObject = this.ExGetBase();
            mSkillSpec = GetComponent<SpecBase>();
            mPlayerCtrl = mBaseObject.GetComp<PlayerController2D>();
        }

        public void Init(SkillSaveData saveData)
        {
            mSkillInstData = new SkillInstData(_SkillSpecData, saveData);

            mSkillSpec.SetSpecs(mSkillInstData.SpecData.Specs, 0);

            mSkillSpec.UpdateAllValuesByStep(mSkillInstData.LevelIndex);
        }

        public virtual void OnEquip(int positionIndex)
        {
            mSkillInstData.SaveData.IsEquipped = true;
            mSkillInstData.SaveData.PositionIndex = positionIndex;
        }
        public virtual void OnUnequip()
        {
            mSkillInstData.SaveData.IsEquipped = false;
            mSkillInstData.SaveData.PositionIndex = -1;
        }
        public void AddSkillPoint()
        {
            mSkillInstData.SaveData.Level++;
            mSkillSpec.UpdateAllValuesByStep(mSkillInstData.LevelIndex);
        }
        public virtual void OnInputDown()
        {
            InputState = SkillInputState.JustDown;
        }
        public virtual void OnInputPressing()
        {
            InputState = SkillInputState.Pressing;
        }
        public virtual void OnInputUp()
        {
            InputState = SkillInputState.JustUp;
        }


        public void DoDamage(Collider2D col)
        {
            float damage = 1;
            Health health = col.ExGetBase().Health;
            if (health != null)
            {
                health.GetDamaged(new DamageInfo(damage), mBaseObject);

                if (health.IsDead)
                {
                    EventManager.Instance.GlobalEvents.InvokeEvent(new KillEnemy(10));
                }
            }
        }

        protected float GetAttackSpeedMultiplier()
        {
            float percentModifier = mBaseObject.Spec.GetPercentModifier(SpecFields.AttackSpeed);
            float multiplier = percentModifier / 100f;
            float finalMultiplier = multiplier > 0 ? 1f + multiplier : (1 / (1f - multiplier));
            return finalMultiplier;
        }
    }
}
