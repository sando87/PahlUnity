using System;
using System.Collections.Generic;
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

        Inventory mInven = null;
        EquipmentMono mEquip = null;

        void Awake()
        {
            mBaseObj = this.ExGetBase();

            mInven = mBaseObj.GetComp<Inventory>();

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
            mItemInteractor.OnTryPickupItem = OnTryPickupItem;
        }

        void OnItemAdded(IInvenItem item, int count, int positionIndex)
        {
            ItemInstInfo instData = item as ItemInstInfo;
            instData.SaveData.IsEquipped = false;
            instData.SaveData.PositionIndex = positionIndex;
            mPlayerSaveData.Items.Add(instData.InstanceID, instData.SaveData);
            SaveManager<InGamePlayingData>.Instance.SaveImmediate();
        }

        public void InitSaveData()
        {
            mPlayerInstData = new PlayerInstData(_PlayerSpecData);

            InGamePlayingData saveData = SaveManager<InGamePlayingData>.Instance.SaveData;
            mPlayerSaveData = saveData.Data;

            mBaseObj.GetComp<PlayerGrowth>().Init(mPlayerSaveData.PlayerStat);

            InitItems();
        }

        void InitItems()
        {
            foreach (ItemSaveData saveData in mPlayerSaveData.Items.Values)
            {
                ItemSpecData specData = TableDataContainer<ItemSpecData>.Instance.GetInfo(saveData.ResourceID);
                ItemInstInfo instData = new ItemInstInfo(specData, saveData);

                if (saveData.IsEquipped)
                {
                    mEquip.TryEquip(instData, 0);
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
