using System.ComponentModel;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace PahlUnity
{
    public class BillboardRotater : MonoBehaviour
    {
        private Camera mBillboardCamera = null;

        void Awake()
        {
            mBillboardCamera = Camera.main;
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
