using System;
using System.Collections.Generic;
using UnityEngine;

namespace PahlUnity.Demo
{
    public class ItemInstInfo : IInvenItem, IEquipItem
    {
        private ItemSpecData mSpecRawData;
        private int mResourceID;
        private long mInstanceID;
        private ItemSaveData mSaveData;

        private IReadOnlyList<SpecFieldValue> mSpecFieldValues = null;

        public ItemInstInfo(ItemSpecData specData)
        {
            mSpecRawData = specData;
            mResourceID = mSpecRawData.ItemID.ExGetStableHash32();
            mInstanceID = DateTime.Now.Ticks;
            mSaveData = new ItemSaveData();
        }
        public ItemInstInfo(ItemSpecData specData, ItemSaveData saveData)
        {
            mSpecRawData = specData;
            mResourceID = mSpecRawData.ItemID.ExGetStableHash32();
            mInstanceID = saveData.InstanceID;
            mSaveData = saveData;
        }

        public int ResourceID => mResourceID;
        public long InstanceID => mInstanceID;
        public int Level => mSaveData == null ? 1 : mSaveData.Level;
        public bool IsStackable => mSpecRawData.IsStackable;
        public int MaxStackCount => mSpecRawData.MaxStackCount;
        public int RandomSeed => (int)mInstanceID;
        public ItemSpecData SpecData => mSpecRawData;
        public ItemSaveData SaveData => mSaveData;

        public EquipmentSlotType SlotType => (int)mSpecRawData.EquipSlot;

        public IReadOnlyList<SpecFieldValue> GetSpecFieldValues()
        {
            if (mSpecFieldValues != null)
                return mSpecFieldValues;

            List<SpecFieldValue> specs = new();
            System.Random random = new(RandomSeed);
            foreach (var spec in mSpecRawData.Specs)
            {
                specs.Add(new SpecFieldValue(spec, random));
            }

            mSpecFieldValues = specs;
            return mSpecFieldValues;
        }
    }
}
