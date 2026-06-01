#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define IS_DEBUG_MODE
#endif

using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace PahlUnity
{
    [System.Serializable]
    public class CharacterSaveData
    {
        public bool IsFirstPlay = true;
        public int LifePotionCount = 0;
        public int ManaPotionCount = 0;
        public int Gold = 0;
        public CharSaveData Stats = new CharSaveData();
        public Dictionary<string, ItemSaveData> Items = new Dictionary<string, ItemSaveData>();
        public Dictionary<string, SkillSaveData> Skills = new Dictionary<string, SkillSaveData>();
    }

    [System.Serializable]
    public class CharSaveData
    {
        public float CurrentExp;
        public int RemainPoint;
        public int RemainSkillPoint;
        public int HealthPoint;
        public int ManaPoint;
        public int AttackPoint;
        public int DefensePoint;
    }

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

        public SkillSaveData(string _resourceID)
        {
            this.ResourceID = _resourceID;
            this.Level = 1;
            this.SubStep = 0;
        }
    }

    [System.Serializable]
    public class InGameSaveData : SaveDataBase
    {
        public Dictionary<string, CharacterSaveData> Characters = new Dictionary<string, CharacterSaveData>();
    }
}