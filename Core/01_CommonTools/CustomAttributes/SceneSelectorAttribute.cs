using System;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
#endif

namespace PahlUnity
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SceneSelectorAttribute : PropertyAttribute
    {
        public SceneSelectorAttribute()
        {
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SceneSelectorAttribute))]
    public class SceneSelectorDrawer : PropertyDrawer
    {
        private string[] _sceneNames;

        private void Init()
        {
            if (_sceneNames != null)
                return;

            _sceneNames = EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s =>
                {
                    string path = s.path;
                    return System.IO.Path.GetFileNameWithoutExtension(path);
                })
                .ToArray();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Init();

            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Use [SceneName] with string.");
                return;
            }

            int currentIndex = Mathf.Max(0, System.Array.IndexOf(_sceneNames, property.stringValue));

            int selectedIndex = EditorGUI.Popup(
                position,
                label.text,
                currentIndex,
                _sceneNames
            );

            property.stringValue = _sceneNames[selectedIndex];
        }
    }
#endif

}