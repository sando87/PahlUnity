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
            SpecOptionData optionRawData = TableDataContainer<SpecOptionData>.Instance.GetInfo(ResourceData.OptionID);

            Option = new SpecOption();
            System.Random ran = new System.Random(SaveData.RandomSeed);

            Option.HealthUp = (PercentUp)optionRawData._HealthUp.GetIntInRange(GetNextFloatNormalized(ran));
            Option.HealthRegen = (float)optionRawData._HealthRegen.GetFloatInRange(GetNextFloatNormalized(ran));
            Option.ManaUp = (PercentUp)optionRawData._ManaUp.GetIntInRange(GetNextFloatNormalized(ran));
            Option.ManaRegen = (float)optionRawData._ManaRegen.GetFloatInRange(GetNextFloatNormalized(ran));
            Option.BaseAttackAdd = (float)optionRawData._BaseAttackAdd.GetFloatInRange(GetNextFloatNormalized(ran));
            Option.PhyAttack = (Percent)optionRawData._PhyAttack.GetIntInRange(GetNextFloatNormalized(ran));
            Option.FireAttack = (Percent)optionRawData._FireAttack.GetIntInRange(GetNextFloatNormalized(ran));
            Option.IceAttack = (Percent)optionRawData._IceAttack.GetIntInRange(GetNextFloatNormalized(ran));
            Option.LightningAttack = (Percent)optionRawData._LightningAttack.GetIntInRange(GetNextFloatNormalized(ran));
            Option.DefenceUp = (PercentUp)optionRawData._DefenceUp.GetIntInRange(GetNextFloatNormalized(ran));
            Option.MoveSpeedUp = (PercentUp)optionRawData._MoveSpeedUp.GetIntInRange(GetNextFloatNormalized(ran));
            Option.AttackSpeedUp = (PercentUp)optionRawData._AttackSpeedUp.GetIntInRange(GetNextFloatNormalized(ran));
            Option.CooltimeDown = (PercentUp)optionRawData._CooltimeDown.GetIntInRange(GetNextFloatNormalized(ran));
            Option.ShieldAdd = (float)optionRawData._ShieldAdd.GetFloatInRange(GetNextFloatNormalized(ran));
            Option.ShieldRegen = (float)optionRawData._ShieldRegen.GetFloatInRange(GetNextFloatNormalized(ran));
            Option.CriticalRate = (PercentUp)optionRawData._CriticalRate.GetIntInRange(GetNextFloatNormalized(ran));
            Option.CriticalAttack = (PercentUp)optionRawData._CriticalAttack.GetIntInRange(GetNextFloatNormalized(ran));
            Option.ProjectileCountUp = (float)optionRawData._ProjectileCountUp.GetFloatInRange(GetNextFloatNormalized(ran));
            Option.ProjectileSpeedUp = (PercentUp)optionRawData._ProjectileSpeedUp.GetIntInRange(GetNextFloatNormalized(ran));
            Option.AttackRangeUp = (PercentUp)optionRawData._AttackRangeUp.GetIntInRange(GetNextFloatNormalized(ran));
            Option.SplashRangeUp = (PercentUp)optionRawData._SplashRangeUp.GetIntInRange(GetNextFloatNormalized(ran));
            Option.DurationUp = (PercentUp)optionRawData._DurationUp.GetIntInRange(GetNextFloatNormalized(ran));
            Option.FireResist = (Percent)optionRawData._FireResist.GetIntInRange(GetNextFloatNormalized(ran));
            Option.IceResist = (Percent)optionRawData._IceResist.GetIntInRange(GetNextFloatNormalized(ran));
            Option.LightningResist = (Percent)optionRawData._LightningResist.GetIntInRange(GetNextFloatNormalized(ran));
            Option.PosionResist = (Percent)optionRawData._PosionResist.GetIntInRange(GetNextFloatNormalized(ran));

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
