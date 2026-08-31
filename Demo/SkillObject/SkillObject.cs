using System;
using UnityEngine;

namespace PahlUnity.Demo
{
    public class SkillObject : MonoBehaviour
    {
        private SkillInstData mSkillInstData;

        private SpecBase mSpecBase;

        public void Init(SkillInstData instData)
        {
            mSkillInstData = instData;

            mSpecBase = GetComponent<SpecBase>();

            mSpecBase.SetSpecs(mSkillInstData.SpecData.Specs, 0);

            mSpecBase.UpdateAllValuesByStep(mSkillInstData.LevelIndex);
        }

        void Start()
        {
            Debug.Log(mSpecBase[SpecFields.MaxHP]);
            Debug.Log(mSpecBase[SpecFields.MaxMP]);
            Debug.Log(mSpecBase[SpecFields.MoveSpeed]);
            Debug.Log(mSpecBase[SpecFields.AttackSpeed]);
        }
    }
}
