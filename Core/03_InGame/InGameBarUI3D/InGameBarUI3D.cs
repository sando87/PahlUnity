using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace PahlUnity
{
    public class InGameBarUI3D : MonoBehaviour
    {
        [SerializeField] Transform _FillAmountBar = null;
        [SerializeField] MeshRenderer _FillAmount = null;
        [SerializeField] Color _Color = Color.white;

        private Camera mBillboardCamera = null;

        void Awake()
        {
            mBillboardCamera = Camera.main;
            _FillAmount.material.color = _Color;
        }

        public void SetBarRate(float _rate)
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
