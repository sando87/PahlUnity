using System;
using NaughtyAttributes;
using UnityEngine;

namespace PahlUnity
{
    public class SpecValue
    {
        public SpecValueInfo Info;
        public float Value;
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

        bool IsBaseStepValueType => _ValueType == SpecValueType.BaseStep;
        [SerializeField, AllowNesting, ShowIf(nameof(IsBaseStepValueType))] private float _Base;
        [SerializeField, AllowNesting, ShowIf(nameof(IsBaseStepValueType))] private float _Step;

        bool IsCurveValueType => _ValueType == SpecValueType.Curve;
        [SerializeField, AllowNesting, ShowIf(nameof(IsCurveValueType))] private AnimationCurve _Curve = null;

        bool IsCustomValueType => _ValueType == SpecValueType.Custom;
        [SerializeField, AllowNesting, ShowIf(nameof(IsCustomValueType))] private string _CustomValue = null;

        public string KeyName => _Key;
        public SpecModifierType ModifierType => _ModifierType;
        public SpecValueType ValueType => _ValueType;

        public bool HasValidKey()
        {
            return !string.IsNullOrWhiteSpace(_Key);
        }

        public float GetValue()
        {
            if (IsNoneValueType)
            {
                return _Value;
            }
            else if (IsMinMaxValueType)
            {
                return UnityEngine.Random.Range(_Min, _Max);
            }
            else if (IsCurveValueType)
            {
                float maxTime = _Curve.keys[_Curve.length - 1].time;
                float randomTime = UnityEngine.Random.Range(0f, maxTime);
                return _Curve.Evaluate(randomTime);
            }
            else if (IsBaseStepValueType)
            {
                return _Base;
            }
            return 0f;
        }
        public float GetValue(System.Random random)
        {
            if (IsNoneValueType)
            {
                return _Value;
            }
            else if (IsMinMaxValueType)
            {
                // Next(int) 는 0 ~ int.MaxValue-1
                int ranVal = random.Next(int.MaxValue);
                float normalizedTime = (float)ranVal / (int.MaxValue - 1); // 0 ~ 1 포함
                float val = _Min + (_Max - _Min) * normalizedTime;
                return val;
            }
            else if (IsCurveValueType)
            {
                int ranVal = random.Next(int.MaxValue);
                float normalizedTime = (float)ranVal / (int.MaxValue - 1); // 0 ~ 1 포함
                float maxTime = _Curve.keys[_Curve.length - 1].time;
                return _Curve.Evaluate(normalizedTime * maxTime);
            }
            return 0f;
        }
        public float GetValueByStep(int step)
        {
            if (IsBaseStepValueType)
            {
                return _Base + (_Step * step);
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
        BaseStep,
        Curve,
        Custom,
    }
}
