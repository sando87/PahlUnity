using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

namespace PahlUnity
{
    public class Health : MonoBehaviour, IHealth
    {
        public bool IsDead => mCurrentHP.ToInt() <= 0;

        public float HpRate => mMaxCurrentHP > 0 ? mCurrentHP / mMaxCurrentHP : 1;
        public float ManaRate => mMaxCurrentMana > 0 ? mCurrentMana / mMaxCurrentMana : 1;
        public float ShieldRate => mMaxCurrentShield > 0 ? mCurrentShield / mMaxCurrentShield : 1;

        public int MaxHealth => mMaxCurrentHP;
        public int MaxMana => mMaxCurrentMana;
        public int MaxShield => mMaxCurrentShield;

        public int CurrentHP => mCurrentHP.ToInt();
        public int CurrentMana => mCurrentMana.ToInt();
        public int CurrentShield => mCurrentShield.ToInt();

        public bool IsDirty { get => mIsDirty; set => mIsDirty = value; }

        // public float CurrentTemputure { get; set; } = 0;

        // public bool IsBurned { get; private set; }
        // public bool IsFreezed { get; private set; }

        protected int mMaxCurrentHP = 10;
        protected int mMaxCurrentMana = 0;
        protected int mMaxCurrentShield = 0;

        [SerializeField, NaughtyAttributes.ReadOnly]
        protected float mCurrentHP = 10;
        [SerializeField, NaughtyAttributes.ReadOnly]
        protected float mCurrentMana = 0;
        [SerializeField, NaughtyAttributes.ReadOnly]
        protected float mCurrentShield = 0;

        protected bool mIsDirty = false;

        public UnityEvent<DamagedResultInfo> OnDamaged = new UnityEvent<DamagedResultInfo>();
        public UnityEvent OnDied = new UnityEvent();

        protected BaseObject mBaseObj = null;
        protected SpecBase mSpec = null;

        protected virtual void Awake()
        {
            mBaseObj = this.ExGetBase();
            mSpec = mBaseObj.Spec;
        }

        protected virtual void Start()
        {
            UpdateMaxStats(false);

            // StartCoroutine(CoProcessBurnOrFreez());
        }

        public void UpdateMaxStats(bool keepRate)
        {
            if (keepRate)
            {
                float hpRate = HpRate;
                float manaRate = ManaRate;
                float shieldRate = ShieldRate;

                mMaxCurrentHP = mSpec.MaxHealth.ToInt();
                mMaxCurrentMana = mSpec.MaxMana.ToInt();
                mMaxCurrentShield = mSpec.MaxShield.ToInt();

                mCurrentHP = mMaxCurrentHP * hpRate;
                mCurrentMana = mMaxCurrentMana * manaRate;
                mCurrentShield = mMaxCurrentShield * shieldRate;
            }
            else
            {
                mMaxCurrentHP = mSpec.MaxHealth.ToInt();
                mMaxCurrentMana = mSpec.MaxMana.ToInt();
                mMaxCurrentShield = mSpec.MaxShield.ToInt();

                mCurrentHP = mMaxCurrentHP;
                mCurrentMana = mMaxCurrentMana;
                mCurrentShield = mMaxCurrentShield;
            }

            mIsDirty = true;
        }

        DamagedResultInfo CalcHitResult(DamageInfo damageInfo)
        {
            DamagedResultInfo damageRetInfo = new DamagedResultInfo();
            damageRetInfo.MaxHealth = mMaxCurrentHP;
            damageRetInfo.BeforeHealth = mCurrentHP.ToInt();
            damageRetInfo.OriDamage = (damageInfo.PhyDamage + damageInfo.FireDamage + damageInfo.IceDamage + damageInfo.LightningDamage).ToInt();

            // 물리 데미지 계산
            float phyDamage = damageInfo.IsCritical ? damageInfo.PhyDamage * damageInfo.CriticalAttackUp : damageInfo.PhyDamage;
            damageRetInfo.TotalDamage = phyDamage.ToInt();

            // 파이어 데미지 계산
            float fireDamage = damageInfo.FireDamage * (Percent.One - mSpec.Option.FireResist);
            fireDamage.ExSetMinimum(0);
            damageRetInfo.TotalDamage += fireDamage.ToInt();

            // 아이스 데미지 계산
            float iceDamage = damageInfo.IceDamage * (Percent.One - mSpec.Option.IceResist);
            iceDamage.ExSetMinimum(0);
            damageRetInfo.TotalDamage += iceDamage.ToInt();

            // 라이트닝 데미지 계산
            float lightningDamage = damageInfo.LightningDamage * (Percent.One - mSpec.Option.LightningResist);
            lightningDamage.ExSetMinimum(0);
            damageRetInfo.TotalDamage += lightningDamage.ToInt();

            return damageRetInfo;
        }

