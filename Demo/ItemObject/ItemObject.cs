using System;
using UnityEngine;

namespace PahlUnity.Demo
{
    public class ItemObject : MonoBehaviour
    {
        [SerializeField] private ItemSpecData _SpecData;

        private SpecBaseMono mSpecBase;
        private ItemSaveData mSaveData;

        public void Init(ItemSaveData saveData)
        {
            mSaveData = saveData;

            mSpecBase = GetComponent<SpecBaseMono>();
            mSpecBase.Init(_SpecData.Specs);
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
