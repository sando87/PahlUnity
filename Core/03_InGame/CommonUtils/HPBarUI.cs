using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace PahlUnity
{
    public class HPBarUI : MonoBehaviour
    {
        [SerializeField] Transform _FillAmountBar = null;

        private Camera mBillboardCamera = null;

        void Awake()
        {
            mBillboardCamera = Camera.main;
        }

        void Start()
        {
            gameObject.SetActive(false);
        }

        public void OnDamaged(HealthInfo before, HealthInfo after)
        {
            gameObject.SetActive(true);
            this.ExDelayedCoroutine(5, () => gameObject.SetActive(false));

            float rate = after.CurrentHP / (float)after.MaxHealth;
            SetHealthBarRate(rate);
        }

        public void OnDied()
        {
            SetHealthBarRate(0);
            StopAllCoroutines();
            gameObject.SetActive(false);
        }

        void SetHealthBarRate(float _rate)
        {
            float rate = Mathf.Clamp(_rate, 0, 1);
            _FillAmountBar.transform.localScale = new Vector3(rate, 1, 1);
        }

        void LateUpdate()
        {
            UpdateBillboardRotation();
        }

        void UpdateBillboardRotation()
        {
            if (mBillboardCamera == null)
            {
                mBillboardCamera = Camera.main;
            }

            if (mBillboardCamera == null)
                return;

            transform.rotation = mBillboardCamera.transform.rotation;
        }
    }
}
