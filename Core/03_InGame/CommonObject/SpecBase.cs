using System;
using UnityEngine;
using System.Collections.Generic;

namespace PahlUnity
{
    public class SpecBase : MonoBehaviour
    {
        private readonly Dictionary<int, SpecFieldValue> mSpecs = new();

        private readonly List<SpecModifier> mModifiers = new();

        public void AddSpecs(IReadOnlyList<SpecFieldRaw> specs, float normalizedRange)
        {
            foreach (var spec in specs)
            {
                SpecFieldValue specValue = new(spec, normalizedRange);
                mSpecs[spec.FieldKey] = specValue;
            }
        }
        public void AddSpecs(IReadOnlyList<SpecFieldRaw> specs, System.Random random)
        {
            foreach (var spec in specs)
            {
                SpecFieldValue specValue = new(spec, random);
                mSpecs[spec.FieldKey] = specValue;
            }
        }

        public void UpdateAllBaseValues(float normalizedRange)
        {
            foreach (var kvp in mSpecs)
            {
                SpecFieldValue specValue = kvp.Value;
                specValue.UpdateBaseValue(normalizedRange);
            }
        }

        public void UpdateAllValuesByStep(int step)
        {
            foreach (var kvp in mSpecs)
            {
                SpecFieldValue specValue = kvp.Value;
                specValue.UpdateCurrentValue(step);
            }
        }
        public void UpdateCurrentValueByStep(int key, int step)
        {
            if (mSpecs.TryGetValue(key, out SpecFieldValue specValue))
            {
                specValue.UpdateCurrentValue(step);
            }
        }

        public void AddModifier(SpecModifier modifier)
        {
            if (!mModifiers.Contains(modifier))
            {
                mModifiers.Add(modifier);
            }
        }


        public float this[int key] => GetValue(key);

        public float GetValue(int key)
        {
            if (mSpecs.TryGetValue(key, out SpecFieldValue spec))
            {
                float value = spec.CurrentValue;

                float addModifier = 0f;
                foreach (var modifier in mModifiers)
                {
                    addModifier += modifier.GetAddModifier(key);
                }

                float percentModifier = 0;
                foreach (var modifier in mModifiers)
                {
                    percentModifier += modifier.GetPercentModifier(key);
                }

                float multiplier = percentModifier / 100f;
                float finalMultiplier = multiplier > 0 ? 1f + multiplier : (1 / (1f - multiplier));

                return (value + addModifier) * finalMultiplier;
            }
            return 0f;
        }
    }
}
