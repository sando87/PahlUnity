using System;
using System.Collections.Generic;
using UnityEngine;

namespace PahlUnity.Demo
{
    public class PlayerObject : MonoBehaviour
    {
        PlayerInstData mPlayerInstData;
        PlayerData mPlayerSaveData;

        BaseObject mBaseObj = null;

        SpecBase mSpecBase = null;

        PlayerGrowth mExp = null;
        Inventory mInven = null;
        Equipment mEquip = null;

        void Awake()
        {
            mBaseObj = GetComponentInParent<BaseObject>();

            mSpecBase = mBaseObj.GetComp<SpecBase>();

            mExp = mBaseObj.GetComp<PlayerGrowth>();
            mInven = new Inventory(20);

            const int EquipSlotTypeWeapon = 0;
            const int EquipSlotTypeAccessory = 1;
            Dictionary<EquipmentSlotType, int> slotMaxCounts = new()
            {
                { EquipSlotTypeWeapon, 2 },
                { EquipSlotTypeAccessory, 3 },
            };
            mEquip = new Equipment(slotMaxCounts);
        }

        public void Init(PlayerInstData instData)
        {
            mPlayerInstData = instData;

            InGamePlayingData saveData = SaveManager<InGamePlayingData>.Instance.SaveData;
            saveData.Characters.TryGetValue(mPlayerInstData.InstanceID, out mPlayerSaveData);

            mExp.Init(mPlayerSaveData.PlayerStat);

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
                    mEquip.Equip(instData, 0);
                }
                else
                {
                    mInven.AddItem(instData, saveData.Count);
                }
            }
        }

        void InitSpec()
        {
            mSpecBase = GetComponent<SpecBase>();

            int currentLevel = mExp.CurrentLevel;
            float maxLevel = 99;
            float normalizedRange = currentLevel / maxLevel;
            mSpecBase.AddSpecs(mPlayerInstData.SpecData.Specs, normalizedRange);

            mSpecBase.UpdateCurrentValueByStep(SpecFields.MaxHP, mPlayerSaveData.PlayerStat.HealthPoint);
            mSpecBase.UpdateCurrentValueByStep(SpecFields.MaxMP, mPlayerSaveData.PlayerStat.ManaPoint);
            mSpecBase.UpdateCurrentValueByStep(SpecFields.Attack, mPlayerSaveData.PlayerStat.AttackPoint);
            mSpecBase.UpdateCurrentValueByStep(SpecFields.Defense, mPlayerSaveData.PlayerStat.DefensePoint);

            SpecModifier[] modifiers = GetComponentsInChildren<SpecModifier>();
            foreach (var modifier in modifiers)
            {
                mSpecBase.AddModifier(modifier);
            }
        }

    }
}
