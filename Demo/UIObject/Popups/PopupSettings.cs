using UnityEngine;
using UnityEngine.UI;
using PahlUnity;
using Cysharp.Threading.Tasks;
using TMPro;
using System.Collections.Generic;

namespace PahlUnity.Demo
{
    public class PopupSettings : PopupBase
    {
        [SerializeField] InputUI _InputUI = null;

        [SerializeField] InputUIButton _BtnClose = null;

        private UniTaskCompletionSource<bool> mCompleteSource = null;
        private bool mIsCompleted = false;

        public override async UniTask Open(object param)
        {
            InputManager.Instance.PushHandlerInput(_InputUI);
            _BtnClose.EventSubmit += OnBtnClose;

            mCompleteSource = new UniTaskCompletionSource<bool>();
            mIsCompleted = false;

            await UniTask.Yield();
        }

        public override async UniTask Close()
        {
            InputManager.Instance.PopHandlerInput();
            await UniTask.Yield();
        }

        public UniTask WaitForComplete()
        {
            return mCompleteSource != null ? mCompleteSource.Task : UniTask.CompletedTask;
        }

        private void OnBtnClose(InputUIButton btn)
        {
            Complete();
        }

        private void Complete()
        {
            if (mIsCompleted)
                return;

            mIsCompleted = true;
            mCompleteSource?.TrySetResult(true);
        }

    }
}
