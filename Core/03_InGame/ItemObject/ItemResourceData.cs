using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace PahlUnity
{
    [System.Serializable]
    public class ItemResourceData : ITableRecord
    {
        public string ItemID;
        public string DisplayName;
        public string Desc;
        public int LevelLimit;
        public string OptionID;
        public ItemBase Prefab;
        public Sprite Icon;

        public int RowIndex { get; set; } // 데이터데이블상에 존재하는 순서
        public long ID { get { return ITableRecord.ToID(ItemID); } } // 데이터 접근을 위한 id값
    }

}