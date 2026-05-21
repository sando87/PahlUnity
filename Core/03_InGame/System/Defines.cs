#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define IS_DEBUG_MODE
#endif

using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace PahlUnity
{
    public readonly partial struct AnimStateHash
    {
        public readonly int mVal;
        public AnimStateHash(string value) => mVal = Animator.StringToHash(value);
        public AnimStateHash(int value) => mVal = value;

        public static implicit operator int(AnimStateHash info) => info.mVal;
        public static implicit operator AnimStateHash(int val) => new AnimStateHash(val);
        public static implicit operator AnimStateHash(string val) => new AnimStateHash(val);

        public override bool Equals(object obj) => obj is AnimStateHash other && mVal == other.mVal;
        public override int GetHashCode() => mVal;

        public static bool operator ==(AnimStateHash a, AnimStateHash b) => a.mVal == b.mVal;
        public static bool operator !=(AnimStateHash a, AnimStateHash b) => a.mVal != b.mVal;

        // Example
        // public static readonly AnimStateNameHash Idle = new("Idle");
        // public static readonly AnimStateNameHash Jump = new("Jump");
        // public static readonly AnimStateNameHash Attack = new("Attack");
        // public static readonly AnimStateNameHash Death = new("Death");
    }

    [System.Flags]
    public enum InteractMask : uint
    {
        Nothing = 0,
        Unit = 1 << 0,
        Skill = 1 << 1,
        Terrain = 1 << 2,
        Projectile = 1 << 3,
        Props = 1 << 4,
        Item = 1 << 5,
        Everything = 0xffffffff
    }

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

    public struct SaveUserPlayData : IEventParam
    {
        public readonly bool ImmediateSave;
        public SaveUserPlayData(bool immediateSave) => ImmediateSave = immediateSave;
    }

}