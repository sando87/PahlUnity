using UnityEngine;

namespace PahlUnity.Demo
{
    [System.Serializable]
    public class ItemSaveData
    {
        public string InstanceID;
        public long ResourceID;
        public bool IsEquipped;
        public int Level;
        public int Count;
        public int PositionIndex;
        public bool IsRepaired;
        public bool IsEquipable;

        public int RandomSeed { get => StableHash.ToInt32(InstanceID); }
        public int LevelIndex { get => Level - 1; }
    }
}