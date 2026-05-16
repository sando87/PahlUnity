using System.Collections.Generic;
using DG.Tweening;
using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

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
        foreach (var kvp in ItemResourceTable.Instance.Enums())
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
        ResourceData = ItemResourceTable.Instance.GetInfo(itemID);

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
        ResourceData = ItemResourceTable.Instance.GetInfo(SaveData.ResourceID);
        UpdateOption();
    }
    public void UpdateOption()
    {
        Option = new SpecOption();
        int point = SaveData.LevelIndex;
        System.Random ran = new System.Random(SaveData.RandomSeed);

        Option.HealthUp = (PercentUp)ResourceData._HealthUp.GetIntInRange(ran.ExNextFloatNormalized());
        Option.HealthRegen = (float)ResourceData._HealthRegen.GetFloatInRange(ran.ExNextFloatNormalized());
        Option.ManaUp = (PercentUp)ResourceData._ManaUp.GetIntInRange(ran.ExNextFloatNormalized());
        Option.ManaRegen = (float)ResourceData._ManaRegen.GetFloatInRange(ran.ExNextFloatNormalized());
        Option.BaseAttackAdd = (float)ResourceData._BaseAttackAdd.GetFloatInRange(ran.ExNextFloatNormalized());
        Option.PhyAttack = (Percent)ResourceData._PhyAttack.GetIntInRange(ran.ExNextFloatNormalized());
        Option.FireAttack = (Percent)ResourceData._FireAttack.GetIntInRange(ran.ExNextFloatNormalized());
        Option.IceAttack = (Percent)ResourceData._IceAttack.GetIntInRange(ran.ExNextFloatNormalized());
        Option.LightningAttack = (Percent)ResourceData._LightningAttack.GetIntInRange(ran.ExNextFloatNormalized());
        Option.DefenceUp = (PercentUp)ResourceData._DefenceUp.GetIntInRange(ran.ExNextFloatNormalized());
        Option.MoveSpeedUp = (PercentUp)ResourceData._MoveSpeedUp.GetIntInRange(ran.ExNextFloatNormalized());
        Option.AttackSpeedUp = (PercentUp)ResourceData._AttackSpeedUp.GetIntInRange(ran.ExNextFloatNormalized());
        Option.CooltimeDown = (PercentUp)ResourceData._CooltimeDown.GetIntInRange(ran.ExNextFloatNormalized());
        Option.ShieldAdd = (float)ResourceData._ShieldAdd.GetFloatInRange(ran.ExNextFloatNormalized());
        Option.ShieldRegen = (float)ResourceData._ShieldRegen.GetFloatInRange(ran.ExNextFloatNormalized());
        Option.CriticalRate = (PercentUp)ResourceData._CriticalRate.GetIntInRange(ran.ExNextFloatNormalized());
        Option.CriticalAttack = (PercentUp)ResourceData._CriticalAttack.GetIntInRange(ran.ExNextFloatNormalized());
        Option.ProjectileCountUp = (float)ResourceData._ProjectileCountUp.GetFloatInRange(ran.ExNextFloatNormalized());
        Option.ProjectileSpeedUp = (PercentUp)ResourceData._ProjectileSpeedUp.GetIntInRange(ran.ExNextFloatNormalized());
        Option.AttackRangeUp = (PercentUp)ResourceData._AttackRangeUp.GetIntInRange(ran.ExNextFloatNormalized());
        Option.SplashRangeUp = (PercentUp)ResourceData._SplashRangeUp.GetIntInRange(ran.ExNextFloatNormalized());
        Option.DurationUp = (PercentUp)ResourceData._DurationUp.GetIntInRange(ran.ExNextFloatNormalized());
        Option.FireResist = (Percent)ResourceData._FireResist.GetIntInRange(ran.ExNextFloatNormalized());
        Option.IceResist = (Percent)ResourceData._IceResist.GetIntInRange(ran.ExNextFloatNormalized());
        Option.LightningResist = (Percent)ResourceData._LightningResist.GetIntInRange(ran.ExNextFloatNormalized());
        Option.PosionResist = (Percent)ResourceData._PosionResist.GetIntInRange(ran.ExNextFloatNormalized());

        UpdateDisplayInfo();
    }

    public void UpdateDisplayInfo()
    {
        DisplayInfo.Clear();
        if (Option == null)
            return;

        List<FieldData> fields = new List<FieldData>();
        ReflectionFieldExtractor.GetFields(Option, fields);
        foreach (var field in fields)
        {
            if (field.Value.Equals("0") || field.Value.Equals("0%"))
                continue;

            DisplayInfo[field.Name] = field.Value;
        }
    }
}
