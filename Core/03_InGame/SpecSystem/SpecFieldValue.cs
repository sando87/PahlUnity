using UnityEngine;

namespace PahlUnity
{
    public class SpecFieldValue
    {
        private readonly SpecFieldRaw mInfo;
        private float mBaseValue;
        private float mCurrentValue;
        private int mCurrentStepCount;

        public SpecFieldRaw Info => mInfo;
        public float BaseValue => mBaseValue;
        public float CurrentValue => mCurrentValue;
        public int CurrentStepCount => mCurrentStepCount;

        public SpecFieldValue(SpecFieldRaw info)
        {
            mInfo = info;
            mBaseValue = info.GetValue();
            mCurrentValue = mBaseValue;
            mCurrentStepCount = 0;
        }
        public SpecFieldValue(SpecFieldRaw info, float normalizedRange)
        {
            mInfo = info;
            mBaseValue = info.GetValue(normalizedRange);
            mCurrentValue = mBaseValue;
            mCurrentStepCount = 0;
        }
        public SpecFieldValue(SpecFieldRaw info, System.Random random)
        {
            mInfo = info;
            mBaseValue = info.GetValue(random);
            mCurrentValue = mBaseValue;
            mCurrentStepCount = 0;
        }
        public void UpdateBaseValue(float normalizedRange)
        {
            mBaseValue = mInfo.GetValue(normalizedRange);
            mCurrentValue = mBaseValue + (mInfo.Step * mCurrentStepCount);
        }
        public void UpdateCurrentValue(int step)
        {
            mCurrentStepCount = step;
            mCurrentValue = mBaseValue + (mInfo.Step * mCurrentStepCount);
        }
    }
}
