using System;
using System.Collections.Generic;

namespace PahlUnity
{
    public class Equipment
    {
        /// 새 아이템 장착 (slot, newItem, oldItem)
        public event Action<EquipmentSlotType, IEquipItem, IEquipItem> OnEquipped;

        /// 장비 해제 (slot, removedItem)
        public event Action<EquipmentSlotType, IEquipItem> OnUnequipped;

        private readonly Dictionary<EquipmentSlotType, IEquipItem> mEquipments = new();

        public IReadOnlyDictionary<EquipmentSlotType, IEquipItem> Equipments => mEquipments;

        public bool IsEquipped(EquipmentSlotType slot)
        {
            return mEquipments.ContainsKey(slot);
        }

        public IEquipItem GetEquipment(EquipmentSlotType slot)
        {
            mEquipments.TryGetValue(slot, out var item);
            return item;
        }

        public bool Equip(IEquipItem item)
        {
            if (item == null)
                return false;

            EquipmentSlotType slot = item.SlotType;

            mEquipments.TryGetValue(slot, out IEquipItem oldItem);

            mEquipments[slot] = item;

            OnEquipped?.Invoke(slot, item, oldItem);

            return true;
        }

        public bool Unequip(EquipmentSlotType slot)
        {
            if (!mEquipments.TryGetValue(slot, out IEquipItem item))
                return false;

            mEquipments.Remove(slot);

            OnUnequipped?.Invoke(slot, item);

            return true;
        }

        public bool Unequip(IEquipItem item)
        {
            if (item == null)
                return false;

            return Unequip(item.SlotType);
        }

        public void Clear()
        {
            var slots = new List<EquipmentSlotType>(mEquipments.Keys);

            foreach (var slot in slots)
            {
                Unequip(slot);
            }
        }

    }
}