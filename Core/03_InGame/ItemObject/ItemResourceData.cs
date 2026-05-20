using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace PahlUnity
{
    [System.Serializable]
    public class ItemResourceData : ITableRecord
    {
        public readonly string ItemID;
        public readonly string DisplayName;
        public readonly string Desc;
        public readonly int LevelLimit;
        public readonly string OptionID;

        public int RowIndex { get; set; } // 데이터데이블상에 존재하는 순서
        public long ID { get { return ITableRecord.ToID(ItemID); } } // 데이터 접근을 위한 id값

        public ItemAssetData AssetData { get; private set; }

        void ITableRecord.OnLoad()
        {
            AssetData = Resources.Load<ItemAssetData>("ScriptableObjects/" + ItemID);
        }
    }

}