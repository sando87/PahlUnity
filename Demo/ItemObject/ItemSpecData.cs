using System.Collections.Generic;
using UnityEngine;

namespace PahlUnity.Demo
{
    [CreateAssetMenu(fileName = "ItemSpecData", menuName = "Demo/ItemSpecData")]
    public class ItemSpecData : ScriptableObject, ITableRecord
    {
        [SerializeField] private string _ItemName = "";
        [SerializeField] private string _ItemDesc = "";
        [SerializeField] private bool _IsStackable = false;
        [SerializeField] private int _MaxStackCount = 1;
        [SerializeField] private EquipmentSlotType _EquipSlot = 0;
        [SerializeField] private Sprite _ItemIcon = null;
        [SerializeField] private List<SpecFieldRaw> _Specs = new List<SpecFieldRaw>();

        public string ItemID => name; // asset name as item id
        public string ItemName => _ItemName;
        public string ItemDesc => _ItemDesc;
        public Sprite ItemIcon => _ItemIcon;
        public bool IsStackable => _IsStackable;
        public int MaxStackCount => _MaxStackCount;
        public EquipmentSlotType EquipSlot => _EquipSlot;

        public IReadOnlyList<SpecFieldRaw> Specs => _Specs;

        public long ID => ItemID.ExGetStableHash64();
        public int RowIndex { get; set; }
    }
}