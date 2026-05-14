using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 씬마다 기본으로 가장 BaseLayer에 배치되는 UI들을 관리한다.
/// 한번에 두개 이상의 씬UI가 존재할 수 없다.(씬UI는 스위칭만 가능)
/// 씬UI 전환시마다 실제 유니티 씬이 전환된다.
/// 사용예) SceneUIManager.Instance.Switch<SomeSceneUI>();
/// </summary>
namespace PahlUnity
{
    public class SceneUIManager : SingletonMono<SceneUIManager>
    {
        [SerializeField] SceneUIBase[] _PrefabsList = null;
        [SerializeField] Canvas _RootCanvas = null;

        Dictionary<string, SceneUIBase> mSceneUIPrefabs = new Dictionary<string, SceneUIBase>();

        public SceneUIBase CurrentSceneUI { get; private set; } = null;

        void Start()
        {
            foreach (SceneUIBase prefab in _PrefabsList)
            {
                mSceneUIPrefabs.Add(prefab.GetType().Name, prefab);
            }
        }

        public async UniTask Switch<T>() where T : SceneUIBase
        {
            string key = typeof(T).Name;
            LOG.errorif(mSceneUIPrefabs.ContainsKey(key) == false, "prefab is not exist");

            SceneUIBase prefab = mSceneUIPrefabs[key];
            string nextSceneName = prefab.SceneName;
            
            if (CurrentSceneUI != null)
            {
                await CurrentSceneUI.Close();
                Destroy(CurrentSceneUI.gameObject);
                CurrentSceneUI = null;
            }

            await SceneManager.LoadSceneAsync(nextSceneName);
            CurrentSceneUI = Instantiate(prefab, _RootCanvas.transform);
            await CurrentSceneUI.Open();
        }
    }
}