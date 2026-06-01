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
        private const string SpecKeyNameProperty = "_Name";

        private readonly List<string> mOptions = new List<string>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty targetProperty = GetTargetProperty(property);

            if (targetProperty == null || targetProperty.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Use with string or SpecKey.");
                return;
            }

            RefreshOptions();

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

        private void RefreshOptions()
        {
            mOptions.Clear();
            mOptions.Add(EmptyLabel);

            SpecFieldSelectorAttribute selectorAttribute = (SpecFieldSelectorAttribute)attribute;
            string typeName = nameof(SpecFieldDefinition);
            string[] guids = AssetDatabase.FindAssets($"t:{typeName}", new[] { "Assets" });
            HashSet<string> fieldSet = new HashSet<string>();

            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                SpecFieldDefinition definition = AssetDatabase.LoadAssetAtPath<SpecFieldDefinition>(assetPath);
                if (definition == null)
                {
                    continue;
                }

                IReadOnlyList<string> fields = definition.Fields;

                for (int fieldIndex = 0; fieldIndex < fields.Count; fieldIndex++)
                {
                    string field = fields[fieldIndex];

                    if (string.IsNullOrWhiteSpace(field))
                    {
                        continue;
                    }

                    string fieldName = field.Trim();

                    if (fieldSet.Add(fieldName))
                    {
                        mOptions.Add(fieldName);
                    }
                }
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
    }
#endif
}
