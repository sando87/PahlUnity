using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace PahlUnity
{
    public class PlayerMain : MonoBehaviour
    {
        [SerializeField] int _CharacterID = 1;
        private int mCharacterID = -1;
        public int CharacterID => GetCharacterData();

        [SerializeField]
        [Dropdown(nameof(IDList))]
        string _ResourceID = "";
        List<string> IDList { get => TableDataContainer<CharResourceData>.Instance.GetAllInfo().Select(info => info.CharacterID).ToList(); }

        // private WrapStation mWrapStationAround = null;
        // private EnemyBossBase mSleepingBoss = null;

        public List<ItemObject> ItemsAround { get; private set; } = new List<ItemObject>();

        public Experience Exp { get; private set; }
        public ItemInventory Inven { get; private set; }
        public Equipment Equip { get; private set; }
        public SkillController SkillCtrl { get; private set; }
        public SpecPlayer Spec { get; private set; }

        BaseObject mBaseObj = null;
        UserSaveData mUserSaveData = null;

        private void Awake()
        {
            mBaseObj = GetComponentInParent<BaseObject>();

            Exp = GetComponentInChildren<Experience>();
            Exp.Init(CharacterID);

            Inven = GetComponentInChildren<ItemInventory>();
            Inven.LoadItemsFromData(CharacterID);

            // UserSaveData mUserSaveData = SaveFileManager<UserSaveData>.Load();
            CharacterSaveData mCharacterSaveData = mUserSaveData.Characters[CharacterID];
            ItemSaveData[] mSavedAllItems = mCharacterSaveData.Items.Values.ToArray();

            Equip = GetComponentInChildren<Equipment>();
            Equip.LoadItemsFromData(mSavedAllItems);
            Equip.OnEquipItem += (item) => OnChangeEquipState();
            Equip.OnUnEquipItem += (item) => OnChangeEquipState();

            Spec = GetComponentInChildren<SpecPlayer>();
            Spec.Init(CharacterID, _ResourceID);
            Spec.LinkOption(Equip.TotalItemOption);
            Spec.LinkOption(mBaseObj.Buffs.TotalBuffOption);

            SkillCtrl = GetComponentInChildren<SkillController>();
            SkillCtrl.InitSkills(CharacterID);
        }

        void Start()
        {
            mBaseObj.Interactor.OnInteractEnter.AddListener(OnColliderEnter);
            mBaseObj.Interactor.OnInteractLeave.AddListener(OnColliderLeave);

            // 캐릭터별 처음 생성시 주어지는 초기 시작 아이템 및 스킬 부여
            UserSaveData saveData = null; // SaveFileManager<UserSaveData>.Load();
            CharacterSaveData playerData = saveData.Characters[CharacterID];
            if (playerData.IsFirstPlay)
            {
                playerData.IsFirstPlay = false;

                playerData.LifePotionCount = 3;
                playerData.ManaPotionCount = 3;

                ItemInfo itemInfo = new ItemInfo();
                itemInfo.InitItem("Item10");
                Inven.AddItem(itemInfo);
                Inven.RepairItem(itemInfo.InstanceID);
                Inven.SetEquipableItem(itemInfo.InstanceID);

                Equip.EquipItem(itemInfo);

                SkillCtrl.LearnNewSkill("Skill05");
                SkillCtrl.EquipSkill("Skill05", 0);
                GameSystem.DoSave_UserSaveData();
            }

            OnChangeEquipState();
        }

        void OnColliderEnter(Collider2D col)
        {
            ItemObject itemObj = col.ExGetCompInBase<ItemObject>();
            if (itemObj != null)
            {
                ItemsAround.Add(itemObj);
            }

            // WrapStation wrapStation = col.ExGetCompInBase<WrapStation>();
            // if (wrapStation != null)
            // {
            //     mWrapStationAround = wrapStation;
            // }

            // EnemyBossBase boss = col.ExGetCompInBase<EnemyBossBase>();
            // if (boss != null && !boss.IsAwaked)
            // {
            //     mSleepingBoss = boss;
            // }

        }
        void OnColliderLeave(Collider2D col)
        {
            ItemObject itemObj = col.ExGetCompInBase<ItemObject>();
            if (itemObj != null)
            {
                ItemsAround.Remove(itemObj);
            }

            // WrapStation wrapStation = col.ExGetCompInBase<WrapStation>();
            // if (wrapStation != null)
            // {
            //     mWrapStationAround = null;
            // }

            // EnemyBossBase boss = col.ExGetCompInBase<EnemyBossBase>();
            // if (boss != null)
            // {
            //     mSleepingBoss = null;
            // }
        }

        void Update()
        {
            if (mBaseObj.Input.JustPressed(PlayerUnitInputType.PotionA))
            {
                if (Inven.CurrentLifePotionCount > 0)
                {
                    Inven.CurrentLifePotionCount--;
                    int healHP = (int)(mBaseObj.Health.MaxHealth * 0.7f);
                    mBaseObj.Health.Heal(healHP);
                }
            }

            if (mBaseObj.Input.JustPressed(PlayerUnitInputType.PotionB))
            {
                if (Inven.CurrentManaPotionCount > 0)
                {
                    Inven.CurrentManaPotionCount--;
                    int healMana = (int)(mBaseObj.Health.MaxMana * 0.7f);
                    mBaseObj.Health.RestoreMana(healMana);
                }
            }

            // if (mWrapStationAround != null && mBaseObj.Input.JustPressed(PlayerUnitInputType.Move))
            // {
            //     if (mBaseObj.Input.MoveY < 0)
            //     {
            //         if (mWrapStationAround.DestScene != SceneType.None)
            //             InGameManager.Instance.Engine.DoWarpStation(mWrapStationAround.DestScene, mWrapStationAround.DestWarpID);
            //     }
            // }

            // if (mSleepingBoss != null && mBaseObj.Input.JustPressed(PlayerUnitInputType.Move))
            // {
            //     if (mBaseObj.Input.MoveY < 0)
            //     {
            //         InGameEngine.Inst.ShowInventorySelectMode((selectedItem) =>
            //         {
            //             mSleepingBoss.DoAwakeBossWithItem(selectedItem);
            //             Inven.RemoveItem(selectedItem.InstanceID);
            //             mSleepingBoss = null;
            //         });
            //     }
            // }
        }

        public void UpdateSpecByLevelUp()
        {
            Spec.UpdateBasicStat();
            mBaseObj.Health.UpdateMaxStats(false);
        }
        public void UpdateSpecByPoint()
        {
            Spec.UpdateBasicStat();
            mBaseObj.Health.UpdateMaxStats(true);
        }
        public void OnChangeEquipState()
        {
            float attackSpeed = mBaseObj.PlayerObj.Spec.Option.AttackSpeedUp.Multiplier;
            mBaseObj.AnimHelper.SetParamFloat(AnimatorParams.AttackSpeed, attackSpeed);

            float moveSpeed = mBaseObj.PlayerObj.Spec.Option.MoveSpeedUp.Multiplier;
            mBaseObj.AnimHelper.SetParamFloat(AnimatorParams.MoveSpeed, moveSpeed);

            mBaseObj.Health.UpdateMaxStats(true);
        }

        int GetCharacterData()
        {
            if (mCharacterID >= 0)
            {
                return mCharacterID;
            }

            // UserSaveData userData = SaveFileManager<UserSaveData>.Load();
            // if (userData.Characters.ContainsKey(_CharacterID))
            // {
            //     mCharacterID = _CharacterID;
            // }
            // else
            // {
            //     userData.Characters[_CharacterID] = new CharacterSaveData();
            //     mCharacterID = _CharacterID;
            //     SaveFileManager<UserSaveData>.Save(userData);
            // }

            return mCharacterID;
        }
    }
}