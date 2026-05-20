using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PahlUnity
{
    public class Equipment : MonoBehaviour
    {
        [SerializeField] UnityEvent<ItemInfo> _OnEquipItem = new UnityEvent<ItemInfo>();
        [SerializeField] UnityEvent<ItemInfo> _OnUnEquipItem = new UnityEvent<ItemInfo>();

        public event UnityAction<ItemInfo> OnEquipItem { add => _OnEquipItem.AddListener(value); remove => _OnEquipItem.RemoveListener(value); }
        public event UnityAction<ItemInfo> OnUnEquipItem { add => _OnUnEquipItem.AddListener(value); remove => _OnUnEquipItem.RemoveListener(value); }

        private Dictionary<string, ItemInfo> mEquipItems = new Dictionary<string, ItemInfo>();

        public SpecOption TotalItemOption { get; private set; } = new SpecOption();

        [ShowIf(nameof(ShowEquipItems))]
        [Dropdown(nameof(ListEquipItems))]
        [OnValueChanged(nameof(SelectEquipItem))]
        public ItemInfo mSelectEquipItem = null;
        public ItemInfo[] ListEquipItems() { return mEquipItems.Values.ToArray(); }

        [SerializeField]
        [ShowIf(nameof(ShowEquipItems))]
        private ItemInfo _SelectEquipItem = null;
        void SelectEquipItem() { _SelectEquipItem = mSelectEquipItem; _SelectEquipItem._Option = mSelectEquipItem.Option; }


        bool ShowEquipItems() { return Application.isPlaying && mEquipItems.Count > 0; }

        [Button]
        [ShowIf(nameof(ShowEquipItems))]
        void _UnEquipItem() { UnEquipItem(mSelectEquipItem); }

        public void LoadItemsFromData(ItemSaveData[] savedAllItemInfo)
        {
            // UserSaveData mUserSaveData = SaveFileManager<UserSaveData>.Load();
            // CharacterSaveData mCharacterSaveData = mUserSaveData.Characters[characterID];
            // var mSaveData = mCharacterSaveData.Items;
            foreach (ItemSaveData savedItemInfo in savedAllItemInfo)
            {
                ItemInfo item = new ItemInfo();
                item.LoadItem(savedItemInfo);

                if (item.IsEquipped)
                {
                    mEquipItems[savedItemInfo.InstanceID] = item;
                    TotalItemOption.Add(item.Option);
                }
            }
        }

        public bool IsEquipItem(ItemInfo item)
        {
            return mEquipItems.ContainsKey(item.InstanceID);
        }

        public void EquipItem(ItemInfo item)
        {
            if (IsEquipItem(item))
                return;

            item.IsEquipped = true;
            mEquipItems.Add(item.InstanceID, item);
            GameSystem.DoSave_UserSaveData();

            TotalItemOption.Add(item.Option);
            _OnEquipItem?.Invoke(item);
        }

        public void UnEquipItem(ItemInfo item)
        {
            if (!IsEquipItem(item))
                return;

            item.IsEquipped = false;
            mEquipItems.Remove(item.InstanceID);
            GameSystem.DoSave_UserSaveData();

            TotalItemOption.Subtract(item.Option);
            _OnUnEquipItem?.Invoke(item);
        }

    }
}