using Newtonsoft.Json;
using UnityEngine;

namespace PahlUnity.Demo
{
    [System.Serializable]
    public class SkillSaveData
    {
        public long InstanceID;
        public long ResourceID;
        public bool IsEquipped;
        public bool IsLearned;
        public int Level;
        public int SubStep;
        public int PositionIndex;

        public SkillSaveData(long instanceID, long resourceID)
        {
            InstanceID = instanceID;
            ResourceID = resourceID;
            IsEquipped = false;
            IsLearned = false;
            Level = 1;
            SubStep = 0;
            PositionIndex = -1;
        }

        [JsonIgnore]
        public int LevelIndex { get => Level - 1; }
    }
}