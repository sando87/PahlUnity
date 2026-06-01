using System;
using UnityEngine;

namespace PahlUnity.Demo
{
    public class EnemySpecMono : MonoBehaviour
    {
        [SerializeField] private EnemySpecData _SpecData;

        private SpecBaseMono mSpecBase;

        public void Init()
        {
            mSpecBase = GetComponent<SpecBaseMono>();
            mSpecBase.Init(_SpecData.Specs);
        }

        void Start()
        {
            Init();

            Debug.Log(mSpecBase[SpecFields.MaxHP]);
            Debug.Log(mSpecBase[SpecFields.MaxMP]);
            Debug.Log(mSpecBase[SpecFields.MoveSpeed]);
            Debug.Log(mSpecBase[SpecFields.AttackSpeed]);
        }
    }
}
