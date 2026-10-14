using System;
using System.Collections.Generic;
using UnityEngine;

namespace PahlUnity.Demo
{
    public class SkillInstData
    {
        private SkillSpecData mSpecRawData;
        private SkillSaveData mSaveData;
        private long mResourceID;
        private long mInstanceID;

        private IReadOnlyList<SpecFieldValue> mSpecFieldValues = null;

        public SkillInstData(SkillSpecData specData)
        {
            mResourceID = mSpecRawData.SkillID.ExGetStableHash64();
            mInstanceID = DateTime.Now.Ticks;
            mSpecRawData = specData;
            mSaveData = new SkillSaveData(mInstanceID, mResourceID);
        }
        public SkillInstData(SkillSpecData specData, SkillSaveData saveData)
        {
            mSpecRawData = specData;
            mResourceID = mSpecRawData.ResourceID;
            mInstanceID = saveData.InstanceID;
            mSaveData = saveData;
        }

        public long ResourceID => mResourceID;
        public long InstanceID => mInstanceID;
        public int RandomSeed => (int)mInstanceID;
        public int Level => mSaveData.Level;
        public int LevelIndex => mSaveData.LevelIndex;
        public bool IsEquipped => mSaveData.IsEquipped;
        public SkillSpecData SpecData => mSpecRawData;
        public SkillSaveData SaveData => mSaveData;

        public IReadOnlyList<SpecFieldValue> GetSpecFieldValues()
        {
            if (mSpecFieldValues != null)
                return mSpecFieldValues;

            List<SpecFieldValue> specs = new();
            System.Random random = new(RandomSeed);
            foreach (var spec in mSpecRawData.Specs)
            {
                specs.Add(new SpecFieldValue(spec, random));
            }

            mSpecFieldValues = specs;
            return mSpecFieldValues;
        }
    }
}
