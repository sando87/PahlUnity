using System.Collections;
using Cysharp.Threading.Tasks;
using PahlUnity;
using UnityEngine;
using UnityEngine.UI;

namespace PahlUnity.Demo
{
    public class SceneUIMainTitle : ScreenUIBase
    {
        [SerializeField] InputUI _InputUI = null;
        [SerializeField] InputUIButton _BtnStartGame = null;
        [SerializeField] InputUIButton _BtnOptions = null;
        [SerializeField] InputUIButton _BtnExit = null;

        void Start()
        {
            InputManager.Instance.SetHandlerInput(_InputUI);

            FadeIn(0.5f);

            _BtnStartGame.EventSubmit += OnBtnStartGame;
            _BtnOptions.EventSubmit += OnBtnOption;
            _BtnExit.EventSubmit += OnBtnExit;
        }

        public void OnBtnStartGame(InputUIButton btn)
        {
            InputManager.Instance.SetHandlerInput(null);
            FadeOut(0.5f);
            this.ExDelayedCoroutine(0.5f, () =>
            {
                InGameManager.Instance.StartGame();
            });
        }
        async public void OnBtnOption(InputUIButton btn)
        {
            PopupSettings popup = await PopupManager.Instance.Open<PopupSettings>();
            await popup.WaitForComplete();
            await PopupManager.Instance.CloseTopPopup();
        }
        public void OnBtnExit(InputUIButton btn)
        {
            if (Application.isEditor)
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
            else
            {
                Application.Quit();
            }
        }
    }
}

