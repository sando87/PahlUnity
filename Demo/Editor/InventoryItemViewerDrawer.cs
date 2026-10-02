#if UNITY_EDITOR
using NaughtyAttributes.Editor;
using UnityEditor;
using UnityEngine;

namespace PahlUnity.Demo
{
    [CustomPropertyDrawer(typeof(ItemViewerOnInspector))]
    public class InventoryItemViewerDrawer : PropertyDrawer
    {
        const float EquipButtonWidth = 72f;
        const float FieldSpacing = 4f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty nameProperty = property.FindPropertyRelative("Name");
            SerializedProperty indexProperty = property.FindPropertyRelative("Index");
            SerializedProperty slotTypeProperty = property.FindPropertyRelative("SlotType");
            SerializedProperty equippedProperty = property.FindPropertyRelative("IsEquipped");
            bool isEquipped = equippedProperty != null && equippedProperty.boolValue;
            SerializedProperty baseObjProperty = property.FindPropertyRelative("BaseObj");
            BaseObject baseObj = baseObjProperty != null ? baseObjProperty.objectReferenceValue as BaseObject : null;

            Rect buttonRect = new Rect(position.xMax - EquipButtonWidth, position.y, EquipButtonWidth, position.height);
            float nameWidth = Mathf.Max(0f, buttonRect.x - position.x - FieldSpacing);
            Rect nameRect = new Rect(position.x, position.y, nameWidth, position.height);

            bool previousEnabled = GUI.enabled;
            GUI.enabled = true;

            string itemName = nameProperty != null ? nameProperty.stringValue : string.Empty;
            EditorGUI.LabelField(nameRect, itemName);

            string buttonLabel = isEquipped ? "해제" : "장착";
            if (GUI.Button(buttonRect, buttonLabel) && indexProperty != null)
            {
                if (isEquipped && slotTypeProperty != null)
                    Unequip(property, (EquipSlotType)slotTypeProperty.intValue, indexProperty.intValue);
                else if (!isEquipped)
                    Equip(property, indexProperty.intValue);

                RefreshList(property);
            }

            GUI.enabled = previousEnabled;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        static void Equip(SerializedProperty property, int slotIndex)
        {
            Inventory inventory = property.serializedObject.targetObject as Inventory;
            if (inventory == null)
                return;

            PlayerObject player = inventory.ExGetCompInBase<PlayerObject>();
            if (player == null)
                return;

            player.DoEquipItem(slotIndex);
        }

        static void Unequip(SerializedProperty property, EquipSlotType slotType, int slotIndex)
        {
            MonoBehaviour behaviour = property.serializedObject.targetObject as MonoBehaviour;
            if (behaviour == null)
                return;

            PlayerObject player = behaviour.ExGetCompInBase<PlayerObject>();
            if (player == null)
                return;

            player.DoUnequipItem(slotType, slotIndex);
        }

        static void RefreshList(SerializedProperty property)
        {
            MonoBehaviour behaviour = property.serializedObject.targetObject as MonoBehaviour;
            if (behaviour == null)
                return;

            Inventory inventory = behaviour.ExGetCompInBase<Inventory>();
            if (inventory == null)
                return;

            inventory.UpdateItemList();

            EquipmentMono equipment = behaviour.ExGetCompInBase<EquipmentMono>();
            if (equipment == null)
                return;

            equipment.UpdateEquipItemList();
        }
    }

    [CustomEditor(typeof(Inventory))]
    public class InventoryEditor : NaughtyInspector
    {
        override protected void OnEnable()
        {
            Inventory inventory = (Inventory)target;
            if (Application.isPlaying)
                inventory.UpdateItemList();

            base.OnEnable();
        }
    }

    [CustomEditor(typeof(EquipmentMono))]
    public class EquipmentMonoEditor : NaughtyInspector
    {
        override protected void OnEnable()
        {
            EquipmentMono equipment = (EquipmentMono)target;
            if (Application.isPlaying)
                equipment.UpdateEquipItemList();

            base.OnEnable();
        }
    }
}
#endif
