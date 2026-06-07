using System;
using UnityEngine;
using System.Collections.Generic;

namespace PahlUnity
{
    public class SpecModifier : MonoBehaviour
    {
        private readonly Dictionary<int, HashSet<SpecFieldValue>> mSpecs = new();

        public void AddModifier(IReadOnlyList<SpecFieldValue> specs)
        {
            foreach (var spec in specs)
            {
                AddModifier(spec);
            }
        }
        public void RemoveModifier(IReadOnlyList<SpecFieldValue> specs)
        {
            foreach (var spec in specs)
            {
                RemoveModifier(spec);
            }
        }

        public void AddModifier(SpecFieldValue spec)
        {
            int key = spec.Info.FieldKey;
            if (!mSpecs.ContainsKey(key))
            {
                mSpecs[key] = new HashSet<SpecFieldValue>();
            }
            mSpecs[key].Add(spec);
        }
        public void RemoveModifier(SpecFieldValue spec)
        {
            int key = spec.Info.FieldKey;
            if (mSpecs.TryGetValue(key, out HashSet<SpecFieldValue> specSet))
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
            if (mSpecs.TryGetValue(key, out HashSet<SpecFieldValue> specSet))
            {
                foreach (var spec in specSet)
                {
                    if (spec.Info.ModifierType == SpecModifierType.Additive)
                        totalModifier += spec.CurrentValue;
                }
            }
            return totalModifier;
        }

        public float GetPercentModifier(int key)
        {
            float totalPercent = 0;
            if (mSpecs.TryGetValue(key, out HashSet<SpecFieldValue> specSet))
            {
                foreach (var spec in specSet)
                {
                    if (spec.Info.ModifierType == SpecModifierType.Percent)
                        totalPercent += spec.CurrentValue;
                }
            }
            return totalPercent;
        }
    }

}