        public void GetDamaged(DamageInfo damage)
        {
            if (IsDead || damage <= 0) return;

            DamagedResultInfo damageRetInfo = CalcHitResult(damage);

            // float fireTemputure = (damage.FireDamage / mMaxCurrentHP) * 100f;
            // float iceTemputure = (damage.IceDamage / mMaxCurrentHP) * 100f;
            // CurrentTemputure += (fireTemputure - iceTemputure);

            int remainDamage = damageRetInfo.TotalDamage;

            if (mCurrentShield > 0)
            {
                int usedShield = Mathf.Min(mCurrentShield.ToInt(), remainDamage);
                mCurrentShield -= usedShield;
                remainDamage -= usedShield;
                damageRetInfo.ValidDamage += usedShield;
            }

            if (remainDamage > 0)
            {
                remainDamage -= mSpec.PhyDefence.ToInt();
                remainDamage.ExSetMinimum(0);
                damageRetInfo.ValidDamage += remainDamage;

                mCurrentHP -= remainDamage;
                mCurrentHP.ExSetMinimum(0);

                damageRetInfo.AfterHealth = mCurrentHP.ToInt();
                mIsDirty = true;
                OnDamaged.Invoke(damageRetInfo);

                if (IsDead)
                {
                    // RemoveSlowEffect();
                    OnDied.Invoke();
                }
            }
        }
        public void GetDied()
        {
            if (IsDead) return;

            // RemoveSlowEffect();
            mCurrentHP = 0;
            mIsDirty = true;
            OnDied.Invoke();
        }
        public void Heal(float amount)
        {
            if (IsDead || amount <= 0) return;
            mCurrentHP += amount;
            mCurrentHP.ExSetMaximum(mMaxCurrentHP);
            mIsDirty = true;
        }
        public void UseMana(float amount)
        {
            mCurrentMana -= amount;
            mCurrentMana.ExSetMinimum(0);
            mIsDirty = true;
        }
        public void RestoreMana(float amount)
        {
            if (IsDead || amount <= 0) return;
            mCurrentMana += amount;
            mCurrentMana.ExSetMaximum(mMaxCurrentMana);
            mIsDirty = true;
        }
        public void RestoreShield(float amount)
        {
            if (IsDead || amount <= 0) return;
            mCurrentShield += amount;
            mCurrentShield.ExSetMaximum(mMaxCurrentShield);
            mIsDirty = true;
        }


        // IEnumerator CoProcessBurnOrFreez()
        // {
        //     while (true)
        //     {
        //         yield return new WaitUntil(() => CurrentTemputure < -10 || 10 < CurrentTemputure);

        //         if (CurrentTemputure > 10)
        //         {
        //             while (CurrentTemputure > 0)
        //             {
        //                 ApplyBurnEffect();
        //                 yield return newWaitForSeconds.Cache(1);
        //                 CurrentTemputure -= 4;
        //             }
        //             RemoveBurnEffect();
        //         }
        //         else if (CurrentTemputure < -10)
        //         {
        //             while (CurrentTemputure < 0)
        //             {
        //                 ApplySlowEffect();
        //                 yield return newWaitForSeconds.Cache(1);
        //                 CurrentTemputure += 4;
        //             }
        //             RemoveSlowEffect();
        //         }
        //     }
        // }

        // void ApplyBurnEffect()
        // {
        //     if (IsDead)
        //         return;

        //     // 데미지 감소 처리
        //     float curTemp = CurrentTemputure;
        //     curTemp.ExSetMaximum(20);
        //     float tempRate = curTemp / 20f;
        //     float damage = tempRate * 10f;

        //     // 온도에 따른 이펙트 크기 감소 처리
        //     mBaseObj.Renderer.SetColor(StringHashes.ColorBurn, new Color(0.5f, 0.25f, 0));
        //     mBaseObj.Renderer.SetBurnRate(tempRate);

        //     DamagedResultInfo damagedResultInfo = new DamagedResultInfo();
        //     damagedResultInfo.MaxHealth = mMaxCurrentHP;
        //     damagedResultInfo.BeforeHealth = mCurrentHP;

        //     mCurrentHP -= damage;
        //     mCurrentHP.ExSetMinimum(0);

        //     damagedResultInfo.OriDamage = damage;
        //     damagedResultInfo.TotalDamage = damage;
        //     damagedResultInfo.ValidDamage = damage.ToInt();
        //     damagedResultInfo.AfterHealth = mCurrentHP;

        //     OnDamaged.Invoke(damagedResultInfo);

        //     IsBurned = true;

        //     if (IsDead)
        //     {
        //         RemoveBurnEffect();
        //         OnDied.Invoke();
        //     }
        // }
        // void RemoveBurnEffect()
        // {
        //     IsBurned = false;
        //     mBaseObj.Renderer.UnSetColor(StringHashes.ColorBurn);
        //     mBaseObj.Renderer.SetBurnRate(0);
        // }

        // void ApplySlowEffect()
        // {
        //     if (IsDead)
        //         return;

        //     // 이속 감소 처리
        //     int buffID = mBaseObj.GetInstanceID();
        //     float moveSpeedUp = CurrentTemputure * 4;
        //     mBaseObj.Buffs.SetMoveSpeedBuff(buffID, new PercentUp(moveSpeedUp));

        //     mBaseObj.Renderer.SetColor(StringHashes.ColorFreez, Color.blue);

        //     IsFreezed = true;
        // }
        // void RemoveSlowEffect()
        // {
        //     int buffID = mBaseObj.GetInstanceID();
        //     mBaseObj.Buffs.RemoveBuff(buffID);

        //     mBaseObj.Renderer.UnSetColor(StringHashes.ColorFreez);

        //     IsFreezed = false;
        // }
    }
}
