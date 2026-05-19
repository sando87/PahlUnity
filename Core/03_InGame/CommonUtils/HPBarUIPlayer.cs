using System;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace PahlUnity
{
    public class HPBarUIPlayer : MonoBehaviour
    {
        [SerializeField] Transform _FillAmountLife = null;
        [SerializeField] Transform _FillAmountMana = null;
        [SerializeField] Transform _FillAmountShield = null;

        private Health mHealthStatus = null;
        private HealthInfo mPreviousState = new HealthInfo();

        public void SetHealthStatus(Health health)
        {
            mHealthStatus = health;
            mPreviousState = new HealthInfo();
        }

        void Update()
        {
            transform.rotation = Quaternion.identity;
            UpdatePlayerHealth();
        }

        void UpdatePlayerHealth()
        {
            if (mHealthStatus == null)
                return;

            HealthInfo curInfo = mHealthStatus.GetCurrentHealthInfo();
            if (!mPreviousState.IsEquals(curInfo))
            {
                float lifeRate = (float)mHealthStatus.CurrentHP / mHealthStatus.MaxHealth;
                SetLifeRate(lifeRate);

                float manaRate = (float)mHealthStatus.CurrentMana / mHealthStatus.MaxMana;
                SetManaRate(manaRate);

                float shieldRate = (float)mHealthStatus.CurrentShield / mHealthStatus.MaxShield;
                SetShieldRate(shieldRate);

                mPreviousState = curInfo;
            }
        }

        void SetLifeRate(float _rate)
        {
            if (_FillAmountLife == null) return;
            float rate = Mathf.Clamp(_rate, 0, 1);
            _FillAmountLife.transform.localScale = new Vector3(rate, 1, 1);
        }
        void SetManaRate(float _rate)
        {
            if (_FillAmountMana == null) return;
            float rate = Mathf.Clamp(_rate, 0, 1);
            _FillAmountMana.transform.localScale = new Vector3(rate, 1, 1);
        }
        void SetShieldRate(float _rate)
        {
            if (_FillAmountShield == null) return;
            float rate = Mathf.Clamp(_rate, 0, 1);
            _FillAmountShield.transform.localScale = new Vector3(rate, 1, 1);
        }
    }
}
