using System;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;

namespace PahlUnity.Demo
{
    public class SkillBullet : SkillObject
    {
        [SerializeField] protected string _AnimStateName = "Attack";

        [SerializeField] protected ProjectileBase2D _ProjPrefab;
        [SerializeField] protected Transform _FirePoint;
        [SerializeField] protected bool _AlignFireRotation = false;
        [SerializeField] protected bool _IsMultiShot = false;
        [SerializeField, ShowIf(nameof(_IsMultiShot))] protected float _ProjSpreadAngle = 10f;
        [SerializeField] protected int _BaseProjCount = 1;

        [Foldout("Events"), SerializeField] protected UnityEvent _OnStart;
        [Foldout("Events"), SerializeField] protected UnityEvent<int> _OnFire;
        [Foldout("Events"), SerializeField] protected UnityEvent _OnEnd;

        protected PlayerController2D mPlayerCtrl = null;

        protected int mAnimStateNameHash = 0;

        private bool mIsAttacking = false;

        protected override void Awake()
        {
            base.Awake();
            mPlayerCtrl = mBaseObject.GetComp<PlayerController2D>();
            mAnimStateNameHash = Animator.StringToHash(_AnimStateName);
        }

        public override void OnInputPressing()
        {
            base.OnInputPressing();

            if (mIsAttacking)
            {
                if (!IsCanAttack())
                {
                    StopAttackState();
                }
            }
            else
            {
                if (IsCanAttack())
                {
                    StartAttackState();
                }
            }
        }

        public override void OnInputUp()
        {
            base.OnInputUp();

            if (mIsAttacking)
            {
                StopAttackState();
            }
        }

        bool IsCanAttack()
        {
            if (mPlayerCtrl.IsCurrentState(PlayerState.Normal))
                return true;

            return false;
        }

        void StartAttackState()
        {
            mIsAttacking = true;
            mBaseObject.Anim.SetParamBool(AnimatorParams.IsAttacking, true);
            mBaseObject.Anim.SetParamFloat(AnimatorParams.AttackSpeed, GetAttackSpeedMultiplier());
            mBaseObject.Anim.SetLayerWeight(1, 1);
            mBaseObject.Anim.PlayAnim(mAnimStateNameHash, OnFireAttackState, null, 1);

            _OnStart.Invoke();
        }
        void OnFireAttackState(int idx)
        {
            CreateProjectiles();
            _OnFire.Invoke(idx);
        }
        void StopAttackState()
        {
            mIsAttacking = false;
            mBaseObject.Anim.SetLayerWeight(1, 0);
            mBaseObject.Anim.SetParamBool(AnimatorParams.IsAttacking, false);
            mBaseObject.Anim.SetParamFloat(AnimatorParams.AttackSpeed, 1);
            _OnEnd.Invoke();
        }

        public virtual void CreateProjectiles()
        {
            Vector3 forwardDir = mBaseObject.Body2D.FrontDirVec2;
            Vector3 startPos = _FirePoint.position;
            int targetLayerMask = 1 << LayerID.Enemy | 1 << LayerID.Terrain;

            ProjectileBase2D.Create(_ProjPrefab, startPos, forwardDir, targetLayerMask);
        }

        public void DoDamage(Collider2D col)
        {
            float damage = 1;
            Health health = col.ExGetBase().Health;
            if (health != null)
            {
                health.GetDamaged(new DamageInfo(damage), mBaseObject);
            }
        }

        float GetAttackSpeedMultiplier()
        {
            float percentModifier = mBaseObject.Spec.GetPercentModifier(SpecFields.AttackSpeed);
            float multiplier = percentModifier / 100f;
            float finalMultiplier = multiplier > 0 ? 1f + multiplier : (1 / (1f - multiplier));
            return finalMultiplier;
        }
    }
}
