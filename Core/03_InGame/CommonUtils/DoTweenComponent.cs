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
        [SerializeField] private bool _startOnEnable = true;
        [SerializeField] private TweenType _tweenType;

        [Header("Values")]
        [SerializeField] private Vector3 _endVector;
        [SerializeField] private float _endFloat;
        [SerializeField] private float _duration = 0.5f;

        [Header("Options")]
        [SerializeField] private float _delay = 0f;
        [SerializeField] private Ease _ease = Ease.OutQuad;
        [SerializeField] private int _loopCount = 0;
        [ShowIf(nameof(ShowLoopType))]
        [SerializeField] private LoopType _loopType = LoopType.Restart;
        bool ShowLoopType() { return _loopCount != 0; }

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
            if (_startOnEnable)
            {
                PlayTween();
            }
        }

        public void PlayTween()
        {
            KillTween();

            switch (_tweenType)
            {
                case TweenType.Move:
                    mTween = transform.DOMove(_endVector, _duration).From(mStartPosition);
                    break;
                case TweenType.MoveX:
                    mTween = transform.DOMoveX(_endFloat, _duration).From(mStartPosition.x);
                    break;
                case TweenType.MoveY:
                    mTween = transform.DOMoveY(_endFloat, _duration).From(mStartPosition.y);
                    break;
                case TweenType.MoveZ:
                    mTween = transform.DOMoveZ(_endFloat, _duration).From(mStartPosition.z);
                    break;

                case TweenType.MoveLocal:
                    mTween = transform.DOLocalMove(_endVector, _duration).From(mStartPositionLocal);
                    break;
                case TweenType.MoveLocalX:
                    mTween = transform.DOLocalMoveX(_endFloat, _duration).From(mStartPositionLocal.x);
                    break;
                case TweenType.MoveLocalY:
                    mTween = transform.DOLocalMoveY(_endFloat, _duration).From(mStartPositionLocal.y);
                    break;
                case TweenType.MoveLocalZ:
                    mTween = transform.DOLocalMoveZ(_endFloat, _duration).From(mStartPositionLocal.z);
                    break;

                case TweenType.Rotate:
                    mTween = transform.DORotate(_endVector, _duration, RotateMode.FastBeyond360).From(mStartRotation);
                    break;
                case TweenType.RotateLocal:
                    mTween = transform.DOLocalRotate(_endVector, _duration, RotateMode.FastBeyond360).From(mStartRotationLocal);
                    break;

                case TweenType.Scale:
                    mTween = transform.DOScale(_endVector, _duration).From(mStartScale);
                    break;
                case TweenType.ScaleX:
                    mTween = transform.DOScaleX(_endFloat, _duration).From(mStartScale.x);
                    break;
                case TweenType.ScaleY:
                    mTween = transform.DOScaleY(_endFloat, _duration).From(mStartScale.y);
                    break;
                case TweenType.ScaleZ:
                    mTween = transform.DOScaleZ(_endFloat, _duration).From(mStartScale.z);
                    break;
            }

            ApplyOptions();
        }

        private void ApplyOptions()
        {
            if (mTween == null) return;

            mTween.SetEase(_ease);

            if (_delay > 0)
            {
                mTween.SetDelay(_delay);
            }

            if (_loopCount != 0)
            {
                mTween.SetLoops(_loopCount, _loopType);
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