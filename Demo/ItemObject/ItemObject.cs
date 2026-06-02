using System;
using UnityEngine;

namespace PahlUnity.Demo
{
    public class ItemObject : MonoBehaviour
    {
        [SerializeField] private ItemSpecData _SpecData;

        private ItemInstInfo mItemInfo;
        private ItemSaveData mSaveData;

        private SpecBaseMono mSpecBase;

        public void Init(ItemInstInfo itemInfo)
        {
            mItemInfo = itemInfo;

            InGamePlayingData saveData = SaveManager<InGamePlayingData>.Instance.SaveData;
            saveData.Items.TryGetValue(itemInfo.InstanceID, out mSaveData);

            mSpecBase = GetComponent<SpecBaseMono>();

            System.Random random = new System.Random(mItemInfo.RandomSeed);
            mSpecBase.Init(_SpecData.Specs, random);

            mSpecBase.UpdateAllValuesByStep(mSaveData.LevelIndex);
        }

        void Start()
        {
            Debug.Log(mSpecBase[SpecFields.MaxHP]);
            Debug.Log(mSpecBase[SpecFields.MaxMP]);
            Debug.Log(mSpecBase[SpecFields.MoveSpeed]);
            Debug.Log(mSpecBase[SpecFields.AttackSpeed]);
        }
    }
}
