using System;
using System.Collections.Generic;
using UnityEngine;

namespace PahlUnity.Demo
{
    public class ItemInstInfo : IInvenItem, IEquipItem
    {
        private ItemSpecData mSpecRawData;
        private long mResourceID;
        private long mInstanceID;
        private ItemSaveData mSaveData;

        private IReadOnlyList<SpecFieldValue> mSpecFieldValues = null;

        public ItemInstInfo(ItemSpecData specData)
        {
            mSpecRawData = specData;
            mResourceID = mSpecRawData.ID;
            mInstanceID = DateTime.Now.Ticks;
            mSaveData = new ItemSaveData(mInstanceID, mResourceID);
        }
        public ItemInstInfo(ItemSpecData specData, ItemSaveData saveData)
        {
            mSpecRawData = specData;
            mResourceID = mSpecRawData.ID;
            mInstanceID = saveData.InstanceID;
            mSaveData = saveData;
        }

        public long ResourceID => mResourceID;
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
