using System;
using System.Collections.Generic;

namespace PahlUnity
{
    /// <summary>
    /// 장비 타입별로 장착 가능한 아이템 슬롯 개수를 초기에 한번 세팅 후 관리하는 기능
    /// 장비 타입별 다수의 아이템 슬롯 관리 기능을 제공
    /// </summary>
    public class Equipment
    {
        /// 새 아이템 장착 (slot, newItem)
        public event Action<IEquipItem> OnEquipped;

        /// 장비 해제 (slot, removedItem)
        public event Action<IEquipItem> OnUnequipped;

        private readonly Dictionary<EquipmentSlotType, List<IEquipItem>> mEquipments = new();

        public Equipment(IReadOnlyDictionary<EquipmentSlotType, int> slotMaxCounts)
        {
            if (slotMaxCounts == null)
                return;

            foreach (var kvp in slotMaxCounts)
            {
                int maxCount = Math.Max(0, kvp.Value);
                mEquipments[kvp.Key] = new List<IEquipItem>(maxCount);

                for (int i = 0; i < maxCount; i++)
                {
                    mEquipments[kvp.Key].Add(null);
                }
            }
        }

        public bool IsEquipped(IEquipItem item)
        {
            if (item == null)
                return false;

            return mEquipments.TryGetValue(item.SlotType, out List<IEquipItem> items) && items.Contains(item);
        }

        public IEquipItem GetEquipment(EquipmentSlotType slot, int index)
        {
            LOG.errorif(!IsValid(slot, index));
            return mEquipments[slot][index];
        }

        public IReadOnlyList<IEquipItem> GetEquipments(EquipmentSlotType slot)
        {
            LOG.errorif(!mEquipments.ContainsKey(slot));
            return mEquipments[slot];
        }

        public int GetEquippedCount(EquipmentSlotType slot)
        {
            if (!mEquipments.TryGetValue(slot, out List<IEquipItem> items))
                return 0;

            int count = 0;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null)
                {
                    count++;
                }
            }

            return count;
        }

        public int GetSlotMaxCount(EquipmentSlotType slot)
        {
            return mEquipments.TryGetValue(slot, out List<IEquipItem> items) ? items.Count : 0;
        }

        public bool HasEmptySlot(EquipmentSlotType slot)
        {
            LOG.errorif(!mEquipments.ContainsKey(slot));
            return GetFirstEmptySlotIndex(slot) >= 0;
        }

        public int GetFirstEmptySlotIndex(EquipmentSlotType slot)
        {
            if (!mEquipments.TryGetValue(slot, out List<IEquipItem> items))
                return -1;

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] == null)
                    return i;
            }

            return -1;
        }

        public bool EquipAtEmptySlot(IEquipItem item)
        {
            if (item == null)
                return false;

            int index = GetFirstEmptySlotIndex(item.SlotType);
            return index >= 0 && Equip(item, index);
        }

        public bool Equip(IEquipItem item, int index)
        {
            LOG.errorif(item == null || !IsValid(item.SlotType, index));

            EquipmentSlotType slot = item.SlotType;
            IEquipItem oldItem = mEquipments[slot][index];
            mEquipments[slot][index] = item;

            if (oldItem != null)
                OnUnequipped?.Invoke(oldItem);

            OnEquipped?.Invoke(item);
            return true;
        }

        public bool Unequip(EquipmentSlotType slot, int index)
        {
            LOG.errorif(!IsValid(slot, index));

            IEquipItem item = mEquipments[slot][index];
            if (item == null)
                return false;

            mEquipments[slot][index] = null;
            OnUnequipped?.Invoke(item);

            return true;
        }

        public bool Unequip(IEquipItem item)
        {
            int index = FindEquipItemIndex(item);
            if (index < 0)
                return false;

            return Unequip(item.SlotType, index);
        }

        public int FindEquipItemIndex(IEquipItem item)
        {
            LOG.errorif(item == null || !mEquipments.ContainsKey(item.SlotType));

            List<IEquipItem> items = mEquipments[item.SlotType];
            int index = items.IndexOf(item);
            if (index < 0)
                return -1;

            return index;
        }

        public void Clear()
        {
            foreach (var kvp in mEquipments)
            {
                EquipmentSlotType slot = kvp.Key;
                List<IEquipItem> items = kvp.Value;
                for (int i = items.Count - 1; i >= 0; i--)
                {
                    IEquipItem item = items[i];
                    if (item == null)
                        continue;

                    items[i] = null;
                    OnUnequipped?.Invoke(item);
                }
            }
        }

        private bool IsValid(EquipmentSlotType slot, int index)
        {
            if (!mEquipments.TryGetValue(slot, out List<IEquipItem> items))
                return false;

            return index >= 0 && index < items.Count;
        }

    }
}