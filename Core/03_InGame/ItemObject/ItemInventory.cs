using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PahlUnity
{
    public class ItemInventory : MonoBehaviour
    {
        private UserSaveData mUserSaveData = null;
        private CharacterSaveData mCharacterSaveData = null;
        private Dictionary<string, ItemSaveData> mSaveData = null;

        private Dictionary<string, ItemInfo> mInvenItems = new Dictionary<string, ItemInfo>();

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

        bool ShowInvenItems() { return Application.isPlaying && mSaveData != null && mInvenItems.Count > 0; }

        public void LoadItemsFromData(int characterID)
        {
            // mUserSaveData = SaveFileManager<UserSaveData>.Load();
            mCharacterSaveData = mUserSaveData.Characters[characterID];
            mSaveData = mCharacterSaveData.Items;
            foreach (var pair in mSaveData)
            {
                ItemSaveData itemSaveData = pair.Value;
                ItemInfo item = new ItemInfo();
                item.LoadItem(itemSaveData);

                if (!item.IsEquipped)
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
            GameSystem.DoSave_UserSaveData();
        }
        public ItemInfo GetItem(string itemInstID)
        {
            if (mInvenItems.ContainsKey(itemInstID))
                return mInvenItems[itemInstID];
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
    }
}