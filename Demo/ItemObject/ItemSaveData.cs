using Newtonsoft.Json;
using UnityEngine;

namespace PahlUnity.Demo
{
    [System.Serializable]
    public class ItemSaveData
    {
        public long InstanceID;
        public long ResourceID;
        public bool IsEquipped;
        public int Level;
        public int Count;
        public int PositionIndex;

        public ItemSaveData(long instanceID, long resourceID)
        {
            InstanceID = instanceID;
            ResourceID = resourceID;
            IsEquipped = false;
            Level = 1;
            Count = 1;
            PositionIndex = -1;
        }

        [JsonIgnore]
        public int LevelIndex { get => Level - 1; }
    }
}