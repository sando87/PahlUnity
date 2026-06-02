using System;
using System.Collections.Generic;

namespace PahlUnity
{
    public enum EquipmentSlotType
    {
        None,
        Weapon,
        Helmet,
        Armor,
        Gloves,
        Boots,
        Ring,
        Necklace,
    }

    public interface IEquipItem
    {
        EquipmentSlotType SlotType { get; }
    }
}