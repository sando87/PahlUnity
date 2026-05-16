using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class ItemInventory : MonoBehaviour
    {
        private Dictionary<string, ItemInfo> mInvenItems = new Dictionary<string, ItemInfo>();
        private Dictionary<string, ItemInfo> mEquipItems = new Dictionary<string, ItemInfo>();
        private Dictionary<string, ItemSaveData> mSaveData = null;
        private UserSaveData mUserSaveData = null;
        private CharacterSaveData mCharacterSaveData = null;

        public SpecOption TotalItemOption
        { get; private set; } = new SpecOption();

        public int CurrentGold { get { return mUserSaveData.Gold; } set { mUserSaveData.Gold = value; GameSystem.RequestSave(); } }
        public int CurrentLifePotionCount
        {
            get { return mCharacterSaveData.LifePotionCount; }
            set { mCharacterSaveData.LifePotionCount = value; GameSystem.RequestSave(); }
        }
        public int CurrentManaPotionCount
        {
            get { return mCharacterSaveData.ManaPotionCount; }
            set { mCharacterSaveData.ManaPotionCount = value; GameSystem.RequestSave(); }
        }

        [ShowIf(nameof(ShowInvenItems))]
        [Dropdown(nameof(ListInvenItems))]
        [OnValueChanged(nameof(SelectInvenItem))]
        public ItemInfo mSelectedInvenItem = null;
        public ItemInfo[] ListInvenItems() { return mInvenItems.Values.ToArray(); }

        [SerializeField]
        [ShowIf(nameof(ShowInvenItems))]
        private ItemInfo _SelectInvenItem = null;
        void SelectInvenItem() { _SelectInvenItem = mSelectedInvenItem; _SelectInvenItem._Option = mSelectedInvenItem.Option; }

        [ShowIf(nameof(ShowEquipItems))]
        [Dropdown(nameof(ListEquipItems))]
        [OnValueChanged(nameof(SelectEquipItem))]
        public ItemInfo mSelectEquipItem = null;
        public ItemInfo[] ListEquipItems() { return mEquipItems.Values.ToArray(); }

        [SerializeField]
        [ShowIf(nameof(ShowEquipItems))]
        private ItemInfo _SelectEquipItem = null;
        void SelectEquipItem() { _SelectEquipItem = mSelectEquipItem; _SelectEquipItem._Option = mSelectEquipItem.Option; }


        bool ShowInvenItems() { return Application.isPlaying && mSaveData != null && mInvenItems.Count > 0; }
        bool ShowEquipItems() { return Application.isPlaying && mSaveData != null && mEquipItems.Count > 0; }

        [Button]
        [ShowIf(nameof(ShowInvenItems))]
        void _EquipItem() { EquipItem(mSelectedInvenItem.InstanceID); }

        [Button]
        [ShowIf(nameof(ShowEquipItems))]
        void _UnEquipItem() { UnEquipItem(mSelectEquipItem.InstanceID); }

        public UnityEvent EventEquipItem = new UnityEvent();
        public UnityEvent EventUnEquipItem = new UnityEvent();

        public void LoadItemsFromData(int characterID)
        {
            mUserSaveData = SaveFileManager<UserSaveData>.Load();
            mCharacterSaveData = mUserSaveData.Characters[characterID];
            mSaveData = mCharacterSaveData.Items;
            foreach (var pair in mSaveData)
            {
                ItemSaveData itemSaveData = pair.Value;
                ItemInfo item = new ItemInfo();
                item.LoadItem(itemSaveData);

                if (item.IsEquipped)
                {
                    mEquipItems[itemSaveData.InstanceID] = item;
                    TotalItemOption.Add(item.Option);
                }
                else
                {
                    mInvenItems[itemSaveData.InstanceID] = item;
                }
            }
        }

        public void AddItem(ItemInfo item)
        {
            if (mInvenItems.ContainsKey(item.InstanceID))
            {
                mInvenItems[item.InstanceID].Count += item.Count;
            }
            else
            {
                mInvenItems[item.InstanceID] = item;
                mSaveData[item.InstanceID] = item.SaveData;
            }
            GameSystem.DoSave_UserSaveData();
        }
        public void RemoveItem(string itemInstID)
        {
            LOG.errorif(!mSaveData.ContainsKey(itemInstID));
            mSaveData.Remove(itemInstID);
            if (mInvenItems.ContainsKey(itemInstID))
                mInvenItems.Remove(itemInstID);
            if (mEquipItems.ContainsKey(itemInstID))
                mEquipItems.Remove(itemInstID);
            GameSystem.DoSave_UserSaveData();
        }
        public ItemInfo GetItem(string itemInstID)
        {
            if (mInvenItems.ContainsKey(itemInstID))
                return mInvenItems[itemInstID];
            else if (mEquipItems.ContainsKey(itemInstID))
                return mEquipItems[itemInstID];
            else
                return null;
        }

        public void SubItem(string itemInstID, int count)
        {
            GetItem(itemInstID).Count -= count;
            GameSystem.DoSave_UserSaveData();
        }
        public void UpgradeItem(string itemInstID)
        {
            GetItem(itemInstID).Level++;
            GameSystem.DoSave_UserSaveData();
        }
        public void RepairItem(string itemInstID)
        {
            GetItem(itemInstID).IsRepaired = true;
            GameSystem.DoSave_UserSaveData();
        }
        public void SetEquipableItem(string itemInstID)
        {
            GetItem(itemInstID).IsEquipable = true;
            GameSystem.DoSave_UserSaveData();
        }

        public void MoveItem(string itemInstID, int newPositionIndex)
        {
            GetItem(itemInstID).PositionIndex = newPositionIndex;
            GameSystem.DoSave_UserSaveData();
        }

        public void EquipItem(string itemInstID)
        {
            ItemInfo item = GetItem(itemInstID);
            if (item.IsEquipped)
                return;

            item.IsEquipped = true;
            mInvenItems.Remove(itemInstID);
            mEquipItems.Add(itemInstID, item);
            GameSystem.DoSave_UserSaveData();

            TotalItemOption.Add(item.Option);
            EventEquipItem?.Invoke();
        }

        public void UnEquipItem(string itemInstID)
        {
            ItemInfo item = GetItem(itemInstID);
            if (!item.IsEquipped)
                return;

            item.IsEquipped = false;
            mEquipItems.Remove(itemInstID);
            mInvenItems.Add(itemInstID, item);
            GameSystem.DoSave_UserSaveData();

            TotalItemOption.Subtract(item.Option);
            EventUnEquipItem?.Invoke();
        }

    }
}