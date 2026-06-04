using System;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
#endif

namespace PahlUnity
{
    [AttributeUsage(AttributeTargets.Field)]
    public class InteractMaskSelectorAttribute : PropertyAttribute
    {
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(InteractMaskSelectorAttribute))]
    public class InteractMaskSelectorDrawer : PropertyDrawer
    {
        private readonly List<string> mNames = new List<string>();
        private readonly List<string> mOptions = new List<string>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!IsUIntProperty(property))
            {
                EditorGUI.LabelField(position, label.text, "Use InteractMaskSelector with uint fields only.");
                return;
            }

            RefreshOptions();

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();

            int selectedMask = GetSelectedMask(property.uintValue);
            int nextSelectedMask = EditorGUI.MaskField(position, label, selectedMask, mOptions.ToArray());

            if (EditorGUI.EndChangeCheck())
            {
                property.uintValue = GetMaskValue(nextSelectedMask);
            }

            EditorGUI.EndProperty();
        }

        private bool IsUIntProperty(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Integer
                && property.numericType == SerializedPropertyNumericType.UInt32;
        }

        private void RefreshOptions()
        {
            mNames.Clear();
            mOptions.Clear();

            List<string> names = InteractMaskDef.CollectNames();
            for (int i = 0; i < names.Count; i++)
            {
                mNames.Add(names[i]);
                mOptions.Add(names[i]);
            }
        }

        private int GetSelectedMask(uint currentValue)
        {
            int selectedMask = 0;
            for (int i = 0; i < mNames.Count; i++)
            {
                uint maskValue = InteractMaskDef.GetMaskValue(i);
                if ((currentValue & maskValue) != 0u)
                {
                    selectedMask |= 1 << i;
                }
            }

            return selectedMask;
        }

        private uint GetMaskValue(int selectedMask)
        {
            uint maskValue = 0u;
            for (int i = 0; i < mNames.Count; i++)
            {
                if ((selectedMask & (1 << i)) != 0)
                {
                    maskValue |= InteractMaskDef.GetMaskValue(i);
                }
            }

            return maskValue;
        }
    }
#endif
}
