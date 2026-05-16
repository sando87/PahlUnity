using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlBit
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
        public float MoveSpeed => BaseStats.MoveSpeed * Option.MoveSpeedUp;
        public float AttackSpeed => BaseStats.AttackSpeed * Option.AttackSpeedUp;
        public float HealthRegen => Option.HealthRegen;
        public float ManaRegen => Option.ManaRegen;
        public float ShieldRegen => Option.ShieldRegen;

        public CharSaveData SaveData { get; private set; } = null;
        public CharResourceData ResourceData { get; private set; } = null;
        public CharStats BaseStats { get; private set; } = null;

        BaseObject mBaseObj = null;

        public void Init(int characterID, string resourceID)
        {
            mBaseObj = this.ExGetBase();

            ResourceData = CharResourceTable.Instance.GetInfo(resourceID);
            UserSaveData userSaveData = SaveFileManager<UserSaveData>.Load();
            SaveData = userSaveData.Characters[characterID].Stats;

            UpdateBasicStat();
        }

        public void UpdateBasicStat()
        {
            int currentLevel = GameSystem.GetLevelFromAccExp(SaveData.CurrentExp);
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


        public void GetDisplayInfo(List<FieldData> fieldDatas)
        {
            fieldDatas.Clear();

            fieldDatas.Add(new FieldData() { Name = nameof(MaxHealth), Value = MaxHealth.ToInt().ToString() });
            fieldDatas.Add(new FieldData() { Name = nameof(MaxMana), Value = MaxMana.ToInt().ToString() });
            fieldDatas.Add(new FieldData() { Name = nameof(BaseAttack), Value = BaseAttack.ToInt().ToString() });
            fieldDatas.Add(new FieldData() { Name = nameof(PhyDefence), Value = PhyDefence.ToInt().ToString() });
            fieldDatas.Add(new FieldData() { Name = nameof(AttackSpeed), Value = AttackSpeed.ToString("0.#") });
            fieldDatas.Add(new FieldData() { Name = nameof(MoveSpeed), Value = MoveSpeed.ToInt().ToString() });
            fieldDatas.Add(new FieldData() { Name = nameof(MaxShield), Value = MaxShield.ToInt().ToString() });
            fieldDatas.Add(new FieldData() { Name = nameof(Option.CooltimeDown), Value = Option.CooltimeDown.ToString() });
            fieldDatas.Add(new FieldData() { Name = nameof(Option.CriticalRate), Value = Option.CriticalRate.ToString() });
            fieldDatas.Add(new FieldData() { Name = nameof(Option.CriticalAttack), Value = Option.CriticalAttack.ToString() });
            fieldDatas.Add(new FieldData() { Name = nameof(HealthRegen), Value = HealthRegen.ToString("0.#") });
            fieldDatas.Add(new FieldData() { Name = nameof(ManaRegen), Value = ManaRegen.ToString("0.#") });
            fieldDatas.Add(new FieldData() { Name = nameof(ShieldRegen), Value = ShieldRegen.ToString("0.#") });
            fieldDatas.Add(new FieldData() { Name = nameof(Option.FireResist), Value = Option.FireResist.ToString() });
            fieldDatas.Add(new FieldData() { Name = nameof(Option.IceResist), Value = Option.IceResist.ToString() });
            fieldDatas.Add(new FieldData() { Name = nameof(Option.LightningResist), Value = Option.LightningResist.ToString() });
            fieldDatas.Add(new FieldData() { Name = nameof(Option.PosionResist), Value = Option.PosionResist.ToString() });
        }

    }
}