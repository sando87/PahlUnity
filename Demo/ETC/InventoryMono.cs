using System;
using System.Collections.Generic;
using NaughtyAttributes;
using PahlUnity;
using UnityEngine;

namespace PahlUnity.Demo
{
    public class InventoryMono : MonoBehaviour
    {
        [SerializeField] private int _SlotCount = 20;

        private Inventory mInventory = null;

        public IReadOnlyList<InventorySlot> Slots => mInventory.Slots;

        public event Action<ItemInstInfo, int, int> OnItemAdded;
        public event Action<ItemInstInfo, int> OnItemRemoved;
        public event Action<ItemInstInfo, int> OnItemMoved;

        void Awake()
        {
            mInventory = new Inventory(_SlotCount);

            mInventory.OnItemAdded += (item, count, index) => OnItemAdded?.Invoke((ItemInstInfo)item, count, index);
            mInventory.OnItemRemoved += (item, index) => OnItemRemoved?.Invoke((ItemInstInfo)item, index);
            mInventory.OnItemMoved += (item, index) => OnItemMoved?.Invoke((ItemInstInfo)item, index);
        }

        public ItemInstInfo GetItem(int slotIndex)
        {
            return mInventory.GetItem(slotIndex) as ItemInstInfo;
        }

        public int AddItem(ItemInstInfo item, int count = 1)
        {
            return mInventory.AddItem(item, count);
        }

        public bool HasEmptySlot()
        {
            return mInventory.HasEmptySlot();
        }

        public int RemoveItem(int slotIndex, int count = 1)
        {
            return mInventory.RemoveItem(slotIndex, count);
        }

        public int RemoveItem(ItemInstInfo item, int count = 1)
        {
            return mInventory.RemoveItem(item, count);
        }

        public void ClearAllItem()
        {
            mInventory.ClearAllItem();
        }

        public bool MoveItem(int fromIndex, int toIndex)
        {
            return mInventory.MoveItem(fromIndex, toIndex);
        }

        public int CountItemSlot()
        {
            return mInventory.CountItemSlot();
        }




        [Header("===== Editor Area =====")]
        [SerializeField, ShowIf(nameof(IsPlayMode)), NaughtyAttributes.ReadOnly]
        List<ItemViewerOnInspector> _ItemList = new List<ItemViewerOnInspector>();
        bool IsPlayMode => Application.isPlaying;
        [Button("UpdateItemList"), ShowIf(nameof(IsPlayMode))]
        public void UpdateItemList()
        {
            _ItemList.Clear();
            if (mInventory == null || mInventory.Slots == null || mInventory.Slots.Count == 0)
                return;

            foreach (InventorySlot slot in mInventory.Slots)
            {
                if (slot.IsEmpty)
                    continue;

                ItemViewerOnInspector itemViewer = new ItemViewerOnInspector();
                itemViewer.Name = slot.Item.Name;
                itemViewer.Index = slot.PositionIndex;
                itemViewer.Count = slot.Count;
                itemViewer.BaseObj = this.ExGetBase();
                _ItemList.Add(itemViewer);
            }
        }
    }

    [Serializable]
    public class ItemViewerOnInspector
    {
        public string Name;
        public int Index;
        public int Count;
        public EquipSlotType SlotType;
        public bool IsEquipped;
        public BaseObject BaseObj;
    }
}