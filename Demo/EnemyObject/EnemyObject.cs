using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlUnity.Demo
{
    public class EnemyObject : MonoBehaviour
    {
        [SerializeField] private EnemySpecData _SpecData;

        private SpecBase mSpecBase;

        void Awake()
        {
            mSpecBase = GetComponent<SpecBase>();
            mSpecBase.SetSpecs(_SpecData.Specs, 0);
        }
    }
}
