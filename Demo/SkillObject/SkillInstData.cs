using System;
using UnityEngine;

namespace PahlUnity.Demo
{
    public class SkillInstData
    {
        private SkillSpecData mSpecData;
        private int mResourceID;
        private long mInstanceID;

        public SkillInstData(SkillSpecData specData)
        {
            mSpecData = specData;
            mResourceID = mSpecData.SkillID.ExGetStableHash32();
            mInstanceID = DateTime.Now.Ticks;
        }
        public SkillInstData(SkillSpecData specData, long instanceID)
        {
            mSpecData = specData;
            mResourceID = mSpecData.SkillID.ExGetStableHash32();
            mInstanceID = instanceID;
        }

        public int ResourceID => mResourceID;
        public long InstanceID => mInstanceID;
        public SkillSpecData SpecData => mSpecData;
    }
}
