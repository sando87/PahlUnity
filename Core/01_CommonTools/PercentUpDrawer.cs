#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace PahlUnity
{
    [CustomPropertyDrawer(typeof(PercentUp))]
    public class PercentUpDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // private double mPercentVal; 가져오기
            var valueProp = property.FindPropertyRelative("mPercentVal");

            EditorGUI.BeginProperty(position, label, property);

            // 오른쪽에 % 표시하도록 UI 구성
            float percentWith = 40f;
            Rect valueRect = new Rect(position.x, position.y, position.width - percentWith, position.height);
            Rect percentRect = new Rect(position.x + position.width - percentWith, position.y, percentWith, position.height);

            // 값 입력
            valueProp.doubleValue = EditorGUI.DoubleField(valueRect, label, valueProp.doubleValue);

            // % 표시 (읽기 전용)
            GUI.enabled = false;
            EditorGUI.LabelField(percentRect, "%");
            GUI.enabled = true;

            EditorGUI.EndProperty();
        }
    }
}
#endif