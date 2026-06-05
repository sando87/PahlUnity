using System;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
#endif

namespace PahlUnity
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SpecFieldSelectorAttribute : PropertyAttribute
    {
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SpecFieldSelectorAttribute))]
    public class SpecFieldSelectorDrawer : PropertyDrawer
    {
        private const string EmptyLabel = "<Empty>";
        private const string MissingLabel = "<Missing> ";
        private const string EmptyDataMessage = "No SpecFieldDefinition data found for this ProjectRoot.";
        private const string SpecKeyNameProperty = "_Name";
        private static readonly HashSet<string> sLoggedEmptyProjectRoots = new HashSet<string>();

        private readonly List<string> mOptions = new List<string>();
        private string mProjectRoot = "Assets";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty targetProperty = GetTargetProperty(property);

            if (targetProperty == null || targetProperty.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Use with string or SpecKey.");
                return;
            }

            RefreshOptions(property);

            string currentValue = targetProperty.stringValue;
            int currentIndex = GetCurrentIndex(currentValue);

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();

            int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, mOptions.ToArray());

            if (EditorGUI.EndChangeCheck())
            {
                if (selectedIndex <= 0)
                {
                    targetProperty.stringValue = "";
                }
                else if (!mOptions[selectedIndex].StartsWith(MissingLabel, StringComparison.Ordinal))
                {
                    targetProperty.stringValue = mOptions[selectedIndex];
                }
            }

            EditorGUI.EndProperty();
        }

        private SerializedProperty GetTargetProperty(SerializedProperty property)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                return property;
            }

            return property.FindPropertyRelative(SpecKeyNameProperty);
        }

        private void RefreshOptions(SerializedProperty property)
        {
            mOptions.Clear();
            mOptions.Add(EmptyLabel);

            mProjectRoot = SpecFieldDefinition.GetProjectRoot(property.serializedObject.targetObject);
            List<string> fields = SpecFieldDefinition.CollectFields(mProjectRoot);
            if (fields.Count == 0)
            {
                LogEmptyDataGuide(mProjectRoot);
                return;
            }

            for (int i = 0; i < fields.Count; i++)
            {
                mOptions.Add(fields[i]);
            }
        }

        private int GetCurrentIndex(string currentValue)
        {
            if (string.IsNullOrEmpty(currentValue))
            {
                return 0;
            }

            for (int i = 1; i < mOptions.Count; i++)
            {
                if (mOptions[i] == currentValue)
                {
                    return i;
                }
            }

            mOptions.Add(MissingLabel + currentValue);
            return mOptions.Count - 1;
        }

        private void LogEmptyDataGuide(string projectRoot)
        {
            if (!sLoggedEmptyProjectRoots.Add(projectRoot))
                return;

            Debug.LogWarning(
                $"[{nameof(SpecFieldSelectorAttribute)}] {EmptyDataMessage}\n" +
                $"ProjectRoot: {projectRoot}\n" +
                $"Guide: Create a '{nameof(SpecFieldDefinition)}' asset under '{projectRoot}' via " +
                $"'Assets/Create/PahlUnity/Spec Field Definition', add field names to the asset, " +
                $"then click 'Generate Spec Fields' on that asset.");
        }
    }
#endif
}
