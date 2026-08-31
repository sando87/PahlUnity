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
        private int mLevel;

        private IReadOnlyList<SpecFieldValue> mSpecFieldValues = null;

        public ItemInstInfo(ItemSpecData specData, int level = 1)
        {
            mSpecRawData = specData;
            mResourceID = mSpecRawData.ItemID.ExGetStableHash32();
            mInstanceID = DateTime.Now.Ticks;
            mLevel = level;
        }
        public ItemInstInfo(ItemSpecData specData, long instanceID, int level = 1)
        {
            mSpecRawData = specData;
            mResourceID = mSpecRawData.ItemID.ExGetStableHash32();
            mInstanceID = instanceID;
            mLevel = level;
        }

        public int ResourceID => mResourceID;
        public long InstanceID => mInstanceID;
        public int Level => mLevel;
        public bool IsStackable => mSpecRawData.IsStackable;
        public int MaxStackCount => mSpecRawData.MaxStackCount;
        public int RandomSeed => (int)mInstanceID;
        public ItemSpecData SpecData => mSpecRawData;

        public EquipmentSlotType SlotType => mSpecRawData.EquipSlot;

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
