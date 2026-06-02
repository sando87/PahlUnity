using System;
using UnityEngine;
using System.Collections.Generic;

namespace PahlUnity
{
    public class SpecBaseMono : MonoBehaviour
    {
        private Dictionary<int, SpecValue> mSpecs = new Dictionary<int, SpecValue>();

        private List<SpecModifierMono> mModifiers = new List<SpecModifierMono>();

        public void Init(IReadOnlyList<SpecValueInfo> specs, System.Random random)
        {
            foreach (var spec in specs)
            {
                int key = spec.KeyName.ExGetStableHash32();
                SpecValue specValue = new SpecValue();
                specValue.Info = spec;
                specValue.BaseValue = specValue.Info.GetValue(random);
                specValue.CurrentValue = specValue.BaseValue;
                mSpecs[key] = specValue;
            }
        }

        public void UpdateAllBaseValues(float normalizedRange)
        {
            foreach (var kvp in mSpecs)
            {
                SpecValue specValue = kvp.Value;
                specValue.BaseValue = specValue.Info.GetValue(normalizedRange);
            }
        }

        public void UpdateAllValuesByStep(int step)
        {
            foreach (var kvp in mSpecs)
            {
                SpecValue specValue = kvp.Value;
                specValue.CurrentValue = specValue.BaseValue + (specValue.Info.Step * step);
            }
        }
        public void UpdateCurrentValueByStep(int key, int step)
        {
            if (mSpecs.TryGetValue(key, out SpecValue specValue))
            {
                specValue.CurrentValue = specValue.BaseValue + (specValue.Info.Step * step);
            }
        }

        public void AddModifier(SpecModifierMono modifier)
        {
            if (!mModifiers.Contains(modifier))
            {
                mModifiers.Add(modifier);
            }
        }


        public float this[int key] => GetValue(key);

        public float GetValue(int key)
        {
            if (mSpecs.TryGetValue(key, out SpecValue spec))
            {
                float value = spec.CurrentValue;

                float addModifier = 0f;
                foreach (var Modifier in mModifiers)
                {
                    addModifier += Modifier.GetAddModifier(key);
                }

                float multiplyModifier = 0;
                foreach (var Modifier in mModifiers)
                {
                    multiplyModifier += Modifier.GetMultiplyModifier(key);
                }

                float finalMultiplier = multiplyModifier > 0 ? 1f + multiplyModifier : (1 / (1f - multiplyModifier));

                return (value + addModifier) * finalMultiplier;
            }
            return 0f;
        }
    }
}
