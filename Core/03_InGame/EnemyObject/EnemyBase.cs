using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlUnity
{
    public class EnemyBase : MonoBehaviour
    {
        [SerializeField] Transform FirePoint = null;
        [SerializeField] ProjectileBase ProjPrefab;

        [SerializeField]
        [Dropdown(nameof(IDList))]
        string _ResourceID = "";
        List<string> IDList { get => TableDataContainer<EnemyResourceData>.Instance.GetAllInfo().Select(info => info.EnemyID).ToList(); }

        public SpecEnemy Spec { get; private set; } = null;

        protected BaseObject mBase = null;

        private void Awake()
        {
            mBase = GetComponentInParent<BaseObject>();

            Spec = mBase.GetComponentInChildren<SpecEnemy>();
            Spec.InitData(_ResourceID);
            Spec.LinkOption(mBase.Buffs.TotalBuffOption);
        }

        protected virtual void Start()
        {
            mBase.Health.OnDied.AddListener(OnDeath);
        }

        public virtual void OnDeath()
        {
            mBase.Body.LockBody = true;

            if (MyUtils.IsPercentHit((int)Spec.ItemDrop.PercentValue))
                DropItem();

            if (MyUtils.IsPercentHit(30))
                DropGold();

            if (MyUtils.IsPercentHit(15))
                DropPotion();
        }

        static float mItemDropTime = 0;
        void DropItem()
        {
            if (!MyUtils.IsCooltimeOver(mItemDropTime, 30))
                return;

            mItemDropTime = Time.time;

            int playerLevel = InGameManager.Instance.Engine.Player.GetComponentInChildren<Experience>().CurrentLevel;
            ItemObject.TryCreateNewItem(mBase.Body.Center, Quaternion.identity, playerLevel);
        }
        void DropGold()
        {
            // Gold itemObj = Instantiate(ResourceManager.Instance.GetPrefabGold(), mBase.Body.Center, Quaternion.identity);
            // itemObj.GoldAmount = Spec.GoldOnDeath;
        }
        static float mPotionDropTime = 0;
        void DropPotion()
        {
            if (!MyUtils.IsCooltimeOver(mPotionDropTime, 10))
                return;

            mPotionDropTime = Time.time;

            // if (MyUtils.IsPercentHit(50))
            //     Instantiate(ResourceManager.Instance.GetPrefabLifePotion(), mBase.Body.Center, Quaternion.identity);
            // else
            //     Instantiate(ResourceManager.Instance.GetPrefabManaPotion(), mBase.Body.Center, Quaternion.identity);
        }

        public void DoAttackMelee(BaseObject target)
        {
            // 스킬 오브젝트 생성
            Vector2 startPos = FirePoint.position;
            ProjectileBase obj = ProjectileBase.Create(ProjPrefab, startPos, mBase.transform.rotation, mBase.gameObject.layer);
            obj.OnHit.AddListener((col) =>
            {
                // 충돌 시 처리할 내용
                Health health = col.ExGetBase().GetComponentInChildren<Health>();
                if (health != null)
                {
                    float damage = mBase.Spec.BaseAttack;
                    health.GetDamaged(damage);
                }
            });
        }

        public void DoAttackShotToPlayer(BaseObject target)
        {
            // 스킬 오브젝트 생성
            Vector3 frontDir = mBase.Body.FrontDirVec2;
            Vector2 startPos = FirePoint.position;
            Vector3 dirToTarget = (target.Body.Center - startPos).normalized;
            Vector2 clampedDir = dirToTarget.ExClampRotate(frontDir, 60);
            ProjectileBase obj = ProjectileBase.Create(ProjPrefab, startPos, mBase.transform.rotation, mBase.gameObject.layer);
            obj.transform.right = clampedDir;
            obj.OnHit.AddListener((col) =>
            {
                // 충돌 시 처리할 내용
                Health health = col.ExGetBase().GetComponentInChildren<Health>();
                if (health != null)
                {
                    float damage = mBase.Spec.BaseAttack;
                    health.GetDamaged(damage);
                }
            });
        }
        public void DoAttackShotForward(BaseObject target)
        {
            // 스킬 오브젝트 생성
            Vector2 startPos = FirePoint.position;
            ProjectileBase obj = ProjectileBase.Create(ProjPrefab, startPos, mBase.transform.rotation, mBase.gameObject.layer);
            obj.OnHit.AddListener((col) =>
            {
                // 충돌 시 처리할 내용
                Health health = col.ExGetBase().GetComponentInChildren<Health>();
                if (health != null)
                {
                    float damage = mBase.Spec.BaseAttack;
                    health.GetDamaged(damage);
                }
            });
        }
        public void DoAttackThrowToPlayer(BaseObject target)
        {
            // 스킬 오브젝트 생성
            Vector2 startPos = FirePoint.position;
            ProjectileBase obj = ProjectileBase.Create(ProjPrefab, startPos, mBase.transform.rotation, mBase.gameObject.layer);

            // 수류탄 투척시 대상 거리에 따른 초기 속도 조절(실험에 근거한 데이터 및 수식..) 
            Vector3 dist = target.Body.Center - mBase.Body.Center;
            float distYRate = Mathf.Max(dist.y, 0);
            Vector2 startVel = Vector2.zero;
            startVel.y = Mathf.Clamp((10 + (dist.y * 1.1f)), 5, 20);
            startVel.x = Mathf.Abs(dist.x * 1.1f) + (distYRate * 0.7f);
            if (mBase.transform.right.x < 0)
                startVel.x *= -1;

            obj.Stats.MoveSpeed = startVel.magnitude;
            obj.transform.right = startVel.normalized;

            obj.OnHit.AddListener((col) =>
            {
                // 충돌 시 처리할 내용
                Health health = col.ExGetBase().GetComponentInChildren<Health>();
                if (health != null)
                {
                    float damage = mBase.Spec.BaseAttack;
                    health.GetDamaged(damage);
                }
            });
        }
        public void SetTarget(BaseObject target)
        {
            mBase.GetComponentInChildren<EnemyAI>().SetTarget(target);
        }
    }
}
