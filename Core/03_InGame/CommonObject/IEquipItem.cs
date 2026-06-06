using System;
using System.Collections.Generic;

namespace PahlUnity
{
    public readonly struct EquipmentSlotType
    {
        public readonly int Value;

        public EquipmentSlotType(int value)
        {
            Value = value;
        }

        public static implicit operator int(EquipmentSlotType id) => id.Value;
        public static implicit operator EquipmentSlotType(int value) => new(value);
    }

    public interface IEquipItem
    {
        EquipmentSlotType SlotType { get; }
    }
}