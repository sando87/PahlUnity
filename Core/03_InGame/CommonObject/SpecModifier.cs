using System;
using UnityEngine;
using System.Collections.Generic;

namespace PahlUnity
{
    public class SpecModifier : MonoBehaviour
    {
        private Dictionary<int, HashSet<SpecValue>> mSpecs = new Dictionary<int, HashSet<SpecValue>>();

        public void AddModifier(IReadOnlyList<SpecValue> specs)
        {
            foreach (var spec in specs)
            {
                AddModifier(spec);
            }
        }
        public void RemoveModifier(IReadOnlyList<SpecValue> specs)
        {
            foreach (var spec in specs)
            {
                RemoveModifier(spec);
            }
        }

        public void AddModifier(SpecValue spec)
        {
            int key = spec.Info.KeyName.ExGetStableHash32();
            if (!mSpecs.ContainsKey(key))
            {
                mSpecs[key] = new HashSet<SpecValue>();
            }
            mSpecs[key].Add(spec);
        }
        public void RemoveModifier(SpecValue spec)
        {
            int key = spec.Info.KeyName.ExGetStableHash32();
            if (mSpecs.TryGetValue(key, out HashSet<SpecValue> specSet))
            {
                specSet.Remove(spec);
                if (specSet.Count == 0)
                {
                    mSpecs.Remove(key);
                }
            }
        }

        public float GetAddModifier(int key)
        {
            float totalModifier = 0f;
            if (mSpecs.TryGetValue(key, out HashSet<SpecValue> specSet))
            {
                foreach (var spec in specSet)
                {
                    if (spec.Info.ModifierType == SpecModifierType.Additive)
                        totalModifier += spec.CurrentValue;
                }
            }
            return totalModifier;
        }

        public float GetMultiplyModifier(int key)
        {
            float totalModifier = 0;
            if (mSpecs.TryGetValue(key, out HashSet<SpecValue> specSet))
            {
                foreach (var spec in specSet)
                {
                    if (spec.Info.ModifierType == SpecModifierType.Multiply)
                        totalModifier += spec.CurrentValue;
                }
            }
            return totalModifier;
        }
    }

}
