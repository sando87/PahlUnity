using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using NaughtyAttributes.Test;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlUnity
{
    public class SpecSkill : SpecBase
    {
        public float PhyAttack => mSpecBase.BaseAttack * (BaseStats.PhyAttack + mSpecBase.Option.PhyAttack);
        public float FireAttack => mSpecBase.BaseAttack * (BaseStats.FireAttack + mSpecBase.Option.FireAttack);
        public float IceAttack => mSpecBase.BaseAttack * (BaseStats.IceAttack + mSpecBase.Option.IceAttack);
        public float LightningAttack => mSpecBase.BaseAttack * (BaseStats.LightningAttack + mSpecBase.Option.LightningAttack);

        public float ManaUse => BaseStats.ManaUse;
        public float Cooltime => BaseStats.Cooltime * mSpecBase.Option.CooltimeDown;
        public float ProjectileCount => BaseStats.ProjectileCount + mSpecBase.Option.ProjectileCountUp;
        public float ProjectileSpeed => BaseStats.ProjectileSpeed * mSpecBase.Option.ProjectileSpeedUp;
        public float AttackRange => BaseStats.AttackRange * mSpecBase.Option.AttackRangeUp;
        public float SplashRange => BaseStats.SplashRange * mSpecBase.Option.SplashRangeUp;
        public float Duration => BaseStats.Duration * mSpecBase.Option.DurationUp;
        public float Interval => BaseStats.Interval;
        public float StartDelay => BaseStats.StartDelay;

        public SkillSaveData SaveData { get; private set; } = null;
        public SkillResourceData ResourceData { get; private set; } = null;
        public SkillStats BaseStats { get; private set; } = null;

        private SpecBase mSpecBase = null;

        public void Init(int characterID, string resourceID)
        {
            // ResourceData = SkillResourceTable.Instance.GetInfo(resourceID);
            // UserSaveData userSaveData = SaveFileManager<UserSaveData>.Load();
            // SaveData = userSaveData.Characters[characterID].Skills[resourceID];

            mSpecBase = this.ExGetBase().Spec;

            BaseStats = GetBasicStatByLevel(SaveData == null ? 0 : SaveData.LevelIndex);
        }

        public void UpdateBasicStat()
        {
            BaseStats = GetBasicStatByLevel(SaveData == null ? 0 : SaveData.LevelIndex);
        }

        SkillStats GetBasicStatByLevel(int currentLevelIndex)
        {
            SkillStats baseStats = new SkillStats();

            baseStats.ManaUse = ResourceData._ManaUse.GetValueByLevel(currentLevelIndex);
            baseStats.Cooltime = ResourceData._Cooltime.GetValueByLevel(currentLevelIndex);
            baseStats.ProjectileCount = ResourceData._ProjectileCount.GetValueByLevel(currentLevelIndex);
            baseStats.ProjectileSpeed = ResourceData._ProjectileSpeed.GetValueByLevel(currentLevelIndex);
            baseStats.AttackRange = ResourceData._AttackRange.GetValueByLevel(currentLevelIndex);
            baseStats.SplashRange = ResourceData._SplashRange.GetValueByLevel(currentLevelIndex);
            baseStats.Duration = ResourceData._Duration.GetValueByLevel(currentLevelIndex);
            baseStats.Interval = ResourceData._Interval.GetValueByLevel(currentLevelIndex);
            baseStats.StartDelay = ResourceData._StartDelay.GetValueByLevel(currentLevelIndex);
            baseStats.PhyAttack = (Percent)ResourceData._PhyAttack.GetValueByLevel(currentLevelIndex);
            baseStats.FireAttack = (Percent)ResourceData._FireAttack.GetValueByLevel(currentLevelIndex);
            baseStats.IceAttack = (Percent)ResourceData._IceAttack.GetValueByLevel(currentLevelIndex);
            baseStats.LightningAttack = (Percent)ResourceData._LightningAttack.GetValueByLevel(currentLevelIndex);

            return baseStats;
        }

        public void GetDisplayInfo(List<ReflectionFieldData> fieldDatas)
        {
            fieldDatas.Clear();

            if (PhyAttack > 0) fieldDatas.Add(new ReflectionFieldData() { FieldName = nameof(PhyAttack), Value = PhyAttack.ToInt().ToString() });
            if (FireAttack > 0) fieldDatas.Add(new ReflectionFieldData() { FieldName = nameof(FireAttack), Value = FireAttack.ToInt().ToString() });
            if (IceAttack > 0) fieldDatas.Add(new ReflectionFieldData() { FieldName = nameof(IceAttack), Value = IceAttack.ToInt().ToString() });
            if (LightningAttack > 0) fieldDatas.Add(new ReflectionFieldData() { FieldName = nameof(LightningAttack), Value = LightningAttack.ToInt().ToString() });
            if (ManaUse > 0) fieldDatas.Add(new ReflectionFieldData() { FieldName = nameof(ManaUse), Value = ManaUse.ToInt().ToString() });
            if (Cooltime > 0) fieldDatas.Add(new ReflectionFieldData() { FieldName = nameof(Cooltime), Value = $"{Cooltime:0.#}s" });
            if (ProjectileCount > 0) fieldDatas.Add(new ReflectionFieldData() { FieldName = nameof(ProjectileCount), Value = ProjectileCount.ToInt().ToString() });
            if (ProjectileSpeed > 0) fieldDatas.Add(new ReflectionFieldData() { FieldName = nameof(ProjectileSpeed), Value = $"{ProjectileSpeed:0.#}" });
            if (AttackRange > 0) fieldDatas.Add(new ReflectionFieldData() { FieldName = nameof(AttackRange), Value = $"{AttackRange:0.#}" });
            if (SplashRange > 0) fieldDatas.Add(new ReflectionFieldData() { FieldName = nameof(SplashRange), Value = $"{SplashRange:0.#}" });
            if (Duration > 0) fieldDatas.Add(new ReflectionFieldData() { FieldName = nameof(Duration), Value = $"{Duration:0.#}s" });
            if (Interval > 0) fieldDatas.Add(new ReflectionFieldData() { FieldName = nameof(Interval), Value = $"{Interval:0.#}s" });
            if (StartDelay > 0) fieldDatas.Add(new ReflectionFieldData() { FieldName = nameof(StartDelay), Value = $"{StartDelay:0.#}s" });
        }

        public void GetBasicStatInfo(List<ReflectionFieldData> fieldDatas)
        {
            fieldDatas.Clear();

            SkillStats nextBaseStats = GetBasicStatByLevel(SaveData.LevelIndex + 1);

            if (BaseStats.PhyAttack.PercentValue > 0)
                fieldDatas.Add(new ReflectionFieldData()
                {
                    FieldName = nameof(BaseStats.PhyAttack),
                    Value = BaseStats.PhyAttack.ToString(),
                    TypeName = SaveData.IsLearned ? nextBaseStats.PhyAttack.ToString() : ""
                });

            if (BaseStats.FireAttack.PercentValue > 0)
                fieldDatas.Add(new ReflectionFieldData()
                {
                    FieldName = nameof(BaseStats.FireAttack),
                    Value = BaseStats.FireAttack.ToString(),
                    TypeName = SaveData.IsLearned ? nextBaseStats.FireAttack.ToString() : ""
                });

            if (BaseStats.IceAttack.PercentValue > 0)
                fieldDatas.Add(new ReflectionFieldData()
                {
                    FieldName = nameof(BaseStats.IceAttack),
                    Value = BaseStats.IceAttack.ToString(),
                    TypeName = SaveData.IsLearned ? nextBaseStats.IceAttack.ToString() : ""
                });

            if (BaseStats.LightningAttack.PercentValue > 0)
                fieldDatas.Add(new ReflectionFieldData()
                {
                    FieldName = nameof(BaseStats.LightningAttack),
                    Value = BaseStats.LightningAttack.ToString(),
                    TypeName = SaveData.IsLearned ? nextBaseStats.LightningAttack.ToString() : ""
                });

            if (BaseStats.ManaUse > 0)
                fieldDatas.Add(new ReflectionFieldData()
                {
                    FieldName = nameof(BaseStats.ManaUse),
                    Value = BaseStats.ManaUse.ToString(),
                    TypeName = SaveData.IsLearned ? nextBaseStats.ManaUse.ToString() : ""
                });

            if (BaseStats.Cooltime > 0)
                fieldDatas.Add(new ReflectionFieldData()
                {
                    FieldName = nameof(BaseStats.Cooltime),
                    Value = BaseStats.Cooltime.ToString("0.#") + "s",
                    TypeName = SaveData.IsLearned ? nextBaseStats.Cooltime.ToString("0.#") + "s" : ""
                });

            if (BaseStats.ProjectileCount > 0)
                fieldDatas.Add(new ReflectionFieldData()
                {
                    FieldName = nameof(BaseStats.ProjectileCount),
                    Value = BaseStats.ProjectileCount.ToString(),
                    TypeName = SaveData.IsLearned ? nextBaseStats.ProjectileCount.ToString() : ""
                });

            if (BaseStats.ProjectileSpeed > 0)
                fieldDatas.Add(new ReflectionFieldData()
                {
                    FieldName = nameof(BaseStats.ProjectileSpeed),
                    Value = BaseStats.ProjectileSpeed.ToString("0.#"),
                    TypeName = SaveData.IsLearned ? nextBaseStats.ProjectileSpeed.ToString("0.#") : ""
                });

            if (BaseStats.AttackRange > 0)
                fieldDatas.Add(new ReflectionFieldData()
                {
                    FieldName = nameof(BaseStats.AttackRange),
                    Value = BaseStats.AttackRange.ToString("0.#"),
                    TypeName = SaveData.IsLearned ? nextBaseStats.AttackRange.ToString("0.#") : ""
                });

            if (BaseStats.SplashRange > 0)
                fieldDatas.Add(new ReflectionFieldData()
                {
                    FieldName = nameof(BaseStats.SplashRange),
                    Value = BaseStats.SplashRange.ToString("0.#"),
                    TypeName = SaveData.IsLearned ? nextBaseStats.SplashRange.ToString("0.#") : ""
                });

            if (BaseStats.Duration > 0)
                fieldDatas.Add(new ReflectionFieldData()
                {
                    FieldName = nameof(BaseStats.Duration),
                    Value = BaseStats.Duration.ToString("0.#") + "s",
                    TypeName = SaveData.IsLearned ? nextBaseStats.Duration.ToString("0.#") + "s" : ""
                });

            if (BaseStats.Interval > 0)
                fieldDatas.Add(new ReflectionFieldData()
                {
                    FieldName = nameof(BaseStats.Interval),
                    Value = BaseStats.Interval.ToString("0.#") + "s",
                    TypeName = SaveData.IsLearned ? nextBaseStats.Interval.ToString("0.#") + "s" : ""
                });

            if (BaseStats.StartDelay > 0)
                fieldDatas.Add(new ReflectionFieldData()
                {
                    FieldName = nameof(BaseStats.StartDelay),
                    Value = BaseStats.StartDelay.ToString("0.#") + "s",
                    TypeName = SaveData.IsLearned ? nextBaseStats.StartDelay.ToString("0.#") + "s" : ""
                });
        }
    }
}