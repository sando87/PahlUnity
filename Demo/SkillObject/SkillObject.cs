using System;
using UnityEngine;

namespace PahlUnity.Demo
{
    public enum SkillInputState { None, JustDown, Pressing, JustUp }

    public class SkillObject : MonoBehaviour
    {
        [SerializeField] SkillSpecData _SkillSpecData = null;

        protected BaseObject mBaseObject;
        protected SpecBase mSpecBase;
        protected SkillInstData mSkillInstData;

        protected bool IsEquipped { get; private set; } = false;
        protected SkillInputState InputState { get; private set; } = SkillInputState.None;

        protected virtual void Awake()
        {
            mBaseObject = this.ExGetBase();
            mSpecBase = GetComponent<SpecBase>();
        }

        protected virtual void Start()
        {
            Init();

            Debug.Log(mSpecBase[SpecFields.MaxHP]);
            Debug.Log(mSpecBase[SpecFields.MaxMP]);
            Debug.Log(mSpecBase[SpecFields.MoveSpeed]);
            Debug.Log(mSpecBase[SpecFields.AttackSpeed]);
        }

        void Init()
        {
            mSkillInstData = new SkillInstData(_SkillSpecData);

            mSpecBase.SetSpecs(mSkillInstData.SpecData.Specs, 0);

            mSpecBase.UpdateAllValuesByStep(mSkillInstData.LevelIndex);
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
    }
}
