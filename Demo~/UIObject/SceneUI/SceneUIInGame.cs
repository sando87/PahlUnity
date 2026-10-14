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
        [SerializeField] private TextMeshProUGUI _LevelText;
        [SerializeField] private Image _LevelBar;
        [SerializeField] private Image _HPBar;
        [SerializeField] private Image _MPBar;
        [SerializeField] private TextMeshProUGUI _GoldText;

        PopupMessageBox mSystemMsgPopup = null;

        void Start()
        {
            FadeIn(0.5f);
        }

        void Update()
        {
            UpdateUIState();

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

        void UpdateUIState()
        {
            if (InGameManager.Instance.Engine == null)
                return;
            if (InGameManager.Instance.Engine.Player == null)
                return;

            Health playerHealth = InGameManager.Instance.Engine.Player.ExGetCompInBase<Health>();
            PlayerGrowth playerGrowth = InGameManager.Instance.Engine.Player.ExGetCompInBase<PlayerGrowth>();
            if (playerHealth == null || playerGrowth == null)
                return;

            _LevelText.text = $"Lv.{playerGrowth.CurrentLevel}";
            _LevelBar.fillAmount = playerGrowth.CurrentExpRate;
            _HPBar.fillAmount = playerHealth.HpRate;
            _MPBar.fillAmount = playerHealth.ManaRate;
            // _GoldText.text = InGameManager.Instance.Engine.Player.Gold.ToString();
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

