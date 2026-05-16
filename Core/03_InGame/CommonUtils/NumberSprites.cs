using DG.Tweening;
using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class NumberSprites : MonoBehaviour
    {
        [SerializeField] Sprite[] NumberImages;
        [SerializeField] Sprite[] NumberOutlineImages;
        [SerializeField] SpriteRenderer First;
        [SerializeField] SpriteRenderer FirstOutline;
        [SerializeField] SpriteRenderer Second;
        [SerializeField] SpriteRenderer SecondOutline;
        [SerializeField] SpriteRenderer Third;
        [SerializeField] SpriteRenderer ThirdOutline;

        [SerializeField] Color _NumberColor = Color.white;
        [SerializeField] Color _OutlineColor = Color.black;
        [SerializeField] bool _Outline = true;
        [SerializeField] float _Scale = 0.5f;
        [SerializeField] float _Gap = 0.1f;
        [SerializeField] float _NumberWorldWidth = 0.6f;

        public void SetNumber(int _number)
        {
            int number = _number < 0 ? 0 : (_number > 999 ? 999 : _number);

            if (number < 10)
            {
                First.sprite = NumberImages[number];
                First.color = _NumberColor;
                FirstOutline.sprite = NumberOutlineImages[number];
                FirstOutline.color = _OutlineColor;
                First.transform.position = transform.position;

                First.gameObject.SetActive(true);
                Second.gameObject.SetActive(false);
                Third.gameObject.SetActive(false);
                FirstOutline.gameObject.SetActive(_Outline);
                SecondOutline.gameObject.SetActive(false);
                ThirdOutline.gameObject.SetActive(false);
            }
            else if (number < 100)
            {
                First.sprite = NumberImages[number % 10];
                First.color = _NumberColor;
                Second.sprite = NumberImages[number / 10];
                Second.color = _NumberColor;

                FirstOutline.sprite = NumberOutlineImages[number % 10];
                FirstOutline.color = _OutlineColor;
                SecondOutline.sprite = NumberOutlineImages[number / 10];
                SecondOutline.color = _OutlineColor;

                float offsetX = (_NumberWorldWidth + _Gap) * 0.5f;
                Vector3 firstCenter = transform.position + new Vector3(offsetX, 0, 0);
                First.transform.position = firstCenter;
                Vector3 secCenter = transform.position + new Vector3(-offsetX, 0, 0);
                Second.transform.position = secCenter;

                First.gameObject.SetActive(true);
                Second.gameObject.SetActive(true);
                Third.gameObject.SetActive(false);
                FirstOutline.gameObject.SetActive(_Outline);
                SecondOutline.gameObject.SetActive(_Outline);
                ThirdOutline.gameObject.SetActive(false);
            }
            else if (number < 1000)
            {
                First.sprite = NumberImages[number % 10];
                First.color = _NumberColor;
                Second.sprite = NumberImages[(number / 10) % 10];
                Second.color = _NumberColor;
                Third.sprite = NumberImages[number / 100];
                Third.color = _NumberColor;

                FirstOutline.sprite = NumberOutlineImages[number % 10];
                FirstOutline.color = _OutlineColor;
                SecondOutline.sprite = NumberOutlineImages[(number / 10) % 10];
                SecondOutline.color = _OutlineColor;
                ThirdOutline.sprite = NumberOutlineImages[number / 100];
                ThirdOutline.color = _OutlineColor;

                float offsetX = _NumberWorldWidth + _Gap;
                Vector3 firstCenter = transform.position + new Vector3(offsetX, 0, 0);
                First.transform.position = firstCenter;
                Second.transform.position = transform.position;
                Vector3 thirdCenter = transform.position + new Vector3(-offsetX, 0, 0);
                Third.transform.position = thirdCenter;

                First.gameObject.SetActive(true);
                Second.gameObject.SetActive(true);
                Third.gameObject.SetActive(true);
                FirstOutline.gameObject.SetActive(_Outline);
                SecondOutline.gameObject.SetActive(_Outline);
                ThirdOutline.gameObject.SetActive(_Outline);
            }

            transform.localScale = new Vector3(_Scale, _Scale, 1);
        }

        public void FadeOut()
        {
            First.DOFade(0, 0.5f);
            FirstOutline.DOFade(0, 0.5f);
            Second.DOFade(0, 0.5f);
            SecondOutline.DOFade(0, 0.5f);
            Third.DOFade(0, 0.5f);
            ThirdOutline.DOFade(0, 0.5f);
        }

    }
}