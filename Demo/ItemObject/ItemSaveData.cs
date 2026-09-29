using UnityEngine;

namespace PahlUnity.Demo
{
    [System.Serializable]
    public class ItemSaveData
    {
        public long InstanceID = 0;
        public long ResourceID = 0;
        public bool IsEquipped = false;
        public int Level = 1;
        public int Count = 1;
        public int PositionIndex = -1;

        public int LevelIndex { get => Level - 1; }
    }
}