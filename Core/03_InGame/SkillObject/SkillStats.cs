
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

namespace PahlUnity
{
    [System.Serializable]
    public class SkillStats
    {
        public float ManaUse;
        public float Cooltime;
        public float ProjectileCount;
        public float ProjectileSpeed;
        public float AttackRange;
        public float SplashRange;
        public float Duration;
        public float Interval;
        public float StartDelay;

        public Percent PhyAttack;
        public Percent FireAttack;
        public Percent IceAttack;
        public Percent LightningAttack;
    }
}