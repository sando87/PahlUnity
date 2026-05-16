using UnityEngine;
using UnityEngine.Events;

namespace PahlUnity
{
    public enum DamageType
    {
        Physics,
        Fire,
        Ice,
        Lightning,
        Poison,
    }
    public struct DamageInfo
    {
        public float PhyDamage;
        public float FireDamage;
        public float IceDamage;
        public float LightningDamage;
        public bool IsCritical;
        public PercentUp CriticalAttackUp;
        public GameObject Attacker;
        public Vector3 HitPoint;       // 타격 위치
        public Vector3 HitDirection;   // 넉백 방향

        public DamageInfo(float phyDamage)
        {
            PhyDamage = phyDamage;
            FireDamage = 0;
            IceDamage = 0;
            LightningDamage = 0;
            IsCritical = false;
            CriticalAttackUp = new PercentUp();
            Attacker = null;
            HitPoint = Vector3.zero;
            HitDirection = Vector3.zero;
        }
        public static implicit operator DamageInfo(float value)
        {
            return new DamageInfo(value);
        }
        public static implicit operator float(DamageInfo damage)
        {
            return damage.PhyDamage + damage.FireDamage + damage.IceDamage + damage.LightningDamage;
        }
        // public static DamageInfo operator +(DamageInfo a, DamageInfo b)
        // {
        //     return new DamageInfo(
        //         a.PhyDamage + b.PhyDamage,
        //         a.FireDamage + b.FireDamage,
        //         a.IceDamage + b.IceDamage,
        //         a.LightningDamage + b.LightningDamage,
        //         a.IsCritical,
        //         a.Attacker,
        //         a.HitPoint,
        //         a.HitDirection
        //     );
        // }

        public bool IsDamageType(DamageType _type)
        {
            switch (_type)
            {
                case DamageType.Physics: return PhyDamage > 0;
                case DamageType.Fire: return FireDamage > 0;
                case DamageType.Ice: return IceDamage > 0;
                case DamageType.Lightning: return LightningDamage > 0;
                default: break;
            }
            return false;
        }

        // public static DamageInfo operator *(DamageInfo damage, float multiplier)
        // {
        //     damage.PhyDamage *= multiplier;
        //     return damage;
        // }

        // public static DamageInfo operator *(float multiplier, DamageInfo damage)
        // {
        //     return damage * multiplier;
        // }


    }
}
