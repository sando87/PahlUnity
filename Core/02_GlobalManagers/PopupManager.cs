using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 팝업관련 UI를 띄우거나 팝업 Depth를 처리하는 등 여러 팝업들을 관리한다.
/// SomePopup의 프리팹 오브젝트가 등록되어 있어야 한다.
/// 사용예) UIPopupManager.Inst.Show<SomePopup>();
/// </summary>
namespace PahlUnity
{
    public class PopupManager : SingletonMono<PopupManager>
    {
        [SerializeField] PopupBase[] _PrefabsList = null;
        [SerializeField] Canvas _RootCanvas = null;

        Dictionary<string, PopupBase> mPopupPrefabs = new Dictionary<string, PopupBase>();

        public PopupBase TopPopup { get; private set; } = null;

        void Start()
        {
            foreach (PopupBase prefab in _PrefabsList)
            {
                mPopupPrefabs.Add(prefab.GetType().Name, prefab);
            }
        }

        public async UniTask<T> Open<T>() where T : PopupBase
        {
            string popupKey = typeof(T).Name;
            LOG.errorif(mPopupPrefabs.ContainsKey(popupKey) == false, "popup prefab is not exist");

            PopupBase prefab = mPopupPrefabs[popupKey];
            PopupBase popup = Instantiate(prefab, _RootCanvas.transform);
            if (TopPopup != null)
            {
                TopPopup.OnBackground();
            }
            TopPopup = popup;
            popup.OnForeground();
            await popup.Open();
            return popup as T;
        }

        public async UniTask CloseTopPopup()
        {
            if (TopPopup == null)
                return;

            PopupBase oldPopup = TopPopup;
            int index = TopPopup.transform.GetSiblingIndex();
            TopPopup = index > 0 ? _RootCanvas.transform.GetChild(index - 1).GetComponent<PopupBase>() : null;
            if (TopPopup != null)
            {
                TopPopup.OnForeground();
            }

            await oldPopup.Close();
            Destroy(oldPopup.gameObject);
        }
    }
}