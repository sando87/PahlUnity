using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlUnity
{
    public class EnemyBase : MonoBehaviour
    {
        [SerializeField]
        [Dropdown(nameof(IDList))]
        string _ResourceID = "";
        List<string> IDList { get => TableDataContainer<EnemyResourceData>.Instance.GetAllInfo().Select(info => info.EnemyID).ToList(); }

        protected BaseObject mBase = null;
        protected ObjectBody2D mBody = null;
        protected SpecEnemy mSpec = null;

        private void Awake()
        {
            mBase = GetComponentInParent<BaseObject>();
            mBody = mBase.GetComp<ObjectBody2D>();
            mSpec = mBase.GetComp<SpecEnemy>();

            mSpec = mBase.GetComp<SpecEnemy>();
            mSpec.InitData(_ResourceID);
            mSpec.LinkOption(mBase.GetComp<BuffController>().TotalBuffOption);
        }

        protected virtual void Start()
        {
            mBase.GetComp<Health>().OnDied += OnDeath;
        }

        public virtual void OnDeath(HealthInfo before, HealthInfo after)
        {
            mBody.LockBody = true;
        }

    }
}
