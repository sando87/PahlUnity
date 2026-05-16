using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace PahlBit
{
    public class SkillResourceTable : DatabaseCSV<SkillResourceData> { }

    [System.Serializable]
    public class SkillResourceData : ICSVFormat
    {
        public readonly string SkillID;
        public readonly string DisplayName;
        public readonly string Desc;

        public readonly string UnlockLevel;
        public readonly string UpgradeStep;
        public readonly string ManaUse;
        public readonly string Cooltime;
        public readonly string ProjectileCount;
        public readonly string ProjectileSpeed;
        public readonly string AttackRange;
        public readonly string SplashRange;
        public readonly string Duration;
        public readonly string Interval;
        public readonly string StartDelay;
        public readonly string PhyAttack;
        public readonly string FireAttack;
        public readonly string IceAttack;
        public readonly string LightningAttack;

        public int RowIndex { get; set; } // 데이터데이블상에 존재하는 순서
        public long ID { get { return ICSVFormat.ToID(SkillID); } } // 데이터 접근을 위한 id값

        public int _UnlockLevel { get; private set; }
        public int[] _UpgradeStep { get; private set; }
        public ParseValue _ManaUse { get; private set; }
        public ParseValue _Cooltime { get; private set; }
        public ParseValue _ProjectileCount { get; private set; }
        public ParseValue _ProjectileSpeed { get; private set; }
        public ParseValue _AttackRange { get; private set; }
        public ParseValue _SplashRange { get; private set; }
        public ParseValue _Duration { get; private set; }
        public ParseValue _Interval { get; private set; }
        public ParseValue _StartDelay { get; private set; }
        public ParseValue _PhyAttack { get; private set; }
        public ParseValue _FireAttack { get; private set; }
        public ParseValue _IceAttack { get; private set; }
        public ParseValue _LightningAttack { get; private set; }


        void ICSVFormat.OnLoad()
        {
            _UnlockLevel = int.Parse(UnlockLevel);
            string[] steps = UpgradeStep.Split('/');
            _UpgradeStep = new int[steps.Length];
            for (int i = 0; i < steps.Length; i++)
                _UpgradeStep[i] = int.Parse(steps[i]);

            _ManaUse = ParseValue.Parse(ManaUse);
            _Cooltime = ParseValue.Parse(Cooltime);
            _ProjectileCount = ParseValue.Parse(ProjectileCount);
            _ProjectileSpeed = ParseValue.Parse(ProjectileSpeed);
            _AttackRange = ParseValue.Parse(AttackRange);
            _SplashRange = ParseValue.Parse(SplashRange);
            _Duration = ParseValue.Parse(Duration);
            _Interval = ParseValue.Parse(Interval);
            _StartDelay = ParseValue.Parse(StartDelay);
            _PhyAttack = ParseValue.Parse(PhyAttack);
            _FireAttack = ParseValue.Parse(FireAttack);
            _IceAttack = ParseValue.Parse(IceAttack);
            _LightningAttack = ParseValue.Parse(LightningAttack);
        }
    }

}