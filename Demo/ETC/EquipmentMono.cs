using UnityEngine;
using PahlUnity;
using System.Collections.Generic;
using System;

namespace PahlUnity.Demo
{
    [RequireComponent(typeof(SpecModifier))]
    public class EquipmentMono : MonoBehaviour
    {
        Equipment mEquipment = null;
        SpecModifier mSpecModifier = null;

        public event Action<ItemInstInfo> OnEquippedItem;
        public event Action<ItemInstInfo> OnUnequippedItem;

        public SpecModifier SpecModifier => mSpecModifier;

        void Awake()
        {
            mSpecModifier = GetComponent<SpecModifier>();
        }
        public void Init(Dictionary<EquipmentSlotType, int> slotMaxCounts)
        {
            mEquipment = new Equipment(slotMaxCounts);
            mEquipment.OnEquipped += OnEquipped;
            mEquipment.OnUnequipped += OnUnequipped;
        }
        void OnEquipped(IEquipItem item)
        {
            ItemInstInfo itemInstData = item as ItemInstInfo;
            LOG.errorif(itemInstData == null);
            SpecModifier.AddModifier(itemInstData.GetSpecFieldValues());
            OnEquippedItem?.Invoke(itemInstData);
        }
        void OnUnequipped(IEquipItem item)
        {
            ItemInstInfo itemInstData = item as ItemInstInfo;
            LOG.errorif(itemInstData == null);
            SpecModifier.RemoveModifier(itemInstData.GetSpecFieldValues());
            OnUnequippedItem?.Invoke(itemInstData);
        }

        public bool TryEquip(ItemInstInfo item)
        {
            return mEquipment.EquipAtEmptySlot(item);
        }
        public bool TryEquip(ItemInstInfo item, int index)
        {
            return mEquipment.Equip(item, index);
        }
        public bool Unequip(ItemInstInfo item)
        {
            return mEquipment.Unequip(item);
        }
        public bool Unequip(EquipmentSlotType slot, int index)
        {
            return mEquipment.Unequip(slot, index);
        }
        public IReadOnlyList<IEquipItem> GetEquipments(EquipmentSlotType slot)
        {
            return mEquipment.GetEquipments(slot);
        }
        public ItemInstInfo GetEquipment(EquipmentSlotType slot, int index)
        {
            return mEquipment.GetEquipment(slot, index) as ItemInstInfo;
        }
        public bool IsValidSlot(EquipmentSlotType slot, int index)
        {
            return mEquipment.IsValidSlot(slot, index);
        }
        public int GetSlotMaxCount(EquipmentSlotType slot)
        {
            return mEquipment.GetSlotMaxCount(slot);
        }
        public bool HasEmptySlot(EquipmentSlotType slot)
        {
            return mEquipment.HasEmptySlot(slot);
        }
        public void ExpandMaxSlotCount(EquipmentSlotType slotType, int expandCount)
        {
            mEquipment.ExpandMaxSlotCount(slotType, expandCount);
        }
    }
}