using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlUnity
{
    public class InGameEngine : MonoBehaviour
    {
        public static InGameEngine Inst => InGameManager.Instance.Engine;

        [SerializeField] AudioClip _BGM = null;
        [SerializeField] BaseObject _PlayerPrefab = null;
        [SerializeField] InputManager _InputManager = null;
        // [SerializeField] InGamePanel _InGamePanel = null;
        // [SerializeField] ItemSelector _ItemSelector = null;
        // [SerializeField] CinemachineCamera _CinemachineCamera = null;
        [SerializeField] PlayerDepthManager _PlayerDepthManager = null;
        [SerializeField] PlatformerPathfinder _Pathfinder = null;

        public BaseObject Player { get; private set; } = null;
        public PlayerDepthManager DepthManager => _PlayerDepthManager;
        public PlatformerPathfinder Pathfinder => _Pathfinder;

        // PopupStats mPopupStats;
        // PopupInven mPopupInven;
        // PopupSkill mPopupSkill;

        Coroutine mRespawnSequence = null;

        IEnumerator Start()
        {
            AudioManager.Instance.Init();
            yield return null;
            AudioManager.Instance.PlayBGM(_BGM);
            yield return newWaitForSeconds.Cache(0.1f);

            Vector2 destWarp = FindRespawnPosition(InGameManager.Instance.DestWarpID);
            InstantiatePlayer(destWarp);
            // _InGamePanel.DoActivatePanel(Player);
            // _CinemachineCamera.Follow = Player.transform;
            // _CinemachineCamera.ForceCameraPosition(Player.transform.position, Quaternion.identity);
            _PlayerDepthManager.SetPlayer(Player);
            yield return newWaitForSeconds.Cache(0.2f);
            SetInputHandler(Player.Input);
        }

        void Update()
        {
            // if (_InputManager.JustPressed(PlayerUnitInputType.ShowPopupStats))
            // {
            //     mPopupStats = PopupManager.Instance.Open<PopupBase>();
            //     if (mPopupStats != null)
            //     {
            //         SetInputHandler(mPopupStats.InputHandler);
            //     }
            //     else
            //     {
            //         SetInputHandler(Player.Input);
            //     }
            // }
            // if (_InputManager.JustPressed(PlayerUnitInputType.ShowPopupInven))
            // {
            //     mPopupInven = PopupManager.Instance.Open<PopupBase>();
            //     if (mPopupInven != null)
            //     {
            //         SetInputHandler(mPopupInven.InputHandler);
            //         mPopupInven.ItemInven = Player.GetComponentInChildren<ItemInventory>();
            //     }
            //     else
            //     {
            //         SetInputHandler(Player.Input);
            //     }
            // }
            // if (_InputManager.JustPressed(PlayerUnitInputType.ShowPopupSkill))
            // {
            //     mPopupSkill = PopupManager.Instance.Open<PopupBase>();
            //     if (mPopupSkill != null)
            //     {
            //         SetInputHandler(mPopupSkill.InputHandler);
            //     }
            //     else
            //     {
            //         SetInputHandler(Player.Input);
            //     }
            // }

            if (Player != null && Player.Health.IsDead && mRespawnSequence == null)
            {
                mRespawnSequence = StartCoroutine(RespawnPlayerSequence());
            }
        }

        public void InstantiatePlayer(Vector3 pos)
        {
            Player = Instantiate(_PlayerPrefab, pos, Quaternion.identity);
        }

        IEnumerator RespawnPlayerSequence()
        {
            yield return newWaitForSeconds.Cache(2f);
            Destroy(Player.gameObject);
            Vector2 destWarp = FindRespawnPosition(InGameManager.Instance.DestWarpID);
            InstantiatePlayer(destWarp);
            // _InGamePanel.DoActivatePanel(Player);
            // _CinemachineCamera.Follow = Player.transform;
            // _CinemachineCamera.ForceCameraPosition(Player.transform.position, Quaternion.identity);
            _PlayerDepthManager.SetPlayer(Player);
            SetInputHandler(Player.Input);
            mRespawnSequence = null;
        }

        public void SetInputHandler(IInputHandler handler)
        {
            _InputManager.SetHandlerInput(handler);
        }
        public IInputHandler GetInputHandler()
        {
            return _InputManager.GetHandlerInput();
        }

        public void ShowItemSelector(Vector2 worldPos, List<ItemObject> items)
        {
            // Canvas canvas = _ItemSelector.GetComponentInParent<Canvas>();
            // canvas.transform.position = Player.transform.position;
            // _ItemSelector.transform.position = worldPos;
            // _ItemSelector.ShowItemSelector(items);
        }
        public void HideItemSelector()
        {
            // _ItemSelector.HideItemSelector();
        }
        public void MoveItemSelector(bool isUp)
        {
            // if (isUp)
            // {
            //     _ItemSelector.MoveUp();
            // }
            // else
            // {
            //     _ItemSelector.MoveDown();
            // }
        }

        public void DoWarpStation(SceneType destScene, int destWarpID)
        {
            // _InputManager.ClearHandlerInput();
            // InGameManager.Instance.StartScene(destScene, destWarpID);
        }

        Vector2 FindRespawnPosition(int destWarpID)
        {
            // WrapStation[] stations = FindObjectsByType<WrapStation>(FindObjectsInactive.Exclude);
            // if (stations == null || stations.Length == 0)
            //     return Vector2.zero;

            // foreach (WrapStation station in stations)
            // {
            //     if (station.ThisWrapID == destWarpID)
            //     {
            //         return station.transform.position;
            //     }
            // }

            // return stations[0].transform.position;
            return Vector2.zero;
        }

        public void ShowInventorySelectMode(Action<ItemInfo> eventSelectItem)
        {
            // mPopupInven = PopupManager.Instance.Open<PopupBase>();
            // if (mPopupInven != null)
            // {
            //     SetInputHandler(mPopupInven.InputHandler);
            //     mPopupInven.ItemInven = Player.GetComponentInChildren<ItemInventory>();
            //     mPopupInven.SetSelectMode((selectedItem) =>
            //     {
            //         eventSelectItem.Invoke(selectedItem);
            //         PopupManager.Instance.Toggle<PopupInven>();
            //         mPopupInven = null;
            //         SetInputHandler(Player.Input);
            //     });
            // }
        }
    }
}
