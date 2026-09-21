using UnityEngine;
using UnityEngine.UI;
using PahlUnity;
using Cysharp.Threading.Tasks;
using System.Collections;
using TMPro;

namespace PahlUnity.Demo
{
    public class SceneUILoading : ScreenUIBase
    {
        [SerializeField] TextMeshProUGUI _LoadingText = null;
        [SerializeField] Image _FillImage = null;

        IEnumerator Start()
        {
            FadeIn(0.5f);

            _LoadingText.text = "Loading.";
            _FillImage.fillAmount = 0;
            yield return newWaitForSeconds.Cache(0.2f);
            _LoadingText.text = "Loading..";
            _FillImage.fillAmount = 0.33f;
            yield return newWaitForSeconds.Cache(0.2f);
            _LoadingText.text = "Loading...";
            _FillImage.fillAmount = 0.66f;
            yield return newWaitForSeconds.Cache(0.2f);
            _LoadingText.text = "Done!!";
            _FillImage.fillAmount = 1;
            yield return newWaitForSeconds.Cache(0.2f);

            FadeOut(0.5f);
            yield return newWaitForSeconds.Cache(0.5f);
            SceneSwitchManager.Instance.ChangeSceneAsync(SceneType.MainTitle).Forget();
        }
    }
}

