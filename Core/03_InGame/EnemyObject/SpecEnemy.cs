using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class SpecEnemy : SpecBase
    {
        public override float MaxHealth => BaseStats.Health * Option.HealthUp;
        public override float MaxMana => BaseStats.Mana * Option.ManaUp;
        public override float MaxShield => BaseStats.Shield;

        public float HealthRegen => Option.HealthRegen;
        public float ManaRegen => Option.ManaRegen;
        public float ShieldRegen => Option.ShieldRegen;

        public override float BaseAttack => BaseStats.Attack + Option.BaseAttackAdd;
        public override float PhyDefence => BaseStats.Defence * Option.DefenceUp;

        public float MoveSpeed => BaseStats.MoveSpeed * Option.MoveSpeedUp;
        public float AttackSpeed => BaseStats.AttackSpeed * Option.AttackSpeedUp;
        public float AttackInterval => 1 / AttackSpeed;
        public float DetectRange => BaseStats.DetectRange;
        public float AttackRange => BaseStats.AttackRange;
        public float DetectLossRange => DetectRange * 10f;
        public PercentUp ItemDrop => BaseStats.ItemDrop;
        public int GoldOnDeath => BaseStats.GoldOnDeath;
        public float ExpOnDeath => BaseStats.ExpOnDeath;
        public Percent HitChance => BaseStats.HitChance;

        public EnemyResourceData ResourceData { get; private set; } = null;
        public EnemyStats BaseStats { get; private set; } = null;

        public void InitData(string resourceID)
        {
            ResourceData = EnemyResourceTable.Instance.GetInfo(resourceID);
            UpdateBasicStats();
        }
        void UpdateBasicStats()
        {
            BaseStats = new EnemyStats();

            BaseStats.Health = ResourceData._Health.GetValue();
            BaseStats.Mana = ResourceData._Mana.GetValue();
            BaseStats.Shield = ResourceData._Shield.GetValue();
            BaseStats.Attack = ResourceData._Attack.GetValue();
            BaseStats.Defence = ResourceData._Defence.GetValue();
            BaseStats.MoveSpeed = ResourceData._MoveSpeed.GetValue();
            BaseStats.AttackSpeed = ResourceData._AttackSpeed.GetValue();
            BaseStats.Cooltime = ResourceData._Cooltime.GetValue();
            BaseStats.DetectRange = ResourceData._DetectRange.GetValue();
            BaseStats.AttackRange = ResourceData._AttackRange.GetValue();
            BaseStats.ItemDrop = (PercentUp)ResourceData._ItemDrop.GetValue();
            BaseStats.GoldOnDeath = ResourceData._GoldOnDeath.GetIntInRange(MyUtils.RandomRate());
            BaseStats.ExpOnDeath = ResourceData._ExpOnDeath.GetValue();
            BaseStats.HitChance = (Percent)ResourceData._HitChance.GetValue();
        }
    }
}