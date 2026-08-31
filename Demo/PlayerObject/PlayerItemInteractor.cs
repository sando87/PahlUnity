using UnityEngine;
using System.Collections.Generic;

namespace PahlUnity.Demo
{
    /// <summary>
    /// 주변에 아이템이 있는지 확인하고 관리한다.
    /// 아이템을 상호작용 할 수 있는 컴포넌트
    /// </summary>
    public class PlayerItemInteractor : MonoBehaviour
    {
        public event System.Action<ItemObject> OnDetectItem;
        public System.Func<ItemObject, bool> OnTryPickupItem { get; set; } = null;

        private List<ItemObject> mAroundItems = new List<ItemObject>();
        private PlayerObject mPlayerBase = null;
        private InputPlayer mPlayerInput = null;

        public ItemObject NearestItem { get; private set; } = null;

        void Awake()
        {
            mPlayerBase = this.ExGetCompInBase<PlayerObject>();
            mPlayerInput = this.ExGetCompInBase<InputPlayer>();

            InteractableCollider interactableCollider = GetComponent<InteractableCollider>();
            interactableCollider.OnInteractEnter3D += OnInteractEnter3D;
            interactableCollider.OnInteractLeave3D += OnInteractLeave3D;
        }


        void Update()
        {
            NearestItem = GetNearestItem();

            if (mPlayerInput.JustPressed(InputActionNameHash.Interact))
            {
                if (NearestItem != null)
                {
                    ItemObject item = NearestItem;
                    int curPlayerLevel = mPlayerBase.ExGetCompInBase<PlayerGrowth>().CurrentLevel;
                    int itemLevel = item.ItemInstData.Level;
                    if (itemLevel > curPlayerLevel)
                    {
                        item.DoDropEffect();
                    }
                    else
                    {
                        bool bSuccess = OnTryPickupItem(item);
                        if (bSuccess)
                        {
                            mAroundItems.Remove(item);
                            NearestItem = null;
                            item.ExGetBase().DestroyObj();
                        }
                    }
                }
            }
        }

        void OnInteractEnter3D(Collider col)
        {
            ItemObject item = col.ExGetCompInBase<ItemObject>();
            if (item == null)
                return;

            mAroundItems.Add(item);
            OnDetectItem?.Invoke(item);
        }

        void OnInteractLeave3D(Collider col)
        {
            ItemObject item = col.ExGetCompInBase<ItemObject>();
            if (item == null)
                return;

            mAroundItems.Remove(item);
        }

        private ItemObject GetNearestItem()
        {
            for (int i = mAroundItems.Count - 1; i >= 0; i--)
            {
                if (mAroundItems[i] == null || !mAroundItems[i].gameObject.activeInHierarchy)
                {
                    mAroundItems.RemoveAt(i);
                }
            }

            if (mAroundItems.Count == 0)
                return null;

            ItemObject nearestItem = null;
            float nearestSqrDistance = float.MaxValue;
            Vector3 currentPosition = transform.position;
            for (int i = 0; i < mAroundItems.Count; i++)
            {
                ItemObject item = mAroundItems[i];
                float sqrDistance = (item.transform.position - currentPosition).sqrMagnitude;
                if (sqrDistance < nearestSqrDistance)
                {
                    nearestSqrDistance = sqrDistance;
                    nearestItem = item;
                }
            }

            return nearestItem;
        }
    }
}