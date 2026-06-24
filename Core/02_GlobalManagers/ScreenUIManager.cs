using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 씬마다 기본으로 가장 BaseLayer에 배치되는 UI들을 관리한다.
/// 한번에 두개 이상의 ScreenUI가 존재할 수 없다.(ScreenUI는 스위칭만 가능)
/// 사용예) ScreenUIManager.Instance.Switch<SomeScreenUI>();
/// 사용예) ScreenUIManager.Instance.SwitchCross<SomeScreenUI>();
/// </summary>
namespace PahlUnity
{
    public class ScreenUIManager : SingletonMono<ScreenUIManager>
    {
        [SerializeField] ScreenUIBase[] _PrefabsList = null;
        [SerializeField] Canvas _RootCanvas = null;

        Dictionary<string, ScreenUIBase> mScreenUIPrefabs = new Dictionary<string, ScreenUIBase>();

        public ScreenUIBase CurrentScreenUI { get; private set; } = null;

        void Start()
        {
            foreach (ScreenUIBase prefab in _PrefabsList)
            {
                mScreenUIPrefabs.Add(prefab.GetType().Name, prefab);
            }

            CurrentScreenUI = GetComponentInChildren<ScreenUIBase>();
        }

        // 씬 전환이 순차적으로 이루어짐(현재UI 닫히는 연출이 끝나면 다음UI 열리는 연출이 시작됨)
        public async UniTask Switch<T>() where T : ScreenUIBase
        {
            string key = typeof(T).Name;
            LOG.errorif(mScreenUIPrefabs.ContainsKey(key) == false, "prefab is not exist");

            await ClosePreviousScreenUI();

            await OpenNextScreenUI(key);
        }

        // 씬 전환이 동시에 이루어짐(현재UI 닫히는 연출과 다음UI 열리는 연출이 동시에 시작됨)
        public async UniTask SwitchCross<T>() where T : ScreenUIBase
        {
            string key = typeof(T).Name;
            LOG.errorif(mScreenUIPrefabs.ContainsKey(key) == false, "prefab is not exist");

            ClosePreviousScreenUI().Forget();

            await OpenNextScreenUI(key);
        }

        async UniTask ClosePreviousScreenUI()
        {
            if (CurrentScreenUI != null)
            {
                await CurrentScreenUI.Close();
                Destroy(CurrentScreenUI.gameObject);
                CurrentScreenUI = null;
            }
        }
        async UniTask OpenNextScreenUI(string key)
        {
            ScreenUIBase prefab = mScreenUIPrefabs[key];
            CurrentScreenUI = Instantiate(prefab, _RootCanvas.transform);
            await CurrentScreenUI.Open();
        }
    }
}