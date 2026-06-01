using System;
using NaughtyAttributes;
using UnityEngine;

namespace PahlUnity
{
    public class SpecValue
    {
        public SpecValueInfo Info;
        public float BaseValue;
        public float CurrentValue;
    }

    [Serializable]
    public class SpecValueInfo
    {
        [SerializeField, SpecFieldSelector] private string _Key = "";
        [SerializeField] private SpecModifierType _ModifierType = SpecModifierType.Base;
        [SerializeField] private SpecValueType _ValueType = SpecValueType.Simple;

        bool IsNoneValueType => _ValueType == SpecValueType.Simple;
        [SerializeField, AllowNesting, ShowIf(nameof(IsNoneValueType))] private float _Value;

        bool IsMinMaxValueType => _ValueType == SpecValueType.MinMax;
        [SerializeField, AllowNesting, ShowIf(nameof(IsMinMaxValueType))] private float _Min;
        [SerializeField, AllowNesting, ShowIf(nameof(IsMinMaxValueType))] private float _Max;

        bool IsCurveValueType => _ValueType == SpecValueType.Curve;
        [SerializeField, AllowNesting, ShowIf(nameof(IsCurveValueType))] private AnimationCurve _Curve = null;

        bool IsCustomValueType => _ValueType == SpecValueType.Custom;
        [SerializeField, AllowNesting, ShowIf(nameof(IsCustomValueType))] private string _CustomValue = null;

        [SerializeField] private float _Step = 0f;

        public string KeyName => _Key;
        public SpecModifierType ModifierType => _ModifierType;
        public SpecValueType ValueType => _ValueType;
        public float Step => _Step;

        public bool HasValidKey()
        {
            return !string.IsNullOrWhiteSpace(_Key);
        }

        public float GetValue()
        {
            float normalizedRange = MyUtils.RandomNormalizedFloat();
            return GetValue(normalizedRange);
        }
        public float GetValue(System.Random random)
        {
            int ranVal = random.Next(int.MaxValue);
            float normalizedRange = (float)ranVal / (int.MaxValue - 1);
            return GetValue(normalizedRange);
        }
        public float GetValue(float normalizedRange)
        {
            if (IsNoneValueType)
            {
                return _Value;
            }
            else if (IsMinMaxValueType)
            {
                float val = _Min + (_Max - _Min) * normalizedRange;
                return val;
            }
            else if (IsCurveValueType)
            {
                return _Curve.Evaluate(normalizedRange);
            }
            return 0f;
        }
        public float GetValueByCustom(Action<string> parser)
        {
            if (IsCustomValueType)
            {
                parser?.Invoke(_CustomValue);
            }
            return 0f;
        }


    }

    public enum SpecModifierType
    {
        Base,
        Additive,
        Multiply,
    }
    public enum SpecValueType
    {
        Simple,
        MinMax,
        Curve,
        Custom,
    }
}
