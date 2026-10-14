using UnityEngine;

namespace PahlUnity.Demo
{
    public class FSMAttack : FiniteStateBase
    {
        const float AttackCooldown = 1.5f;

        [SerializeField] private ProjectileBase2D _ProjectilePrefab = null;

        BaseObject mBase;
        EnemyController2D mEnemy;

        void Awake()
        {
            mBase = this.ExGetBase();
            mEnemy = mBase.GetComp<EnemyController2D>();
        }

        public override void EnterState()
        {
            base.EnterState();
            mEnemy.StopHorizontal();
            mEnemy.TurnToPlayer();
            mBase.Anim.PlayAnim(AnimStateNameHash.Attack, OnFireAttackAnim, OnEndAttackAnim);
        }

        public override void LeaveState()
        {
            base.LeaveState();
            mBase.Anim.SetParamFloat(AnimatorParams.AttackSpeed, 1f);
        }

        void OnFireAttackAnim(int idx)
        {
            DoAttackFire();
            mEnemy.SetAttackCooldown(AttackCooldown);
        }

        void OnEndAttackAnim()
        {
            if (mEnemy.IsPlayerInDetectRange())
                mEnemy.ChangeState(EnemyState.Chase);
            else
                mEnemy.ChangeState(EnemyState.Patrol);
        }

        void DoAttackFire()
        {
            Vector2 startPos = mBase.Body2D.Center.ExToVector2() + (mBase.Body2D.FrontDirVec2 * 0.5f);
            Vector2 attackDir = mBase.Body2D.FrontDirVec2;
            int targetLayerMask = 0;
            ProjectileBase2D obj = ProjectileBase2D.Create(_ProjectilePrefab, startPos, attackDir, targetLayerMask, mBase);
            obj.OnHit += (col) =>
            {
                Health health = col.ExGetCompInBase<Health>();
                if (health != null)
                {
                    float damage = 1f;
                    DamageInfo damageInfo = new DamageInfo(damage);
                    health.GetDamaged(damageInfo, mBase);
                }
            };
        }
    }
}
