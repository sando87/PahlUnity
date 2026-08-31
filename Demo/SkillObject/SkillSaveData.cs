using Newtonsoft.Json;
using UnityEngine;

namespace PahlUnity.Demo
{
    [System.Serializable]
    public class SkillSaveData
    {
        public string ResourceID;
        public bool IsEquipped;
        public bool IsLearned;
        public int Level;
        public int SubStep;
        public int PositionIndex;

        [JsonIgnore]
        public int LevelIndex { get => Level - 1; }
    }
}