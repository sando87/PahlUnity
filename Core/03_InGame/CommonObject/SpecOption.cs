
namespace PahlUnity
{
    [System.Serializable]
    public class SpecOption
    {
        public PercentUp HealthUp;
        public float HealthRegen;
        public PercentUp ManaUp;
        public float ManaRegen;
        public float BaseAttackAdd;
        public Percent PhyAttack;
        public Percent FireAttack;
        public Percent IceAttack;
        public Percent LightningAttack;
        public PercentUp DefenceUp;
        public PercentUp MoveSpeedUp;
        public PercentUp AttackSpeedUp;
        public PercentUp CooltimeDown;
        public float ShieldAdd;
        public float ShieldRegen;
        public PercentUp CriticalRate;
        public PercentUp CriticalAttack;
        public float ProjectileCountUp;
        public PercentUp ProjectileSpeedUp;
        public PercentUp AttackRangeUp;
        public PercentUp SplashRangeUp;
        public PercentUp DurationUp;
        public Percent FireResist;
        public Percent IceResist;
        public Percent LightningResist;
        public Percent PosionResist;

        public bool IsDirty { get; set; } = true;

        // ----- += 연산 메서드 -----
        public void Add(SpecOption other)
        {
            IsDirty = true;

            HealthUp += other.HealthUp;
            HealthRegen += other.HealthRegen;
            ManaUp += other.ManaUp;
            ManaRegen += other.ManaRegen;
            BaseAttackAdd += other.BaseAttackAdd;
            PhyAttack += other.PhyAttack;
            FireAttack += other.FireAttack;
            IceAttack += other.IceAttack;
            LightningAttack += other.LightningAttack;
            DefenceUp += other.DefenceUp;
            MoveSpeedUp += other.MoveSpeedUp;
            AttackSpeedUp += other.AttackSpeedUp;
            CooltimeDown += other.CooltimeDown;
            ShieldAdd += other.ShieldAdd;
            ShieldRegen += other.ShieldRegen;
            CriticalRate += other.CriticalRate;
            CriticalAttack += other.CriticalAttack;
            ProjectileCountUp += other.ProjectileCountUp;
            ProjectileSpeedUp += other.ProjectileSpeedUp;
            AttackRangeUp += other.AttackRangeUp;
            SplashRangeUp += other.SplashRangeUp;
            DurationUp += other.DurationUp;
            FireResist += other.FireResist;
            IceResist += other.IceResist;
            LightningResist += other.LightningResist;
            PosionResist += other.PosionResist;
        }

        // ----- -= 연산 메서드 -----
        public void Subtract(SpecOption other)
        {
            IsDirty = true;

            HealthUp -= other.HealthUp;
            HealthRegen -= other.HealthRegen;
            ManaUp -= other.ManaUp;
            ManaRegen -= other.ManaRegen;
            BaseAttackAdd -= other.BaseAttackAdd;
            PhyAttack -= other.PhyAttack;
            FireAttack -= other.FireAttack;
            IceAttack -= other.IceAttack;
            LightningAttack -= other.LightningAttack;
            DefenceUp -= other.DefenceUp;
            MoveSpeedUp -= other.MoveSpeedUp;
            AttackSpeedUp -= other.AttackSpeedUp;
            CooltimeDown -= other.CooltimeDown;
            ShieldAdd -= other.ShieldAdd;
            ShieldRegen -= other.ShieldRegen;
            CriticalRate -= other.CriticalRate;
            CriticalAttack -= other.CriticalAttack;
            ProjectileCountUp -= other.ProjectileCountUp;
            ProjectileSpeedUp -= other.ProjectileSpeedUp;
            AttackRangeUp -= other.AttackRangeUp;
            SplashRangeUp -= other.SplashRangeUp;
            DurationUp -= other.DurationUp;
            FireResist -= other.FireResist;
            IceResist -= other.IceResist;
            LightningResist -= other.LightningResist;
            PosionResist -= other.PosionResist;
        }

        public void SetAllZero()
        {
            IsDirty = true;

            HealthUp.SetZero();
            HealthRegen = 0;
            ManaUp.SetZero();
            ManaRegen = 0;
            BaseAttackAdd = 0;
            PhyAttack.SetZero();
            FireAttack.SetZero();
            IceAttack.SetZero();
            LightningAttack.SetZero();
            DefenceUp.SetZero();
            MoveSpeedUp.SetZero();
            AttackSpeedUp.SetZero();
            CooltimeDown.SetZero();
            ShieldAdd = 0;
            ShieldRegen = 0;
            CriticalRate.SetZero();
            CriticalAttack.SetZero();
            ProjectileCountUp = 0;
            ProjectileSpeedUp.SetZero();
            AttackRangeUp.SetZero();
            SplashRangeUp.SetZero();
            DurationUp.SetZero();
            FireResist.SetZero();
            IceResist.SetZero();
            LightningResist.SetZero();
            PosionResist.SetZero();
        }

    }

}