using System;
using UnityEngine;

namespace PahlUnity.Demo
{
    public class ItemInstInfo : IInvenItem
    {
        private ItemSpecData mSpecData;
        private int mResourceID;
        private long mInstanceID;

        public ItemInstInfo(ItemSpecData specData)
        {
            mSpecData = specData;
            mResourceID = mSpecData.ItemID.ExGetStableHash32();
            mInstanceID = DateTime.Now.Ticks;
        }

        public int ResourceID => mResourceID;
        public long InstanceID => mInstanceID;
        public bool IsStackable => mSpecData.IsStackable;
        public int MaxStackCount => mSpecData.MaxStackCount;
        public int RandomSeed => (int)mInstanceID;
    }
}
