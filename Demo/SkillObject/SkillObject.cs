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

        protected bool IsEquipped { get; private set; } = false;
        protected SkillInputState InputState { get; private set; } = SkillInputState.None;

        protected virtual void Awake()
        {
            mBaseObject = this.ExGetBase();
            mSkillSpec = GetComponent<SpecBase>();
            mPlayerCtrl = mBaseObject.GetComp<PlayerController2D>();
        }

        protected virtual void Start()
        {
            Init();
        }

        void Init()
        {
            mSkillInstData = new SkillInstData(_SkillSpecData);

            mSkillSpec.SetSpecs(mSkillInstData.SpecData.Specs, 0);

            mSkillSpec.UpdateAllValuesByStep(mSkillInstData.LevelIndex);
        }

        public virtual void OnEquip()
        {
            IsEquipped = true;
        }
        public virtual void OnUnequip()
        {
            IsEquipped = false;
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
