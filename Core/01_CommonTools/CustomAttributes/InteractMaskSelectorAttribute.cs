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
        private const string EmptyMaskMessage = "No InteractMaskDef data found for this ProjectRoot.";
        private static readonly HashSet<string> sLoggedEmptyProjectRoots = new HashSet<string>();

        private readonly List<string> mNames = new List<string>();
        private readonly List<string> mOptions = new List<string>();
        private string mProjectRoot = "Assets";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!IsUIntProperty(property))
            {
                EditorGUI.LabelField(position, label.text, "Use InteractMaskSelector with uint fields only.");
                return;
            }

            RefreshOptions(property);

            EditorGUI.BeginProperty(position, label, property);

            if (mOptions.Count == 0)
            {
                if (property.uintValue != 0u)
                {
                    property.uintValue = 0u;
                }

                EditorGUI.HelpBox(position, $"{label.text}: {EmptyMaskMessage}", MessageType.Info);
                LogEmptyDataGuide(mProjectRoot);
                EditorGUI.EndProperty();
                return;
            }

            EditorGUI.BeginChangeCheck();

            int selectedMask = GetSelectedMask(property.uintValue);
            int nextSelectedMask = EditorGUI.MaskField(position, label, selectedMask, mOptions.ToArray());

            if (EditorGUI.EndChangeCheck())
            {
                property.uintValue = GetMaskValue(nextSelectedMask);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            RefreshOptions(property);
            if (mOptions.Count == 0)
            {
                return EditorGUIUtility.singleLineHeight * 2f;
            }

            return base.GetPropertyHeight(property, label);
        }

        private bool IsUIntProperty(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Integer
                && property.numericType == SerializedPropertyNumericType.UInt32;
        }

        private void RefreshOptions(SerializedProperty property)
        {
            mNames.Clear();
            mOptions.Clear();

            mProjectRoot = InteractMaskDef.GetProjectRoot(property.serializedObject.targetObject);
            List<string> names = InteractMaskDef.CollectNames(mProjectRoot);
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

        private void LogEmptyDataGuide(string projectRoot)
        {
            if (!sLoggedEmptyProjectRoots.Add(projectRoot))
                return;

            Debug.LogWarning(
                $"[{nameof(InteractMaskSelectorAttribute)}] {EmptyMaskMessage}\n" +
                $"ProjectRoot: {projectRoot}\n" +
                $"Guide: Create an '{nameof(InteractMaskDef)}' asset under '{projectRoot}' via " +
                $"'Assets/Create/PahlUnity/Interact Mask Definition', add mask names to the asset, " +
                $"then click 'Generate Interact Mask Defines' on that asset.");
        }
    }
#endif
}
