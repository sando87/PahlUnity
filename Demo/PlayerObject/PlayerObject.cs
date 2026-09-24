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

            mInven = new Inventory(20);

            mEquip = mBaseObj.GetComp<EquipmentMono>();
        }

        void Start()
        {
            InputManager.Instance.SetHandlerInput(mBaseObj.Input);

            Dictionary<EquipmentSlotType, int> slotMaxCounts = new()
            {
                { (int)EquipSlotType.Weapon, 2 },
                { (int)EquipSlotType.Accessory, 3 },
            };
            mEquip.Init(slotMaxCounts);

            Init();

            InitSpec();

            mBaseObj.Health.SetMaxStats(mBaseObj.Spec[SpecFields.MaxHP], 0, 0, false);

            mItemInteractor = mBaseObj.GetComp<PlayerItemInteractor>();
            mItemInteractor.OnTryPickupItem = OnTryPickupItem;
        }

        public void Init()
        {
            mPlayerInstData = new PlayerInstData(_PlayerSpecData);

            InGamePlayingData saveData = SaveManager<InGamePlayingData>.Instance.SaveData;
            saveData.Characters.TryGetValue(mPlayerInstData.InstanceID, out mPlayerSaveData);

            mBaseObj.GetComp<PlayerGrowth>().Init(mPlayerSaveData.PlayerStat);

            InitItems();

            InitSpec();
        }

        void InitItems()
        {
            foreach (ItemSaveData saveData in mPlayerSaveData.Items.Values)
            {
                ItemSpecData specData = TableDataContainer<ItemSpecData>.Instance.GetInfo(saveData.ResourceID);
                ItemInstInfo instData = new ItemInstInfo(specData, saveData.InstanceID);

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
            return false;
        }

    }
}
