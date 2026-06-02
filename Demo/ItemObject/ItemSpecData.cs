using System.Collections.Generic;
using UnityEngine;

namespace PahlUnity.Demo
{
    [CreateAssetMenu(fileName = "ItemSpecData", menuName = "Demo/ItemSpecData")]
    public class ItemSpecData : ScriptableObject, ITableRecord
    {
        [SerializeField] private string _ItemID = "";
        [SerializeField] private string _ItemName = "";
        [SerializeField] private string _ItemDesc = "";
        [SerializeField] private bool _IsStackable = false;
        [SerializeField] private int _MaxStackCount = 1;
        [SerializeField] private EquipmentSlotType _EquipSlot = EquipmentSlotType.None;
        [SerializeField] private Sprite _ItemIcon = null;
        [SerializeField] private List<SpecValueInfo> _Specs = new List<SpecValueInfo>();

        public string ItemID => _ItemID;
        public string ItemName => _ItemName;
        public string ItemDesc => _ItemDesc;
        public Sprite ItemIcon => _ItemIcon;
        public bool IsStackable => _IsStackable;
        public int MaxStackCount => _MaxStackCount;
        public EquipmentSlotType EquipSlot => _EquipSlot;

        public IReadOnlyList<SpecValueInfo> Specs => _Specs;

        public long ID => _ItemID.ExGetStableHash64();
        public int RowIndex { get; set; }
    }
}