using System;
using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

namespace PahlUnity
{
    public class Health : MonoBehaviour
    {
        [SerializeField] UnityEvent<HealthInfo, HealthInfo> _OnChanged;
        [SerializeField] UnityEvent<HealthInfo, HealthInfo> _OnDied;

        public event Action<HealthInfo, HealthInfo> OnChanged;
        public event Action<HealthInfo, HealthInfo> OnDied;

        public bool IsDead => mCurrentHP.ExFloorToInt() <= 0;

        public float HpRate => mMaxCurrentHP > 0 ? mCurrentHP / mMaxCurrentHP : 1;
        public float ManaRate => mMaxCurrentMana > 0 ? mCurrentMana / mMaxCurrentMana : 1;
        public float ShieldRate => mMaxCurrentShield > 0 ? mCurrentShield / mMaxCurrentShield : 1;

        public int MaxHealth => mMaxCurrentHP;
        public int MaxMana => mMaxCurrentMana;
        public int MaxShield => mMaxCurrentShield;

        public float CurrentHP => mCurrentHP;
        public float CurrentMana => mCurrentMana;
        public float CurrentShield => mCurrentShield;

        protected int mMaxCurrentHP = 10;
        protected int mMaxCurrentMana = 0;
        protected int mMaxCurrentShield = 0;

        [SerializeField, NaughtyAttributes.ReadOnly]
        protected float mCurrentHP = 10;
        [SerializeField, NaughtyAttributes.ReadOnly]
        protected float mCurrentMana = 0;
        [SerializeField, NaughtyAttributes.ReadOnly]
        protected float mCurrentShield = 0;

        protected BaseObject mBaseObj = null;

        protected virtual void Awake()
        {
            mBaseObj = this.ExGetBase();
        }

        public void SetMaxStats(float maxHP, float maxMana, float maxShield, bool keepRate)
        {
            HealthInfo before = GetCurrentHealthInfo();
            if (keepRate)
            {
                float hpRate = HpRate;
                float manaRate = ManaRate;
                float shieldRate = ShieldRate;

                mMaxCurrentHP = maxHP.ExFloorToInt();
                mMaxCurrentMana = maxMana.ExFloorToInt();
                mMaxCurrentShield = maxShield.ExFloorToInt();

                mCurrentHP = mMaxCurrentHP * hpRate;
                mCurrentMana = mMaxCurrentMana * manaRate;
                mCurrentShield = mMaxCurrentShield * shieldRate;
            }
            else
            {
                mMaxCurrentHP = maxHP.ExFloorToInt();
                mMaxCurrentMana = maxMana.ExFloorToInt();
                mMaxCurrentShield = maxShield.ExFloorToInt();

                mCurrentHP = mMaxCurrentHP;
                mCurrentMana = mMaxCurrentMana;
                mCurrentShield = mMaxCurrentShield;
            }

            HealthInfo after = GetCurrentHealthInfo();
            InvokeHealthChanged(before, after);
        }

        public void SetNewHealth(float newHP, float newMana, float newShield)
        {
            if (IsDead) return;

            HealthInfo before = GetCurrentHealthInfo();
            mCurrentHP = newHP;
            mCurrentHP.ExSetMaximum(mMaxCurrentHP);
            mCurrentMana = newMana;
            mCurrentMana.ExSetMaximum(mMaxCurrentMana);
            mCurrentShield = newShield;
            mCurrentShield.ExSetMaximum(mMaxCurrentShield);
            HealthInfo after = GetCurrentHealthInfo();

            if (IsDead)
            {
                InvokeHealthDied(before, after);
            }
            else
            {
                InvokeHealthChanged(before, after);
            }
        }
        public void GetDamaged(float damage)
        {
            if (IsDead || damage <= 0) return;

            HealthInfo before = GetCurrentHealthInfo();
            mCurrentHP -= damage;
            mCurrentHP.ExSetClamp(0, mMaxCurrentHP);
            HealthInfo after = GetCurrentHealthInfo();

            if (IsDead)
            {
                InvokeHealthDied(before, after);
            }
            else
            {
                InvokeHealthChanged(before, after);
            }
        }
        public void GetDied()
        {
            if (IsDead) return;

            HealthInfo before = GetCurrentHealthInfo();
            mCurrentHP = 0;
            mCurrentShield = 0;
            mCurrentMana = 0;
            HealthInfo after = GetCurrentHealthInfo();
            InvokeHealthDied(before, after);
        }
        public void Heal(float amount)
        {
            if (IsDead || amount <= 0) return;
            HealthInfo before = GetCurrentHealthInfo();
            mCurrentHP += amount;
            mCurrentHP.ExSetMaximum(mMaxCurrentHP);
            HealthInfo after = GetCurrentHealthInfo();
            InvokeHealthChanged(before, after);
        }
        public void UseMana(float amount)
        {
            HealthInfo before = GetCurrentHealthInfo();
            mCurrentMana -= amount;
            mCurrentMana.ExSetMinimum(0);
            HealthInfo after = GetCurrentHealthInfo();
            InvokeHealthChanged(before, after);
        }
        public void RestoreMana(float amount)
        {
            if (IsDead || amount <= 0) return;
            HealthInfo before = GetCurrentHealthInfo();
            mCurrentMana += amount;
            mCurrentMana.ExSetMaximum(mMaxCurrentMana);
            HealthInfo after = GetCurrentHealthInfo();
            InvokeHealthChanged(before, after);
        }
        public void RestoreShield(float amount)
        {
            if (IsDead || amount <= 0) return;
            HealthInfo before = GetCurrentHealthInfo();
            mCurrentShield += amount;
            mCurrentShield.ExSetMaximum(mMaxCurrentShield);
            HealthInfo after = GetCurrentHealthInfo();
            InvokeHealthChanged(before, after);
        }

        void InvokeHealthChanged(HealthInfo before, HealthInfo after)
        {
            _OnChanged.Invoke(before, after);
            OnChanged?.Invoke(before, after);
        }

        void InvokeHealthDied(HealthInfo before, HealthInfo after)
        {
            _OnDied.Invoke(before, after);
            OnDied?.Invoke(before, after);
        }

        public HealthInfo GetCurrentHealthInfo()
        {
            return new HealthInfo(
                mMaxCurrentHP,
                mMaxCurrentMana,
                mMaxCurrentShield,
                mCurrentHP,
                mCurrentMana,
                mCurrentShield);
        }
    }

    public struct HealthInfo
    {
        public int MaxHealth;
        public int MaxMana;
        public int MaxShield;
        public float CurrentHP;
        public float CurrentMana;
        public float CurrentShield;

        public HealthInfo(int maxHealth, int maxMana, int maxShield, float currentHP, float currentMana, float currentShield)
        {
            MaxHealth = maxHealth;
            MaxMana = maxMana;
            MaxShield = maxShield;
            CurrentHP = currentHP;
            CurrentMana = currentMana;
            CurrentShield = currentShield;
        }

        public bool IsEquals(HealthInfo other)
        {
            return MaxHealth == other.MaxHealth &&
                MaxMana == other.MaxMana &&
                MaxShield == other.MaxShield &&
                CurrentHP.ExIsEquals(other.CurrentHP) &&
                CurrentMana.ExIsEquals(other.CurrentMana) &&
                CurrentShield.ExIsEquals(other.CurrentShield);
        }
    }
}
