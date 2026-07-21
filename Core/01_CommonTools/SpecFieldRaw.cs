using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PahlUnity
{
    [Serializable]
    public struct SpecFieldRaw
    {
        [SerializeField, SpecFieldSelector] private string _Key;
        [SerializeField] private SpecModifierType _ModifierType;
        [SerializeField] private SpecValueType _ValueType;

        bool IsNoneValueType => _ValueType == SpecValueType.Simple;
        [SerializeField] private float _Value;

        bool IsMinMaxValueType => _ValueType == SpecValueType.MinMax;
        [SerializeField] private float _Min;
        [SerializeField] private float _Max;

        bool IsCurveValueType => _ValueType == SpecValueType.Curve;
        [SerializeField] private AnimationCurve _Curve;

        bool IsListValueType => _ValueType == SpecValueType.List;
        [SerializeField] private List<float> _ListValues;

        bool IsCustomValueType => _ValueType == SpecValueType.Custom;
        [SerializeField] private string _CustomValue;

        [SerializeField] private float _Step;

        public int FieldKey => StableHash.ToInt32(_Key);
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
            else if (IsListValueType)
            {
                return _ListValues[Mathf.RoundToInt(normalizedRange * (_ListValues.Count - 1))];
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
        Percent,
    }
    public enum SpecValueType
    {
        Simple,
        MinMax,
        Curve,
        Custom,
        List,
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SpecFieldRaw))]
    public class SpecValueInfoDrawer : PropertyDrawer
    {
        private const float VerticalSpacing = 2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect currentRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(currentRect, property.isExpanded, label, true);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                DrawProperty(ref currentRect, property.FindPropertyRelative("_Key"));
                DrawProperty(ref currentRect, property.FindPropertyRelative("_ModifierType"));

                SerializedProperty valueTypeProperty = property.FindPropertyRelative("_ValueType");
                DrawProperty(ref currentRect, valueTypeProperty);

                SpecValueType valueType = (SpecValueType)valueTypeProperty.enumValueIndex;
                DrawValueProperty(ref currentRect, property, valueType);

                DrawProperty(ref currentRect, property.FindPropertyRelative("_Step"));

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;
            if (!property.isExpanded)
            {
                return height;
            }

            height += GetPropertyHeight(property.FindPropertyRelative("_Key"));
            height += GetPropertyHeight(property.FindPropertyRelative("_ModifierType"));

            SerializedProperty valueTypeProperty = property.FindPropertyRelative("_ValueType");
            height += GetPropertyHeight(valueTypeProperty);

            SpecValueType valueType = (SpecValueType)valueTypeProperty.enumValueIndex;
            height += GetValuePropertyHeight(property, valueType);
            height += GetPropertyHeight(property.FindPropertyRelative("_Step"));

            return height;
        }

        private void DrawValueProperty(ref Rect currentRect, SerializedProperty property, SpecValueType valueType)
        {
            switch (valueType)
            {
                case SpecValueType.Simple:
                    DrawProperty(ref currentRect, property.FindPropertyRelative("_Value"));
                    break;
                case SpecValueType.MinMax:
                    DrawProperty(ref currentRect, property.FindPropertyRelative("_Min"));
                    DrawProperty(ref currentRect, property.FindPropertyRelative("_Max"));
                    break;
                case SpecValueType.Curve:
                    DrawProperty(ref currentRect, property.FindPropertyRelative("_Curve"));
                    break;
                case SpecValueType.Custom:
                    DrawProperty(ref currentRect, property.FindPropertyRelative("_CustomValue"));
                    break;
                case SpecValueType.List:
                    DrawProperty(ref currentRect, property.FindPropertyRelative("_ListValues"));
                    break;
            }
        }

        private float GetValuePropertyHeight(SerializedProperty property, SpecValueType valueType)
        {
            switch (valueType)
            {
                case SpecValueType.Simple:
                    return GetPropertyHeight(property.FindPropertyRelative("_Value"));
                case SpecValueType.MinMax:
                    return GetPropertyHeight(property.FindPropertyRelative("_Min")) +
                        GetPropertyHeight(property.FindPropertyRelative("_Max"));
                case SpecValueType.Curve:
                    return GetPropertyHeight(property.FindPropertyRelative("_Curve"));
                case SpecValueType.Custom:
                    return GetPropertyHeight(property.FindPropertyRelative("_CustomValue"));
                case SpecValueType.List:
                    return GetPropertyHeight(property.FindPropertyRelative("_ListValues"));
                default:
                    return 0f;
            }
        }

        private void DrawProperty(ref Rect currentRect, SerializedProperty property)
        {
            float height = EditorGUI.GetPropertyHeight(property, true);
            currentRect.y += currentRect.height + VerticalSpacing;
            currentRect.height = height;
            EditorGUI.PropertyField(currentRect, property, true);
        }

        private float GetPropertyHeight(SerializedProperty property)
        {
            return EditorGUI.GetPropertyHeight(property, true) + VerticalSpacing;
        }
    }
#endif
}
