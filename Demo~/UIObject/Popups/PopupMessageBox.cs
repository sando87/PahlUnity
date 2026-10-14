using UnityEngine;
using UnityEngine.UI;
using PahlUnity;
using Cysharp.Threading.Tasks;
using TMPro;
using System.Collections.Generic;

namespace PahlUnity.Demo
{
    public class MessageBoxParam
    {
        public string Title;
        public string Message;
        public bool IsOneButton;
        public string ButtonTextA;
        public string ButtonTextB;

    }
    public class PopupMessageBox : PopupBase
    {
        [SerializeField] InputUI _InputUI = null;

        [SerializeField] TextMeshProUGUI _TitleText = null;
        [SerializeField] TextMeshProUGUI _MessageText = null;
        [SerializeField] InputUIButton _BtnYes = null;
        [SerializeField] InputUIButton _BtnNo = null;

        private UniTaskCompletionSource<bool> mCompleteSource = null;

        public override async UniTask Open(object param)
        {
            if (param is MessageBoxParam messageBoxParam)
            {
                _TitleText.text = messageBoxParam.Title;
                _MessageText.text = messageBoxParam.Message;

                _BtnYes.GetComponentInChildren<TextMeshProUGUI>().text = messageBoxParam.ButtonTextA;
                _BtnNo.GetComponentInChildren<TextMeshProUGUI>().text = messageBoxParam.ButtonTextB;

                if (messageBoxParam.IsOneButton)
                {
                    _BtnNo.gameObject.SetActive(false);

                    _BtnYes.EventSubmit += OnBtnYes;

                    // 버튼 위치를 가운데로 조정
                    RectTransform rectTr = _BtnYes.GetComponent<RectTransform>();
                    Vector2 pos = rectTr.anchoredPosition;
                    rectTr.anchoredPosition = new Vector2(0, pos.y);
                }
                else
                {
                    _BtnYes.EventSubmit += OnBtnYes;
                    _BtnNo.EventSubmit += OnBtnNo;
                }
            }

            InputManager.Instance.PushHandlerInput(_InputUI);
            mCompleteSource = new UniTaskCompletionSource<bool>();

            await UniTask.Yield();
        }

        public override async UniTask Close()
        {
            InputManager.Instance.PopHandlerInput();
            await UniTask.Yield();
        }

        public UniTask<bool> WaitForComplete()
        {
            return mCompleteSource.Task;
        }

        private void OnBtnYes(InputUIButton btn)
        {
            Complete(true);
        }
        private void OnBtnNo(InputUIButton btn)
        {
            Complete(false);
        }

        private void Complete(bool isYes)
        {
            mCompleteSource?.TrySetResult(isYes);
        }

    }
}
