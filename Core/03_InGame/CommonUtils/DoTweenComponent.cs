using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

namespace PahlUnity
{
    public class DoTweenComponent : MonoBehaviour
    {
        public enum TweenType
        {
            Move, MoveX, MoveY, MoveZ,
            MoveLocal, MoveLocalX, MoveLocalY, MoveLocalZ,
            Rotate, RotateLocal,
            Scale, ScaleX, ScaleY, ScaleZ,
        }

        [Header("Base")]
        [SerializeField] private bool _StartOnEnable = true;
        [SerializeField] private TweenType _TweenType;

        [Header("Values")]
        [SerializeField] private Vector3 _EndVector;
        [SerializeField] private float _EndFloat;
        [SerializeField] private float _Duration = 0.5f;

        [Header("Options")]
        [SerializeField] private float _Delay = 0f;
        [SerializeField] private Ease _Ease = Ease.OutQuad;
        [SerializeField] private int _LoopCount = 0;
        [ShowIf(nameof(ShowLoopType))]
        [SerializeField] private LoopType _LoopType = LoopType.Restart;
        bool ShowLoopType() { return _LoopCount != 0; }

        private Tween mTween;
        private Vector3 mStartPosition;
        private Vector3 mStartPositionLocal;
        private Vector3 mStartRotation;
        private Vector3 mStartRotationLocal;
        private Vector3 mStartScale;

        void Start()
        {
            mStartPosition = transform.position;
            mStartPositionLocal = transform.localPosition;
            mStartRotation = transform.eulerAngles;
            mStartRotationLocal = transform.localEulerAngles;
            mStartScale = transform.localScale;
        }

        void OnEnable()
        {
            if (_StartOnEnable)
            {
                PlayTween();
            }
        }

        public void PlayTween()
        {
            KillTween();

            switch (_TweenType)
            {
                case TweenType.Move:
                    mTween = transform.DOMove(_EndVector, _Duration).From(mStartPosition);
                    break;
                case TweenType.MoveX:
                    mTween = transform.DOMoveX(_EndFloat, _Duration).From(mStartPosition.x);
                    break;
                case TweenType.MoveY:
                    mTween = transform.DOMoveY(_EndFloat, _Duration).From(mStartPosition.y);
                    break;
                case TweenType.MoveZ:
                    mTween = transform.DOMoveZ(_EndFloat, _Duration).From(mStartPosition.z);
                    break;

                case TweenType.MoveLocal:
                    mTween = transform.DOLocalMove(_EndVector, _Duration).From(mStartPositionLocal);
                    break;
                case TweenType.MoveLocalX:
                    mTween = transform.DOLocalMoveX(_EndFloat, _Duration).From(mStartPositionLocal.x);
                    break;
                case TweenType.MoveLocalY:
                    mTween = transform.DOLocalMoveY(_EndFloat, _Duration).From(mStartPositionLocal.y);
                    break;
                case TweenType.MoveLocalZ:
                    mTween = transform.DOLocalMoveZ(_EndFloat, _Duration).From(mStartPositionLocal.z);
                    break;

                case TweenType.Rotate:
                    mTween = transform.DORotate(_EndVector, _Duration, RotateMode.FastBeyond360).From(mStartRotation);
                    break;
                case TweenType.RotateLocal:
                    mTween = transform.DOLocalRotate(_EndVector, _Duration, RotateMode.FastBeyond360).From(mStartRotationLocal);
                    break;

                case TweenType.Scale:
                    mTween = transform.DOScale(_EndVector, _Duration).From(mStartScale);
                    break;
                case TweenType.ScaleX:
                    mTween = transform.DOScaleX(_EndFloat, _Duration).From(mStartScale.x);
                    break;
                case TweenType.ScaleY:
                    mTween = transform.DOScaleY(_EndFloat, _Duration).From(mStartScale.y);
                    break;
                case TweenType.ScaleZ:
                    mTween = transform.DOScaleZ(_EndFloat, _Duration).From(mStartScale.z);
                    break;
            }

            ApplyOptions();
        }

        private void ApplyOptions()
        {
            if (mTween == null) return;

            mTween.SetEase(_Ease);

            if (_Delay > 0)
            {
                mTween.SetDelay(_Delay);
            }

            if (_LoopCount != 0)
            {
                mTween.SetLoops(_LoopCount, _LoopType);
            }
        }

        public void KillTween()
        {
            if (mTween != null && mTween.IsActive())
            {
                mTween.Kill();
                mTween = null;
            }
        }

        private void OnDisable()
        {
            KillTween();
        }
    }
}