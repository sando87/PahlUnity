using UnityEngine;
using UnityEngine.UI;
using PahlUnity;
using Cysharp.Threading.Tasks;
using System.Collections;

namespace PahlUnity.Demo
{
    public class UIButtonEffect : MonoBehaviour
    {
        [SerializeField] Image _Image = null;
        [SerializeField] AudioClip _SFXonSelect = null;
        [SerializeField] AudioClip _SFXonSubmit = null;

        Color mOriColor = new Color();

        void Start()
        {
            if (_Image == null)
                _Image = GetComponent<Image>();

            mOriColor = _Image.color;

            InputUIButton btn = GetComponent<InputUIButton>();
            if (btn != null)
            {
                btn.EventSelect += OnSelect;
                btn.EventDeselect += OnDeselect;
                btn.EventSubmit += OnSubmit;
            }
        }

        void OnSelect(InputUIButton btn)
        {
            _Image.color = Color.white;
            AudioManager.Instance.PlaySFXClip(_SFXonSelect);
        }

        void OnDeselect(InputUIButton btn)
        {
            _Image.color = mOriColor;
        }

        void OnSubmit(InputUIButton btn)
        {
            AudioManager.Instance.PlaySFXClip(_SFXonSubmit);
            // RectTransform rectTransform = GetComponent<RectTransform>();
            // Vector2 prevPos = rectTransform.anchoredPosition;
            // Color prevColor = _Image.color;
            // _Image.color = Color.gray;
            // rectTransform.anchoredPosition = new Vector2(prevPos.x + 3, prevPos.y - 3);
            // this.ExDelayedCoroutineUnSacled(0.15f, () =>
            // {
            //     _Image.color = prevColor;
            //     rectTransform.anchoredPosition = prevPos;
            // });
        }
    }
}

