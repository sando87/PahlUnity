using System;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace PahlUnity
{
    public interface IHealth
    {
        public int MaxHealth { get; }
        public int MaxMana { get; }
        public int MaxShield { get; }

        public int CurrentHP { get; }
        public int CurrentMana { get; }
        public int CurrentShield { get; }

        public bool IsDirty { get; set; }
    }

    public class HPBarUIPlayer : MonoBehaviour
    {
        [SerializeField] Transform _FillAmountLife = null;
        [SerializeField] Transform _FillAmountMana = null;

        private IHealth mHealthStatus = null;

        public void SetHealthStatus(IHealth health)
        {
            mHealthStatus = health;
        }

        void Update()
        {
            transform.rotation = Quaternion.identity;
            if (mHealthStatus != null)
            {
                UpdatePlayerHealth();
            }
        }

        void UpdatePlayerHealth()
        {
            if (mHealthStatus == null || !mHealthStatus.IsDirty)
                return;

            float lifeRate = (float)mHealthStatus.CurrentHP / mHealthStatus.MaxHealth;
            SetLifeRate(lifeRate);

            float manaRate = (float)mHealthStatus.CurrentMana / mHealthStatus.MaxMana;
            SetManaRate(manaRate);

            mHealthStatus.IsDirty = false;
        }

        void SetLifeRate(float _rate)
        {
            float rate = Mathf.Clamp(_rate, 0, 1);
            _FillAmountLife.transform.localScale = new Vector3(rate, 1, 1);
        }
        void SetManaRate(float _rate)
        {
            float rate = Mathf.Clamp(_rate, 0, 1);
            _FillAmountMana.transform.localScale = new Vector3(rate, 1, 1);
        }
    }
}
