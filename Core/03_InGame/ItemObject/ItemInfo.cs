using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlUnity
{
    [System.Serializable]
    public class ItemInfo
    {
        public SpecOption _Option = null;

        public override string ToString()
        {
            return SaveData == null ? "none" : SaveData.InstanceID;
        }

        public ItemSaveData SaveData { get; private set; } = null;
        public ItemResourceData ResourceData { get; private set; } = null;
        public SpecOption Option { get; private set; } = null;
        public Dictionary<string, string> DisplayInfo { get; private set; } = new Dictionary<string, string>();

        public string InstanceID => SaveData.InstanceID;
        public string Name => ResourceData.DisplayName;
        public long ResourceID => ResourceData.ID;
        public bool IsEquipped { get => SaveData.IsEquipped; set => SaveData.IsEquipped = value; }
        public int Count { get => SaveData.Count; set => SaveData.Count = value; }
        public int PositionIndex { get => SaveData.PositionIndex; set { SaveData.PositionIndex = value; } }
        public int Level { get => SaveData.Level; set { SaveData.Level = value; UpdateOption(); } }
        public bool IsRepaired { get => SaveData.IsRepaired; set => SaveData.IsRepaired = value; }
        public bool IsEquipable { get => SaveData.IsEquipable; set => SaveData.IsEquipable = value; }

        static public string GetRandomItemID(int levelLimit)
        {
            List<ItemResourceData> list = new List<ItemResourceData>();
            foreach (var kvp in TableDataContainer<ItemResourceData>.Instance.Enums())
            {
                if (kvp.LevelLimit > levelLimit)
                    continue;

                list.Add(kvp);
            }

            if (list.Count <= 0)
                return "";

            ItemResourceData resourceData = list.ExGetRandom();
            return resourceData.ItemID;
        }
        public void InitItem(string itemID)
        {
            ResourceData = TableDataContainer<ItemResourceData>.Instance.GetInfo(itemID);

            SaveData = new ItemSaveData();
            SaveData.InstanceID = System.Guid.NewGuid().ToString();
            SaveData.ResourceID = ResourceData.ID;
            SaveData.IsEquipped = false;
            SaveData.Level = 1;
            SaveData.Count = 1;
            SaveData.PositionIndex = -1;
            SaveData.IsRepaired = false;

            UpdateOption();
        }
        public void LoadItem(ItemSaveData data)
        {
            SaveData = data;
            ResourceData = TableDataContainer<ItemResourceData>.Instance.GetInfo(SaveData.ResourceID);
            UpdateOption();
        }
        public void UpdateOption()
        {
            Option = new SpecOption();
            int point = SaveData.LevelIndex;
            System.Random ran = new System.Random(SaveData.RandomSeed);

            Option.HealthUp = (PercentUp)ResourceData._HealthUp.GetIntInRange(GetNextFloatNormalized(ran));
            Option.HealthRegen = (float)ResourceData._HealthRegen.GetFloatInRange(GetNextFloatNormalized(ran));
            Option.ManaUp = (PercentUp)ResourceData._ManaUp.GetIntInRange(GetNextFloatNormalized(ran));
            Option.ManaRegen = (float)ResourceData._ManaRegen.GetFloatInRange(GetNextFloatNormalized(ran));
            Option.BaseAttackAdd = (float)ResourceData._BaseAttackAdd.GetFloatInRange(GetNextFloatNormalized(ran));
            Option.PhyAttack = (Percent)ResourceData._PhyAttack.GetIntInRange(GetNextFloatNormalized(ran));
            Option.FireAttack = (Percent)ResourceData._FireAttack.GetIntInRange(GetNextFloatNormalized(ran));
            Option.IceAttack = (Percent)ResourceData._IceAttack.GetIntInRange(GetNextFloatNormalized(ran));
            Option.LightningAttack = (Percent)ResourceData._LightningAttack.GetIntInRange(GetNextFloatNormalized(ran));
            Option.DefenceUp = (PercentUp)ResourceData._DefenceUp.GetIntInRange(GetNextFloatNormalized(ran));
            Option.MoveSpeedUp = (PercentUp)ResourceData._MoveSpeedUp.GetIntInRange(GetNextFloatNormalized(ran));
            Option.AttackSpeedUp = (PercentUp)ResourceData._AttackSpeedUp.GetIntInRange(GetNextFloatNormalized(ran));
            Option.CooltimeDown = (PercentUp)ResourceData._CooltimeDown.GetIntInRange(GetNextFloatNormalized(ran));
            Option.ShieldAdd = (float)ResourceData._ShieldAdd.GetFloatInRange(GetNextFloatNormalized(ran));
            Option.ShieldRegen = (float)ResourceData._ShieldRegen.GetFloatInRange(GetNextFloatNormalized(ran));
            Option.CriticalRate = (PercentUp)ResourceData._CriticalRate.GetIntInRange(GetNextFloatNormalized(ran));
            Option.CriticalAttack = (PercentUp)ResourceData._CriticalAttack.GetIntInRange(GetNextFloatNormalized(ran));
            Option.ProjectileCountUp = (float)ResourceData._ProjectileCountUp.GetFloatInRange(GetNextFloatNormalized(ran));
            Option.ProjectileSpeedUp = (PercentUp)ResourceData._ProjectileSpeedUp.GetIntInRange(GetNextFloatNormalized(ran));
            Option.AttackRangeUp = (PercentUp)ResourceData._AttackRangeUp.GetIntInRange(GetNextFloatNormalized(ran));
            Option.SplashRangeUp = (PercentUp)ResourceData._SplashRangeUp.GetIntInRange(GetNextFloatNormalized(ran));
            Option.DurationUp = (PercentUp)ResourceData._DurationUp.GetIntInRange(GetNextFloatNormalized(ran));
            Option.FireResist = (Percent)ResourceData._FireResist.GetIntInRange(GetNextFloatNormalized(ran));
            Option.IceResist = (Percent)ResourceData._IceResist.GetIntInRange(GetNextFloatNormalized(ran));
            Option.LightningResist = (Percent)ResourceData._LightningResist.GetIntInRange(GetNextFloatNormalized(ran));
            Option.PosionResist = (Percent)ResourceData._PosionResist.GetIntInRange(GetNextFloatNormalized(ran));

            UpdateDisplayInfo();
        }

        float GetNextFloatNormalized(System.Random random)
        {
            // Next(int) 는 0 ~ int.MaxValue-1
            int value = random.Next(int.MaxValue);      // 0 ~ 2,147,483,646
            return (float)value / (int.MaxValue - 1); // 0 ~ 1 포함
        }

        public void UpdateDisplayInfo()
        {
            DisplayInfo.Clear();
            if (Option == null)
                return;

            List<ReflectionFieldData> fields = new List<ReflectionFieldData>();
            ReflectionFieldExtractor.GetFields(Option, fields);
            foreach (var field in fields)
            {
                if (field.Value.Equals("0") || field.Value.Equals("0%"))
                    continue;

                DisplayInfo[field.FieldName] = field.Value;
            }
        }
    }
}
