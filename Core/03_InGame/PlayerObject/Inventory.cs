using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PahlUnity
{
    public class Inventory : MonoBehaviour
    {
        private CharacterSaveData mCharacterSaveData = null;
        private Dictionary<string, ItemSaveData> mSaveData = null;

        private Dictionary<string, ItemInfo> mInvenItems = new Dictionary<string, ItemInfo>();

        public int CurrentGold
        {
            get { return mCharacterSaveData.Gold; }
            set { mCharacterSaveData.Gold = value; EventManager.Instance.GlobalEvents.InvokeEvent(new SaveUserPlayData(false)); }
        }
        public int CurrentLifePotionCount
        {
            get { return mCharacterSaveData.LifePotionCount; }
            set { mCharacterSaveData.LifePotionCount = value; EventManager.Instance.GlobalEvents.InvokeEvent(new SaveUserPlayData(false)); }
        }
        public int CurrentManaPotionCount
        {
            get { return mCharacterSaveData.ManaPotionCount; }
            set { mCharacterSaveData.ManaPotionCount = value; EventManager.Instance.GlobalEvents.InvokeEvent(new SaveUserPlayData(false)); }
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

        bool ShowInvenItems() { return Application.isPlaying && mInvenItems.Count > 0; }

        public void LoadItemsFromData(CharacterSaveData characterSaveData)
        {
            mCharacterSaveData = characterSaveData;
            mSaveData = characterSaveData.Items;
            foreach (var pair in mCharacterSaveData.Items)
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
            EventManager.Instance.GlobalEvents.InvokeEvent(new SaveUserPlayData(true));
        }
        public void RemoveItem(string itemInstID)
        {
            LOG.errorif(!mSaveData.ContainsKey(itemInstID));
            mSaveData.Remove(itemInstID);
            if (mInvenItems.ContainsKey(itemInstID))
                mInvenItems.Remove(itemInstID);
            EventManager.Instance.GlobalEvents.InvokeEvent(new SaveUserPlayData(true));
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
            EventManager.Instance.GlobalEvents.InvokeEvent(new SaveUserPlayData(true));
        }
        public void UpgradeItem(string itemInstID)
        {
            GetItem(itemInstID).Level++;
            EventManager.Instance.GlobalEvents.InvokeEvent(new SaveUserPlayData(true));
        }
        public void RepairItem(string itemInstID)
        {
            GetItem(itemInstID).IsRepaired = true;
            EventManager.Instance.GlobalEvents.InvokeEvent(new SaveUserPlayData(true));
        }
        public void SetEquipableItem(string itemInstID)
        {
            GetItem(itemInstID).IsEquipable = true;
            EventManager.Instance.GlobalEvents.InvokeEvent(new SaveUserPlayData(true));
        }

        public void MoveItem(string itemInstID, int newPositionIndex)
        {
            GetItem(itemInstID).PositionIndex = newPositionIndex;
            EventManager.Instance.GlobalEvents.InvokeEvent(new SaveUserPlayData(true));
        }
    }
}