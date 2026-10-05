#if UNITY_EDITOR
using NaughtyAttributes.Editor;
using UnityEditor;
using UnityEngine;

namespace PahlUnity.Demo
{
    [CustomPropertyDrawer(typeof(SkillViewerOnInspector))]
    public class SkillViewerDrawer : PropertyDrawer
    {
        const float UpgradeButtonWidth = 52f;
        const float EquipButtonWidth = 52f;
        const float SkillPointWidth = 36f;
        const float FieldSpacing = 4f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty nameProperty = property.FindPropertyRelative("Name");
            SerializedProperty skillPointProperty = property.FindPropertyRelative("SkillPoint");
            SerializedProperty equippedProperty = property.FindPropertyRelative("IsEquipped");
            SerializedProperty skillProperty = property.FindPropertyRelative("SkillObj");

            SkillObject skill = skillProperty != null ? skillProperty.objectReferenceValue as SkillObject : null;
            bool isEquipped = skill != null ? skill.IsEquipped : equippedProperty != null && equippedProperty.boolValue;

            Rect equipRect = new Rect(position.xMax - EquipButtonWidth, position.y, EquipButtonWidth, position.height);
            Rect upgradeRect = new Rect(equipRect.x - FieldSpacing - UpgradeButtonWidth, position.y, UpgradeButtonWidth, position.height);
            Rect pointRect = new Rect(upgradeRect.x - FieldSpacing - SkillPointWidth, position.y, SkillPointWidth, position.height);
            float nameWidth = Mathf.Max(0f, pointRect.x - position.x - FieldSpacing);
            Rect nameRect = new Rect(position.x, position.y, nameWidth, position.height);

            bool previousEnabled = GUI.enabled;
            GUI.enabled = true;

            string skillName = nameProperty != null ? nameProperty.stringValue : string.Empty;
            int skillPoint = skillPointProperty != null ? skillPointProperty.intValue : 0;
            if (skill != null && skill.InstData != null)
                skillPoint = skill.InstData.Level;

            EditorGUI.LabelField(nameRect, skillName);
            EditorGUI.LabelField(pointRect, skillPoint.ToString());

            if (GUI.Button(upgradeRect, "강화") && skill != null)
            {
                skill.AddSkillPoint();
                RefreshList(property);
            }

            string equipLabel = isEquipped ? "해제" : "장착";
            if (GUI.Button(equipRect, equipLabel) && skill != null)
            {
                if (isEquipped)
                    skill.OnUnequip();
                else
                    skill.OnEquip();

                RefreshList(property);
            }

            GUI.enabled = previousEnabled;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        static void RefreshList(SerializedProperty property)
        {
            SkillController skillController = property.serializedObject.targetObject as SkillController;
            if (skillController == null)
                return;

            skillController.UpdateSkillList();
        }
    }

    [CustomEditor(typeof(SkillController))]
    public class SkillControllerEditor : NaughtyInspector
    {
        override protected void OnEnable()
        {
            SkillController skillController = (SkillController)target;
            if (Application.isPlaying)
                skillController.UpdateSkillList();

            base.OnEnable();
        }
    }
}
#endif
