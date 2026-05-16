using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PahlUnity
{
    public class SpriteAnimationEx : MonoBehaviour
    {
        [SerializeField] SpriteRenderer _Renderer = null;

        [Foldout("Intro")][SerializeField] Sprite[] _SpritesIntro = null;
        [Foldout("Intro")][SerializeField] float _IntervalIntro = 0.1f;
        [Foldout("Intro")][SerializeField] UnityEvent _OnEndIntro = null;

        [Foldout("Loop")][SerializeField] Sprite[] _SpritesLoop = null;
        [Foldout("Loop")][SerializeField] float _IntervalLoop = 0.1f;

        [Foldout("Outro")][SerializeField] Sprite[] _SpritesOutro = null;
        [Foldout("Outro")][SerializeField] float _IntervalOutro = 0.1f;
        [Foldout("Outro")][SerializeField] UnityEvent _OnEndOutro = null;

        int mIndex = 0;

        void OnEnable()
        {
            if (_Renderer == null)
                return;

            if (_SpritesIntro != null && _SpritesIntro.Length > 0)
            {
                PlayAnimIntro();
            }
            else
            {
                PlayAnimLoop();
            }
        }

        public void PlayAnimIntro()
        {
            if (!gameObject.activeInHierarchy)
                return;

            StopAllCoroutines();

            mIndex = 0;
            this.ExRepeatCoroutine(
                _IntervalIntro,
                () => _Renderer.sprite = _SpritesIntro[mIndex++ % _SpritesIntro.Length],
                _SpritesIntro.Length,
                () => { _Renderer.sprite = null; _OnEndIntro?.Invoke(); });
        }
        public void PlayAnimLoop()
        {
            if (!gameObject.activeInHierarchy)
                return;

            StopAllCoroutines();

            if (_SpritesLoop != null && _SpritesLoop.Length > 0)
            {
                mIndex = 0;
                this.ExRepeatCoroutine(
                    _IntervalLoop,
                    () => _Renderer.sprite = _SpritesLoop[mIndex++ % _SpritesLoop.Length]);
            }
        }
        public void PlayAnimOutro()
        {
            if (!gameObject.activeInHierarchy)
                return;

            StopAllCoroutines();

            mIndex = 0;
            this.ExRepeatCoroutine(
                _IntervalOutro,
                () => _Renderer.sprite = _SpritesOutro[mIndex++ % _SpritesOutro.Length],
                _SpritesOutro.Length,
                () => { _Renderer.sprite = null; _OnEndOutro?.Invoke(); });
        }

        public void HideAnimation()
        {
            StopAllCoroutines();
            mIndex = 0;
            _Renderer.sprite = null;
        }
    }
}
