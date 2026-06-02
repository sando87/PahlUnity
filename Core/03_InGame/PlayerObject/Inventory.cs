using System;
using System.Collections.Generic;

namespace PahlUnity
{
    public class Inventory
    {
        private readonly List<InventorySlot> mSlots;

        public IReadOnlyList<InventorySlot> Slots => mSlots;

        public event Action<IInvenItem, int> OnItemAdded;
        public event Action<IInvenItem, int> OnItemRemoved;
        public event Action<IInvenItem, int> OnItemMoved;

        public Inventory(int slotCount)
        {
            mSlots = new List<InventorySlot>(slotCount);

            for (int i = 0; i < slotCount; i++)
            {
                mSlots.Add(new InventorySlot());
            }
        }

        public void InitItems(IReadOnlyList<IInvenItem> itemDataList)
        {
            for (int i = 0; i < mSlots.Count && i < itemDataList.Count; i++)
            {
                IInvenItem itemData = itemDataList[i];
                if (itemData != null)
                {
                    mSlots[i].Item = itemData;
                    mSlots[i].Count = 1;
                }
            }
        }

        public int AddItem(IInvenItem item, int count = 1)
        {
            LOG.errorif(item == null || count <= 0);

            if (item.IsStackable)
            {
                InventorySlot sameItemSlot = FindSameItem(item.ResourceID);
                if (sameItemSlot != null)
                {
                    int addCount = sameItemSlot.AddCount(count);
                    OnItemAdded?.Invoke(item, addCount);
                    return addCount;
                }
                else
                {
                    InventorySlot emptySlot = FindEmptySlot();
                    if (emptySlot == null)
                        return 0;

                    int addCount = Math.Min(count, item.MaxStackCount);
                    emptySlot.Item = item;
                    emptySlot.Count = addCount;
                    OnItemAdded?.Invoke(item, addCount);
                    return addCount;
                }
            }
            else
            {
                InventorySlot emptySlot = FindEmptySlot();
                if (emptySlot == null)
                    return 0;

                emptySlot.Item = item;
                emptySlot.Count = 1;
                OnItemAdded?.Invoke(item, 1);
                return 1;
            }
        }

        public int RemoveItem(int slotIndex, int count = 1)
        {
            LOG.errorif(slotIndex < 0 || slotIndex >= mSlots.Count || count <= 0);

            InventorySlot slot = mSlots[slotIndex];
            if (slot.IsEmpty)
                return 0;

            int removeCount = Math.Min(slot.Count, count);
            slot.Count -= removeCount;

            if (slot.Count <= 0)
            {
                slot.Clear();
            }

            OnItemRemoved?.Invoke(slot.Item, removeCount);
            return removeCount;
        }

        public bool MoveItem(int fromIndex, int toIndex)
        {
            LOG.errorif(fromIndex < 0 || fromIndex >= mSlots.Count || toIndex < 0 || toIndex >= mSlots.Count);
            if (fromIndex == toIndex)
                return true;

            InventorySlot fromSlot = mSlots[fromIndex];
            InventorySlot toSlot = mSlots[toIndex];
            if (fromSlot.IsEmpty)
                return false;

            IInvenItem targetItem = fromSlot.Item;
            int targetCount = fromSlot.Count;
            if (toSlot.IsEmpty)
            {
                toSlot.Item = targetItem;
                toSlot.Count = targetCount;
                fromSlot.Clear();
                OnItemMoved?.Invoke(targetItem, toIndex);
            }
            else
            {
                fromSlot.Item = toSlot.Item;
                fromSlot.Count = toSlot.Count;
                toSlot.Item = targetItem;
                toSlot.Count = targetCount;
                OnItemMoved?.Invoke(targetItem, toIndex);
                OnItemMoved?.Invoke(fromSlot.Item, fromIndex);
            }

            return true;
        }

        private InventorySlot FindSameItem(int resourceId)
        {
            foreach (InventorySlot slot in mSlots)
            {
                if (!slot.IsEmpty && slot.Item.ResourceID == resourceId)
                {
                    return slot;
                }
            }

            return null;
        }

        private InventorySlot FindEmptySlot()
        {
            foreach (InventorySlot slot in mSlots)
            {
                if (slot.IsEmpty)
                {
                    return slot;
                }
            }

            return null;
        }
    }
}