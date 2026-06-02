using System;
using UnityEngine;

namespace PahlUnity.Demo
{
    public class ItemObject : MonoBehaviour
    {
        private ItemInstInfo mItemInfo;
        private ItemSaveData mSaveData;

        private SpecBaseMono mSpecBase;

        public void Init(ItemInstInfo itemInfo, ItemSaveData saveData)
        {
            mItemInfo = itemInfo;
            mSaveData = saveData;

            mSpecBase = GetComponent<SpecBaseMono>();

            System.Random random = new System.Random(mItemInfo.RandomSeed);
            mSpecBase.Init(mItemInfo.SpecData.Specs, random);

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
