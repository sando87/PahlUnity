using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace PahlBit
{
    public class HPBarUI : MonoBehaviour
    {
        [SerializeField] Transform _FillAmountBar = null;

        void Start()
        {
            gameObject.SetActive(false);
        }

        public void OnDamaged(DamagedResultInfo resultInfo)
        {
            gameObject.SetActive(true);
            this.ExDelayedCoroutine(5, () => gameObject.SetActive(false));

            SetHealthBarRate(resultInfo.CurrentHealthRate);
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

        void Update()
        {
            transform.rotation = Quaternion.identity;
        }
    }
}
