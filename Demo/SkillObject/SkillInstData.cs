using System;
using System.Collections.Generic;
using UnityEngine;

namespace PahlUnity.Demo
{
    public class SkillInstData
    {
        private SkillSpecData mSpecRawData;
        private int mResourceID;
        private long mInstanceID;
        private int mLevel;

        private IReadOnlyList<SpecFieldValue> mSpecFieldValues = null;

        public SkillInstData(SkillSpecData specData, int level = 1)
        {
            mSpecRawData = specData;
            mResourceID = mSpecRawData.SkillID.ExGetStableHash32();
            mInstanceID = DateTime.Now.Ticks;
            mLevel = level;
        }
        public SkillInstData(SkillSpecData specData, long instanceID, int level = 1)
        {
            mSpecRawData = specData;
            mResourceID = mSpecRawData.SkillID.ExGetStableHash32();
            mInstanceID = instanceID;
            mLevel = level;
        }

        public int ResourceID => mResourceID;
        public long InstanceID => mInstanceID;
        public int RandomSeed => (int)mInstanceID;
        public int Level => mLevel;
        public int LevelIndex => mLevel - 1;

        public void AddSkillPoint()
        {
            mLevel++;
        }
        public SkillSpecData SpecData => mSpecRawData;

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
