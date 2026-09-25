using Cysharp.Threading.Tasks;
using DG.Tweening;
using PahlUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PahlUnity.Demo
{
    public class SceneUIInGame : ScreenUIBase
    {
        PopupMessageBox mSystemMsgPopup = null;

        void Start()
        {
            FadeIn(0.5f);

        }

        void Update()
        {
            if (InputManager.Instance.JustPressed(InputActionNameHash.System))
            {
                if (mSystemMsgPopup != null)
                {
                    mSystemMsgPopup = null;
                    PopupManager.Instance.CloseTopPopup().Forget();
                }
                else
                {
                    ShowSystemMenu().Forget();
                }
            }
        }

        async UniTask ShowSystemMenu()
        {
            MessageBoxParam param = new MessageBoxParam();
            param.Title = "System Menu";
            param.Message = "Are you sure you want to exit the game?";
            param.IsOneButton = false;
            param.ButtonTextA = "Exit";
            param.ButtonTextB = "Cancel";
            mSystemMsgPopup = await PopupManager.Instance.Open<PopupMessageBox>(param);
            bool result = await mSystemMsgPopup.WaitForComplete();
            mSystemMsgPopup = null;
            if (result)
            {
                InputManager.Instance.SetHandlerInput(null);
                FadeOut(0.5f);
                PopupManager.Instance.CloseTopPopup().Forget();
                await UniTask.Delay(500);
                InGameManager.Instance.EndGame();
            }
            else
            {
                await PopupManager.Instance.CloseTopPopup();
            }
        }
    }
}

