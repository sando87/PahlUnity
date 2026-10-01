using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlUnity.Demo
{
    public class EnemyObject : MonoBehaviour
    {
        [SerializeField] EnemySpecData _SpecData;

        private EnemyInstData mEnemyInstData;

        private BaseObject mBaseObject;

        void Awake()
        {
            mBaseObject = this.ExGetBase();
        }

        void Start()
        {
            mEnemyInstData = new EnemyInstData(_SpecData);
            mBaseObject.Spec.SetSpecs(mEnemyInstData.SpecData.Specs, 0);
            mBaseObject.Spec.UpdateAllValuesByStep(mEnemyInstData.LevelIndex);

            mBaseObject.Health.OnDied += OnEnemyDied;
        }

        void OnEnemyDied(BaseObject attacker)
        {
            // Drop Random Item
            ItemSpecData dropItemSpec = TableDataContainer<ItemSpecData>.Instance.GetRandomItem();
            ItemObject itemPrefab = InGameManager.Instance.Engine.ItemPrefab;
            ItemObject itemObj = Instantiate(itemPrefab, transform.position, Quaternion.identity);
            itemObj.Init(dropItemSpec);
        }
    }
}
