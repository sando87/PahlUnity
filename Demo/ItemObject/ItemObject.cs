using System;
using UnityEngine;
using DG.Tweening;
using System.Collections;

namespace PahlUnity.Demo
{
    public class ItemObject : MonoBehaviour
    {
        [SerializeField] AudioClip _SFXonDropStart = null;
        [SerializeField] AudioClip _SFXonDropEnd = null;
        [SerializeField] AudioClip _SFXonEquip = null;

        public ItemInstInfo ItemInstData { get; private set; }

        private Transform mRenderTr = null;
        private Vector3 mRotAxis = Vector3.zero;
        private bool mIsDropped = false;

        public void Init(ItemInstInfo itemInstData)
        {
            ItemInstData = itemInstData;
        }

        public virtual void OnDrop()
        {
            mRenderTr = this.ExGetBase().Render.transform;

            mIsDropped = true;

            Vector3 rotDir = MyUtils.Random(Vector3.zero, 1);
            rotDir.y = 0;
            float rotateSpeed = 1000;
            mRotAxis = rotDir.normalized * rotateSpeed;

            DoDropEffect();
        }

        public virtual void OnEuip(BaseObject owner)
        {
            StopAllCoroutines();

            if (mIsDropped)
                AudioManager.Instance.PlaySFXClip(_SFXonEquip);
        }
        public virtual void OnDump(BaseObject owner)
        {
        }

        public void DoDropEffect()
        {
            mRenderTr.DOKill();
            mRenderTr.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            StopCoroutine(nameof(DoDropMovement));
            StartCoroutine(nameof(DoDropMovement));
        }

        IEnumerator DoDropMovement()
        {
            AudioManager.Instance.PlaySFXClip(_SFXonDropStart);

            float duration = 1f;
            float halfDuration = duration * 0.5f;
            mRenderTr.DORotate(mRotAxis, duration, RotateMode.FastBeyond360).From(Vector3.zero).SetEase(Ease.Linear);

            mRenderTr.DOLocalMoveY(5, halfDuration).SetEase(Ease.OutQuad);
            yield return newWaitForSeconds.Cache(halfDuration);
            mRenderTr.DOLocalMoveY(0.2f, halfDuration).SetEase(Ease.InQuad);
            yield return newWaitForSeconds.Cache(halfDuration * 0.8f);
            AudioManager.Instance.PlaySFXClip(_SFXonDropEnd);
            yield return newWaitForSeconds.Cache(halfDuration * 0.2f);

            mRenderTr.DOKill();
            mRenderTr.localRotation = Quaternion.identity;
            mRenderTr.DOLocalMoveY(0.5f, 2f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
        }
    }
}
