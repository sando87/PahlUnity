using System;
using System.Collections.Generic;
using NaughtyAttributes;
using PahlUnity;
using UnityEngine;

namespace PahlUnity.Demo
{
    [System.Serializable]
    public class EquipSlotConfig
    {
        public EquipSlotType SlotType;
        public int SlotCount;
    }

    [RequireComponent(typeof(SpecModifier))]
    public class EquipmentMono : MonoBehaviour
    {
        [SerializeField] EquipSlotConfig[] _EquipSlotConfig;

        Equipment mEquipment = null;
        SpecModifier mSpecModifier = null;

        public event Action<ItemInstInfo, int> OnEquippedItem;
        public event Action<ItemInstInfo> OnUnequippedItem;

        public SpecModifier SpecModifier => mSpecModifier;

        void Awake()
        {
            mSpecModifier = GetComponent<SpecModifier>();

            Dictionary<EquipmentSlotType, int> slotMaxCounts = new();
            foreach (var config in _EquipSlotConfig)
            {
                slotMaxCounts.Add((int)config.SlotType, config.SlotCount);
            }
            Init(slotMaxCounts);
        }
        public void Init(Dictionary<EquipmentSlotType, int> slotMaxCounts)
        {
            mEquipment = new Equipment(slotMaxCounts);
            mEquipment.OnEquipped += OnEquipped;
            mEquipment.OnUnequipped += OnUnequipped;
        }
        void OnEquipped(IEquipItem item, int index)
        {
            ItemInstInfo itemInstData = item as ItemInstInfo;
            LOG.errorif(itemInstData == null);
            SpecModifier.AddModifier(itemInstData.GetSpecFieldValues());
            OnEquippedItem?.Invoke(itemInstData, index);
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




        [Header("===== Editor Area =====")]
        bool IsPlayMode => Application.isPlaying;
        [SerializeField, ShowIf(nameof(IsPlayMode)), NaughtyAttributes.ReadOnly]
        List<string> _EquipItemList = new List<string>();
        [Button("UpdateEquipItemList"), ShowIf(nameof(IsPlayMode))]
        void UpdateEquipItemList()
        {
            _EquipItemList.Clear();
            foreach (var slotType in Enum.GetValues(typeof(EquipSlotType)))
            {
                if ((EquipSlotType)slotType == EquipSlotType.None)
                    continue;

                IReadOnlyList<IEquipItem> items = mEquipment.GetEquipments((int)slotType);
                if (items == null)
                    continue;

                for (int index = 0; index < items.Count; index++)
                {
                    ItemInstInfo itemInstData = items[index] as ItemInstInfo;
                    if (itemInstData != null)
                        _EquipItemList.Add($"{slotType}[{index}] : {itemInstData.SpecData.ItemID}");
                }
            }
        }
    }
}