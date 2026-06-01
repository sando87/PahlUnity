using System;
using UnityEngine;
using System.Collections.Generic;

namespace PahlUnity
{
    public class SpecBaseMono : MonoBehaviour
    {
        private Dictionary<int, SpecValue> mSpecs = new Dictionary<int, SpecValue>();

        private List<SpecModifierMono> mModifiers = new List<SpecModifierMono>();

        public void Init(IReadOnlyList<SpecValueInfo> specs)
        {
            foreach (var spec in specs)
            {
                int key = StableHash.ToInt32(spec.KeyName);
                SpecValue specValue = new SpecValue();
                specValue.Info = spec;
                specValue.Value = specValue.Info.GetValue();
                mSpecs[key] = specValue;
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
                float value = spec.Value;

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
