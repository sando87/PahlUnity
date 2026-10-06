using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace PahlUnity.Demo
{
    public class PlayerObject : MonoBehaviour
    {
        [SerializeField] private PlayerSpecData _PlayerSpecData = null;

        PlayerInstData mPlayerInstData;
        PlayerData mPlayerSaveData;

        BaseObject mBaseObj = null;
        PlayerItemInteractor mItemInteractor = null;

        InventoryMono mInven = null;
        EquipmentMono mEquip = null;

        void Awake()
        {
            mBaseObj = this.ExGetBase();

            mInven = mBaseObj.GetComp<InventoryMono>();

            mEquip = mBaseObj.GetComp<EquipmentMono>();
            mItemInteractor = mBaseObj.GetComp<PlayerItemInteractor>();
        }

        void Start()
        {
            InputManager.Instance.SetHandlerInput(mBaseObj.Input);

            InitSaveData();

            InitSpec();

            mBaseObj.Health.SetMaxStats(mBaseObj.Spec[SpecFields.MaxHP], 0, 0, false);

            mInven.OnItemAdded += OnItemAdded;
            mEquip.OnEquippedItem += OnItemEquipped;

            mItemInteractor.OnTryPickupItem = OnTryPickupItem;
        }

        void OnItemAdded(ItemInstInfo item, int count, int positionIndex)
        {
            item.SaveData.IsEquipped = false;
            item.SaveData.PositionIndex = positionIndex;
            mPlayerSaveData.Items[item.InstanceID] = item.SaveData;

            EventManager.Instance.GlobalEvents.InvokeEvent(new SaveUserPlayData(true));
        }

        void OnItemEquipped(ItemInstInfo item, int positionIndex)
        {
            item.SaveData.IsEquipped = true;
            item.SaveData.PositionIndex = positionIndex;
            mPlayerSaveData.Items[item.InstanceID] = item.SaveData;

            EventManager.Instance.GlobalEvents.InvokeEvent(new SaveUserPlayData(true));
        }

        public void DoEquipItem(int invenSlotIndex)
        {
            ItemInstInfo item = mInven.GetItem(invenSlotIndex);
            if (item != null)
            {
                if (mEquip.TryEquip(item))
                {
                    mInven.RemoveItem(item);
                }
            }
        }
        public void DoUnequipItem(EquipSlotType itemSlotType, int invenSlotIndex)
        {
            ItemInstInfo item = mEquip.GetEquipment((int)itemSlotType, invenSlotIndex);
            if (item == null)
                return;

            if (mInven.HasEmptySlot())
            {
                mEquip.Unequip(item);
                mInven.AddItem(item);
            }
        }

        public void InitSaveData()
        {
            mPlayerInstData = new PlayerInstData(_PlayerSpecData);

            InGamePlayingData saveData = SaveManager<InGamePlayingData>.Instance.SaveData;
            mPlayerSaveData = saveData.Data;

            mBaseObj.GetComp<PlayerGrowth>().Init(mPlayerSaveData.PlayerStat);

            InitItems();

            mBaseObj.GetComp<SkillController>().Init(mPlayerSaveData);
        }

        void InitItems()
        {
            foreach (ItemSaveData saveData in mPlayerSaveData.Items.Values)
            {
                ItemSpecData specData = TableDataContainer<ItemSpecData>.Instance.GetInfo(saveData.ResourceID);
                ItemInstInfo instData = new ItemInstInfo(specData, saveData);

                if (saveData.IsEquipped)
                {
                    mEquip.TryEquip(instData, instData.SaveData.PositionIndex);
                }
                else
                {
                    mInven.AddItem(instData, saveData.Count);
                }
            }
        }

        void InitSpec()
        {
            int currentLevel = mBaseObj.GetComp<PlayerGrowth>().CurrentLevel;
            float maxLevel = 99;
            float normalizedRange = currentLevel / maxLevel;
            mBaseObj.Spec.SetSpecs(mPlayerInstData.SpecData.Specs, normalizedRange);

            mBaseObj.Spec.UpdateCurrentValueByStep(SpecFields.MaxHP, mPlayerSaveData.PlayerStat.HealthPoint);
            mBaseObj.Spec.UpdateCurrentValueByStep(SpecFields.MaxMP, mPlayerSaveData.PlayerStat.ManaPoint);
            mBaseObj.Spec.UpdateCurrentValueByStep(SpecFields.Attack, mPlayerSaveData.PlayerStat.AttackPoint);
            mBaseObj.Spec.UpdateCurrentValueByStep(SpecFields.Defense, mPlayerSaveData.PlayerStat.DefensePoint);

            SpecModifier[] modifiers = GetComponentsInChildren<SpecModifier>();
            foreach (var modifier in modifiers)
            {
                mBaseObj.Spec.AddModifier(modifier);
            }
        }


        bool OnTryPickupItem(ItemObject item)
        {
            int addedCount = mInven.AddItem(item.ItemInstData, 1);
            if (addedCount > 0)
            {
                item.OnPickedUp(mBaseObj);
                return true;
            }

            return false;
        }
    }
}
