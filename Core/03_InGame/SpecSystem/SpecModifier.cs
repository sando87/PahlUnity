using System;
using System.Collections.Generic;
using UnityEngine;

namespace PahlUnity
{
    [Serializable]
    public struct SpecModifierSum
    {
        public string Key;
        public float Add;
        public float Percent;
    }

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

        public bool HasModifier(int key)
        {
            return mSpecs.ContainsKey(key);
        }

        public float GetAddModifier(int key)
        {
            float totalModifier = 0f;
            if (mSpecs.TryGetValue(key, out HashSet<SpecFieldValue> specSet))
            {
                foreach (var spec in specSet)
                {
                    if (spec.Info.ModifierType == SpecModifierType.Base)
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

        public void GetModifierSums(List<SpecModifierSum> results)
        {
            results.Clear();
            foreach (KeyValuePair<int, HashSet<SpecFieldValue>> pair in mSpecs)
            {
                SpecModifierSum sum = new SpecModifierSum();
                sum.Key = GetModifierKeyName(pair.Value, pair.Key);
                sum.Add = GetAddModifier(pair.Key);
                sum.Percent = GetPercentModifier(pair.Key);
                results.Add(sum);
            }

            results.Sort(CompareModifierSum);
        }

        static string GetModifierKeyName(HashSet<SpecFieldValue> specSet, int fieldKey)
        {
            foreach (SpecFieldValue spec in specSet)
            {
                SpecFieldRaw info = spec.Info;
                if (info.HasValidKey())
                    return info.Key;
            }

            return fieldKey.ToString();
        }

        static int CompareModifierSum(SpecModifierSum left, SpecModifierSum right)
        {
            return string.CompareOrdinal(left.Key, right.Key);
        }
    }

}
