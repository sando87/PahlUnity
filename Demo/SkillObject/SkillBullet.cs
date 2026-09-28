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
        protected FiniteStateBase mAttackState = null;

        protected override void Awake()
        {
            base.Awake();
            mPlayerCtrl = mBaseObject.GetComp<PlayerController2D>();
            mAnimStateNameHash = Animator.StringToHash(_AnimStateName);

            mAttackState = this.AddComponent<FiniteStateBase>();
            mAttackState.EventEnter += OnEnterAttackState;
            mAttackState.EventLeave += OnLeaveAttackState;
        }

        public override void OnInputDown()
        {
            base.OnInputDown();

            if (IsCanAttack())
            {
                mBaseObject.FSM.TryChangeState(mAttackState);
            }
        }

        bool IsCanAttack()
        {
            if (!mPlayerCtrl.IsCurrentState(PlayerState.Normal))
                return false;

            return true;
        }

        void OnEnterAttackState()
        {
            // mBaseObject.Physics2D.StopMoving();
            mBaseObject.Anim.SetParamBool(AnimatorParams.IsAttacking, true);
            mBaseObject.Anim.SetParamFloat(AnimatorParams.AttackSpeed, GetAttackSpeedMultiplier());
            mBaseObject.Anim.SetLayerWeight(1, 1);
            mBaseObject.Anim.PlayAnim(mAnimStateNameHash, OnFireAttackState, () =>
            {
                mBaseObject.FSM.ChangeDefaultState();
            }, 1);

            _OnStart.Invoke();
        }
        void OnFireAttackState(int idx)
        {
            CreateProjectiles();
            _OnFire.Invoke(idx);
        }
        void OnLeaveAttackState()
        {
            // mBaseObject.Physics2D.StopMoving();
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
