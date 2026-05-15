using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 화면 내의 여러 패널들을 관리한다.
/// 패널은 화면 내에서 여러개가 동시에 존재할 수 있다.
/// 사용예) PanelManager.Instance.Show<SomePanel>();
/// 사용예) PanelManager.Instance.Hide<SomePanel>();
/// </summary>
namespace PahlUnity
{
    public class PanelManager : SingletonMono<PanelManager>
    {
        [SerializeField] PanelBase[] _PrefabsList = null;
        [SerializeField] Canvas _RootCanvas = null;

        Dictionary<string, PanelBase> mPanelObjects = new Dictionary<string, PanelBase>();

        public async UniTask<T> Show<T>() where T : PanelBase
        {
            string panelKey = typeof(T).Name;
            if (mPanelObjects.ContainsKey(panelKey))
            {
                PanelBase panel = mPanelObjects[panelKey];
                await panel.Show();
                return panel as T;
            }
            else
            {
                PanelBase prefab = FindPanelPrefab<T>();
                LOG.errorif(prefab == null, "prefab is not exist");

                PanelBase panel = Instantiate(prefab, _RootCanvas.transform);
                mPanelObjects.Add(panelKey, panel);
                await panel.Show();
                return panel as T;
            }
        }

        public async UniTask Hide<T>() where T : PanelBase
        {
            string panelKey = typeof(T).Name;
            if (mPanelObjects.ContainsKey(panelKey))
            {
                PanelBase panel = mPanelObjects[panelKey];
                await panel.Hide();
            }
        }

        public async UniTask Toggle<T>() where T : PanelBase
        {
            string panelKey = typeof(T).Name;
            if (mPanelObjects.ContainsKey(panelKey) && mPanelObjects[panelKey].IsShowing)
            {
                await Hide<T>();
            }
            else
            {
                await Show<T>();
            }
        }

        PanelBase FindPanelPrefab<T>() where T : PanelBase
        {
            foreach (PanelBase prefab in _PrefabsList)
            {
                if (prefab is T)
                    return prefab;
            }
            return null;
        }
    }
}