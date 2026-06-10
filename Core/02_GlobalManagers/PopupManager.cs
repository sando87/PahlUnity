using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// UI 팝업창들을 관리한다.
/// 팝업창은 스택형태로 쌓이며 화면 내에서 최상위 하나만 활성화 된다.
/// 팝업창이 열릴 때마다 가장 위에 배치되며 Top팝업만 활성화되고 이전 팝업은 비활성화된다.
/// 닫힐 때는 바로 아래에 있는 팝업창이 활성화 된다.
/// 사용예) PopupManager.Instance.Open<SomePopup>();
/// 사용예) PopupManager.Instance.CloseTopPopup();
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

        public async UniTask<T> Open<T>(object param = null) where T : PopupBase
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
            await popup.Open(param);
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