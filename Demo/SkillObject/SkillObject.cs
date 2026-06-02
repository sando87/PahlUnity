using System;
using UnityEngine;

namespace PahlUnity.Demo
{
    public class SkillObject : MonoBehaviour
    {
        [SerializeField] private SkillSpecData _SpecData;

        private SpecBaseMono mSpecBase;
        private SkillSaveData mSaveData;

        public void Init(SkillSaveData saveData)
        {
            mSaveData = saveData;

            mSpecBase = GetComponent<SpecBaseMono>();
            mSpecBase.Init(_SpecData.Specs, new System.Random());
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
