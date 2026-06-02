using System;
using UnityEngine;

namespace PahlUnity.Demo
{
    public class PlayerObject : MonoBehaviour
    {
        [SerializeField] private PlayerSpecData _SpecData;

        private SpecBaseMono mSpecBase;
        private PlayerSaveData mSaveData;

        public void Init(PlayerSaveData saveData)
        {
            mSaveData = saveData;

            mSpecBase = GetComponent<SpecBaseMono>();
            mSpecBase.Init(_SpecData.Specs, new System.Random());

            int currentLevel = 12;  // mSaveData.CurrentExp.ToLevel();
            float maxLevel = 99;
            float levelRate = currentLevel / maxLevel;
            mSpecBase.UpdateAllBaseValues(levelRate);

            mSpecBase.UpdateCurrentValueByStep(SpecFields.MaxHP, mSaveData.HealthPoint);
            mSpecBase.UpdateCurrentValueByStep(SpecFields.MaxMP, mSaveData.ManaPoint);
            mSpecBase.UpdateCurrentValueByStep(SpecFields.Attack, mSaveData.AttackPoint);
            mSpecBase.UpdateCurrentValueByStep(SpecFields.Defense, mSaveData.DefensePoint);
        }

        void Start()
        {
            Debug.Log(mSpecBase[SpecFields.MaxHP]);
            Debug.Log(mSpecBase[SpecFields.MaxMP]);
            Debug.Log(mSpecBase[SpecFields.Attack]);
            Debug.Log(mSpecBase[SpecFields.Defense]);
        }
    }
}
