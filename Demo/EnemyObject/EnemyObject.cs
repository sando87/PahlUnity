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

        private SpecBaseMono mSpecBase;

        void Awake()
        {
            mSpecBase = GetComponent<SpecBaseMono>();
            mSpecBase.Init(_SpecData.Specs, 0);
        }
    }
}
