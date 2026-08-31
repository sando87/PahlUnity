using System;
using UnityEngine;

namespace PahlUnity
{
    public class InventorySlot
    {
        public IInvenItem Item;
        public int Count;

        public bool IsEmpty => Item == null;

        public void Clear()
        {
            Item = null;
            Count = 0;
        }
        public int AddCount(int count)
        {
            if (IsEmpty)
                return count;

            int remainSpace = Item.MaxStackCount - Count;
            int addCount = Math.Min(remainSpace, count);
            Count += addCount;
            return addCount;
        }
    }
}