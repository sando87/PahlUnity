using UnityEngine;
using UnityEngine.UI;
using PahlUnity;
using Cysharp.Threading.Tasks;
using System.Collections;

namespace PahlUnity.Demo
{
    public class SceneUILogo : ScreenUIBase
    {
        IEnumerator Start()
        {
            FadeIn(0.5f);

            yield return newWaitForSeconds.Cache(1);

            FadeOut(0.5f);
            yield return newWaitForSeconds.Cache(0.5f);
            SceneSwitchManager.Instance.ChangeSceneAsync(SceneType.Loading).Forget();
        }
    }
}

