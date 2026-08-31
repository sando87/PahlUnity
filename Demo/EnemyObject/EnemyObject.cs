using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlUnity.Demo
{
    public class EnemyObject : MonoBehaviour
    {
        private EnemyInstData mEnemyInstData;

        private SpecBase mSpecBase;

        public void Init(EnemyInstData instData)
        {
            mEnemyInstData = instData;

            mSpecBase = GetComponent<SpecBase>();

            mSpecBase.SetSpecs(mEnemyInstData.SpecData.Specs, 0);

            mSpecBase.UpdateAllValuesByStep(mEnemyInstData.LevelIndex);
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
