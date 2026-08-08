using System.ComponentModel;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace PahlUnity
{
    public class HPBarUI : MonoBehaviour
    {
        [SerializeField] Transform _FillAmountBar = null;
        [SerializeField] float _Duration = 5f;

        private Camera mBillboardCamera = null;

        void Awake()
        {
            mBillboardCamera = Camera.main;
        }

        void Start()
        {
            gameObject.SetActive(false);
        }

        public void OnStateChanged(HealthInfo before, HealthInfo after)
        {
            gameObject.SetActive(true);
            if (_Duration > 0)
                this.ExDelayedCoroutine(_Duration, () => gameObject.SetActive(false));

            float rate = after.CurrentHP / (float)after.MaxHealth;
            SetHealthBarRate(rate);
        }

        public void OnDied()
        {
            gameObject.SetActive(true);
            SetHealthBarRate(0);
            _FillAmountBar.gameObject.SetActive(false);

            StopAllCoroutines();
            transform.DOScale(1.5f, 0.1f).SetEase(Ease.OutQuad).SetLoops(2, LoopType.Yoyo);
            // this.ExDelayedCoroutine(1f, () => transform.DOScaleX(0, 1f).SetEase(Ease.InBack));
            this.ExDelayedCoroutine(1f, () => gameObject.SetActive(false));
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
