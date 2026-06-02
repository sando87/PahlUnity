using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace PahlUnity
{
    public class PlayerBase : MonoBehaviour
    {
        [SerializeField]
        [Dropdown(nameof(IDList))]
        string _ResourceID = "";
        List<string> IDList { get => TableDataContainer<CharResourceData>.Instance.GetAllInfo().Select(info => info.CharacterID).ToList(); }

        // private WrapStation mWrapStationAround = null;
        // private EnemyBossBase mSleepingBoss = null;

        public List<ItemBase> ItemsAround { get; private set; } = new List<ItemBase>();

        public PlayerGrowth Exp { get; private set; }
        public Inventory Inven { get; private set; }
        public Equipment Equip { get; private set; }
        public SkillController SkillCtrl { get; private set; }
        public SpecPlayer Spec { get; private set; }

        BaseObject mBaseObj = null;
        AnimatorHelper mAnim = null;
        Health mHealth = null;
        SpecBase mSpec = null;
        CharacterSaveData mCharSaveData = null;

        private void Awake()
        {
            mBaseObj = GetComponentInParent<BaseObject>();
            mAnim = mBaseObj.GetComp<AnimatorHelper>();
            mHealth = mBaseObj.GetComp<Health>();
            mSpec = mBaseObj.GetComp<SpecBase>();

            InGameSaveData allCharacterSaveData = SaveManager<InGameSaveData>.Instance.SaveData;
            mCharSaveData = allCharacterSaveData.Characters[_ResourceID];

            Exp = mBaseObj.GetComp<PlayerGrowth>();
            Exp.Init(mCharSaveData.Stats);

            Inven = new Inventory(20);
            // Inven.LoadItemsFromData(mCharSaveData);

            ItemSaveData[] mSavedAllItems = mCharSaveData.Items.Values.ToArray();

            // Equip = mBaseObj.GetComp<Equipment>();
            // Equip.LoadItemsFromData(mSavedAllItems);
            // Equip.OnEquipItem += (item) => OnChangeEquipState();
            // Equip.OnUnEquipItem += (item) => OnChangeEquipState();

            Spec = mBaseObj.GetComp<SpecPlayer>();
            Spec.Init(mCharSaveData.Stats, _ResourceID);
            // Spec.LinkOption(Equip.TotalItemOption);
            // Spec.LinkOption(mBaseObj.GetComp<BuffController>().TotalBuffOption);

            SkillCtrl = mBaseObj.GetComp<SkillController>();
            SkillCtrl.InitSkills(mCharSaveData);
        }

        void Start()
        {
            mBaseObj.GetComp<InteractableCollider>().OnInteractEnter2D += OnColliderEnter;
            mBaseObj.GetComp<InteractableCollider>().OnInteractLeave2D += OnColliderLeave;

            // 캐릭터별 처음 생성시 주어지는 초기 시작 아이템 및 스킬 부여
            if (mCharSaveData.IsFirstPlay)
            {
                mCharSaveData.IsFirstPlay = false;

                mCharSaveData.LifePotionCount = 3;
                mCharSaveData.ManaPotionCount = 3;

                // ItemInfo itemInfo = new ItemInfo();
                // itemInfo.InitItem("Item10");
                // Inven.AddItem(itemInfo);
                // Inven.RepairItem(itemInfo.InstanceID);
                // Inven.SetEquipableItem(itemInfo.InstanceID);

                // Equip.EquipItem(itemInfo);

                SkillCtrl.LearnNewSkill("Skill05");
                SkillCtrl.EquipSkill("Skill05", 0);
                EventManager.Instance.GlobalEvents.InvokeEvent(new SaveUserPlayData(true));
            }

            OnChangeEquipState();
        }

        void OnColliderEnter(Collider2D col)
        {
            ItemBase itemObj = col.ExGetCompInBase<ItemBase>();
            if (itemObj != null)
            {
                ItemsAround.Add(itemObj);
            }
        }
        void OnColliderLeave(Collider2D col)
        {
            ItemBase itemObj = col.ExGetCompInBase<ItemBase>();
            if (itemObj != null)
            {
                ItemsAround.Remove(itemObj);
            }
        }

        public void UpdateSpecByLevelUp()
        {
            Spec.UpdateBasicStat();
            mHealth.SetMaxStats(mSpec.MaxHealth, mSpec.MaxMana, mSpec.MaxShield, false);
        }
        public void UpdateSpecByPoint()
        {
            Spec.UpdateBasicStat();
            mHealth.SetMaxStats(mSpec.MaxHealth, mSpec.MaxMana, mSpec.MaxShield, true);
        }
        public void OnChangeEquipState()
        {
            // float attackSpeed = Spec.Option.AttackSpeedUp.Multiplier;
            // mAnim.SetParamFloat(AnimatorParams.AttackSpeed, attackSpeed);

            // float moveSpeed = Spec.Option.MoveSpeedUp.Multiplier;
            // mAnim.SetParamFloat(AnimatorParams.MoveSpeed, moveSpeed);

            mHealth.SetMaxStats(mSpec.MaxHealth, mSpec.MaxMana, mSpec.MaxShield, true);
        }
    }
}