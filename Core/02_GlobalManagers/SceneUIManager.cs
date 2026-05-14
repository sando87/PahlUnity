using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 팝업관련 UI를 띄우거나 팝업 Depth를 처리하는 등 여러 팝업들을 관리한다.
/// SomePopup의 프리팹 오브젝트가 등록되어 있어야 한다.
/// 사용예) SceneUIManager.Inst.Show<SomePopup>();
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