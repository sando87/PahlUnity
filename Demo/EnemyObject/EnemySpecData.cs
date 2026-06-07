using System.Collections.Generic;
using UnityEngine;

namespace PahlUnity.Demo
{
    [CreateAssetMenu(fileName = "EnemySpecData", menuName = "Demo/EnemySpecData")]
    public class EnemySpecData : ScriptableObject
    {
        [SerializeField] private string _EnemyID = "";
        [SerializeField] private string _EnemyName = "";
        [SerializeField] private string _EnemyDesc = "";
        [SerializeField] private GameObject _EnemyPrefab = null;
        [SerializeField] private List<SpecFieldRaw> _Specs = new List<SpecFieldRaw>();

        public string EnemyID => _EnemyID;
        public string EnemyName => _EnemyName;
        public string EnemyDesc => _EnemyDesc;
        public GameObject EnemyPrefab => _EnemyPrefab;

        public IReadOnlyList<SpecFieldRaw> Specs => _Specs;

    }
}