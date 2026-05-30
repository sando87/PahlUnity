using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlUnity
{
    public class SpecPlayer : SpecBase
    {
        [SerializeField] float _MoveSpeed = 5f;
        [SerializeField] float _AttackSpeed = 1f;

        public override float MaxHealth => BaseStats.Health * Option.HealthUp;
        public override float MaxMana => BaseStats.Mana * Option.ManaUp;
        public override float MaxShield => BaseStats.Shield + Option.ShieldAdd;
        public override float BaseAttack => BaseStats.Attack + Option.BaseAttackAdd;
        public override float PhyDefence => BaseStats.Defence * Option.DefenceUp;
        public override float MoveSpeed => BaseStats.MoveSpeed * Option.MoveSpeedUp;
        public override float AttackSpeed => BaseStats.AttackSpeed * Option.AttackSpeedUp;
        public float HealthRegen => Option.HealthRegen;
        public float ManaRegen => Option.ManaRegen;
        public float ShieldRegen => Option.ShieldRegen;

        public CharSaveData SaveData { get; private set; } = null;
        public CharResourceData ResourceData { get; private set; } = null;
        public CharStats BaseStats { get; private set; } = null;

        BaseObject mBaseObj = null;

        public void Init(CharSaveData saveData, string resourceID)
        {
            mBaseObj = this.ExGetBase();

            ResourceData = TableDataContainer<CharResourceData>.Instance.GetInfo(resourceID);
            SaveData = saveData;

            UpdateBasicStat();
        }

        public void UpdateBasicStat()
        {
            int currentLevel = PlayerGrowth.GetLevelFromAccExp(SaveData.CurrentExp);
            int currentLevelIndex = currentLevel - 1;

            BaseStats = new CharStats();

            BaseStats.Attack = ResourceData._Attack.GetValueByBoth(SaveData.AttackPoint, currentLevelIndex);
            BaseStats.Defence = ResourceData._Defence.GetValueByBoth(SaveData.DefensePoint, currentLevelIndex);
            BaseStats.Health = ResourceData._Health.GetValueByBoth(SaveData.HealthPoint, currentLevelIndex);
            BaseStats.Mana = ResourceData._Mana.GetValueByBoth(SaveData.ManaPoint, currentLevelIndex);

            BaseStats.Shield = 0;
            BaseStats.MoveSpeed = _MoveSpeed;
            BaseStats.AttackSpeed = _AttackSpeed;
        }


        public void GetDisplayInfo(List<ReflectionFieldData> fieldDatas)
        {
            fieldDatas.Clear();

            fieldDatas.Add(new ReflectionFieldData() { FieldName = nameof(MaxHealth), Value = MaxHealth.ExFloorToInt().ToString() });
            fieldDatas.Add(new ReflectionFieldData() { FieldName = nameof(MaxMana), Value = MaxMana.ExFloorToInt().ToString() });
            fieldDatas.Add(new ReflectionFieldData() { FieldName = nameof(MaxShield), Value = MaxShield.ExFloorToInt().ToString() });
            fieldDatas.Add(new ReflectionFieldData() { FieldName = nameof(BaseAttack), Value = BaseAttack.ToString("0.#") });
            fieldDatas.Add(new ReflectionFieldData() { FieldName = nameof(PhyDefence), Value = PhyDefence.ToString("0.#") });
            fieldDatas.Add(new ReflectionFieldData() { FieldName = nameof(AttackSpeed), Value = AttackSpeed.ToString("0.#") });
            fieldDatas.Add(new ReflectionFieldData() { FieldName = nameof(MoveSpeed), Value = MoveSpeed.ToString("0.#") });
            fieldDatas.Add(new ReflectionFieldData() { FieldName = nameof(Option.CooltimeDown), Value = Option.CooltimeDown.ToString() });
            fieldDatas.Add(new ReflectionFieldData() { FieldName = nameof(Option.CriticalRate), Value = Option.CriticalRate.ToString() });
            fieldDatas.Add(new ReflectionFieldData() { FieldName = nameof(Option.CriticalAttack), Value = Option.CriticalAttack.ToString() });
            fieldDatas.Add(new ReflectionFieldData() { FieldName = nameof(HealthRegen), Value = HealthRegen.ToString("0.#") });
            fieldDatas.Add(new ReflectionFieldData() { FieldName = nameof(ManaRegen), Value = ManaRegen.ToString("0.#") });
            fieldDatas.Add(new ReflectionFieldData() { FieldName = nameof(ShieldRegen), Value = ShieldRegen.ToString("0.#") });
            fieldDatas.Add(new ReflectionFieldData() { FieldName = nameof(Option.FireResist), Value = Option.FireResist.ToString() });
            fieldDatas.Add(new ReflectionFieldData() { FieldName = nameof(Option.IceResist), Value = Option.IceResist.ToString() });
            fieldDatas.Add(new ReflectionFieldData() { FieldName = nameof(Option.LightningResist), Value = Option.LightningResist.ToString() });
            fieldDatas.Add(new ReflectionFieldData() { FieldName = nameof(Option.PosionResist), Value = Option.PosionResist.ToString() });
        }

    }
}