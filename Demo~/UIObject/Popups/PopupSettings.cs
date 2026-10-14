using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PahlUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PahlUnity.Demo
{
    public class PopupSettings : PopupBase
    {
        [SerializeField] InputUI _InputUI = null;

        [SerializeField] InputUIButton _BtnBGMVolume = null;
        [SerializeField] InputUIButton _BtnVFXVolume = null;
        [SerializeField] InputUIButton _BtnVsync = null;
        [SerializeField] InputUIButton _BtnClose = null;

        private UniTaskCompletionSource mCompleteSource = null;

        public override async UniTask Open(object param)
        {
            InputManager.Instance.PushHandlerInput(_InputUI);

            mCompleteSource = new UniTaskCompletionSource();

            _BtnBGMVolume.EventSubmit += OnBtnBGMVolume;
            _BtnVFXVolume.EventSubmit += OnBtnVFXVolume;
            _BtnVsync.EventSubmit += OnBtnVsync;
            _BtnClose.EventSubmit += OnBtnClose;

            await UniTask.Yield();
        }

        public override async UniTask Close()
        {
            InputManager.Instance.PopHandlerInput();
            await UniTask.Yield();
        }


        private void OnBtnBGMVolume(InputUIButton btn)
        {
            GameSettingInfo.BGMVolume = 0.5f;
            UpdateUIFromData();
            GameSettingInfo.ApplySettingsToSystem();
        }
        private void OnBtnVFXVolume(InputUIButton btn)
        {
            GameSettingInfo.SFXVolume = 0.5f;
            UpdateUIFromData();
            GameSettingInfo.ApplySettingsToSystem();
        }
        private void OnBtnVsync(InputUIButton btn)
        {
            GameSettingInfo.IsVSync = !GameSettingInfo.IsVSync;
            UpdateUIFromData();
            GameSettingInfo.ApplySettingsToSystem();
        }
        private void UpdateUIFromData()
        {
            // Update BGM Volume
            _BtnBGMVolume.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = GameSettingInfo.BGMVolume.ToString("F0");
            // Update SFX Volume
            _BtnVFXVolume.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = GameSettingInfo.SFXVolume.ToString("F0");
            // Update VSync
            _BtnVsync.transform.Find("ToggleOn").gameObject.SetActive(GameSettingInfo.IsVSync);
            _BtnVsync.transform.Find("ToggleOff").gameObject.SetActive(!GameSettingInfo.IsVSync);
        }



        public UniTask WaitForComplete()
        {
            return mCompleteSource.Task;
        }
        private void OnBtnClose(InputUIButton btn)
        {
            Complete();
        }
        private void Complete()
        {
            mCompleteSource?.TrySetResult();
        }

    }
}
