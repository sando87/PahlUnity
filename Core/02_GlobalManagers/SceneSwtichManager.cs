using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 씬전환을 부드럽게 시켜주는 기능(해당 기능은 저사양 장비에서 씬 전환시 화면 끊김 또는 메모리 최적화관련 이슈를 개선하기 위해 설계됨)
/// 첫번째 주요기능
/// - 전환하고자 하는 씬에 초기에 배치된 객체가 많아서 씬 로딩 자체가 느릴때 중간에 로딩씬을 두어서 부드럽게 넘어가도록 설계됨(안그러면 검은화면에서 꽤 오래 멈춘것처럼 보임)
/// 두번째 주요기능
/// - 1번씬에서 2번씬으로 전환시 원래데로라면 1번씬의 리소스와 2번씬의 리소스가 한꺼번에 메모리에 올라가야 할 만큼의 공간이 필요해서 문제가 되었는데
///   이 기능을 사용하면 2번씬 로딩하기 전에 1번씬에서 사용된 리소스를 모두 해제하고 2번씬을 진입하기 때문에 메모리가 그만큼 덜 필요해지게 된다.
/// </summary>

namespace PahlUnity
{
    public class SceneSwtichManager : SingletonMono<SceneSwtichManager>
    {
        [SceneSelector] string _LoadingSceneName = String.Empty;

        public async UniTask ChangeSceneAsync(string nextScene)
        {
            // 다음씬 로딩
            await SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Single);
        }

        public async UniTask ChangeSceneAsyncWithLoading(string nextScene)
        {
            bool isShowLoadingScene = !string.IsNullOrEmpty(_LoadingSceneName);

            if (isShowLoadingScene)
            {
                await SceneManager.LoadSceneAsync(_LoadingSceneName, LoadSceneMode.Additive);
            }

            // 이전씬 제거
            string curSceneName = GetCurrentScene();
            await SceneManager.UnloadSceneAsync(curSceneName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
            await UniTask.Yield();

            // 메모리 정리
            await Resources.UnloadUnusedAssets();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            // 다음씬 로딩
            await SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Additive);

            Scene next = SceneManager.GetSceneByName(nextScene);

            SceneManager.SetActiveScene(next);

            if (isShowLoadingScene)
            {
                await SceneManager.UnloadSceneAsync(_LoadingSceneName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
            }
        }

        public string GetCurrentScene()
        {
            return SceneManager.GetActiveScene().name;
        }
    }

}