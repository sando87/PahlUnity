using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace PahlUnity
{
    [System.Serializable]
    public class UserSaveData
    {
        public int Gold = 0;

        public Dictionary<int, CharacterSaveData> Characters = new Dictionary<int, CharacterSaveData>();
    }

    [System.Serializable]
    public class CharacterSaveData
    {
        public bool IsFirstPlay = true;
        public int LifePotionCount = 0;
        public int ManaPotionCount = 0;
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

        public int RandomSeed { get => InstanceID.GetHashCode(); }
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
}