using System;

namespace PahlUnity
{
    public readonly struct EquipmentSlotType : IEquatable<EquipmentSlotType>
    {
        public readonly int Value;

        public EquipmentSlotType(int value)
        {
            Value = value;
        }

        public bool Equals(EquipmentSlotType other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is EquipmentSlotType other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value;
        }

        public static bool operator ==(EquipmentSlotType left, EquipmentSlotType right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EquipmentSlotType left, EquipmentSlotType right)
        {
            return !left.Equals(right);
        }

        public static implicit operator int(EquipmentSlotType id) => id.Value;
        public static implicit operator EquipmentSlotType(int value) => new(value);
    }

    public interface IEquipItem
    {
        EquipmentSlotType SlotType { get; }
    }
}