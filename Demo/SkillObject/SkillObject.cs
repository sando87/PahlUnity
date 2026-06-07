using System;
using UnityEngine;

namespace PahlUnity.Demo
{
    public class SkillObject : MonoBehaviour
    {
        private SkillInstData mSkillInstData;
        private SkillSaveData mSaveData;

        private SpecBase mSpecBase;

        public void Init(SkillInstData instData, SkillSaveData saveData)
        {
            mSkillInstData = instData;
            mSaveData = saveData;

            mSpecBase = GetComponent<SpecBase>();

            mSpecBase.AddSpecs(mSkillInstData.SpecData.Specs, 0);

            mSpecBase.UpdateAllValuesByStep(mSaveData.LevelIndex);
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
