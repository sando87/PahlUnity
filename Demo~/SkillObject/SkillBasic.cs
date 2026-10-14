using System;
using DG.Tweening;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;

namespace PahlUnity.Demo
{
    public class SkillBasic : SkillObject
    {
        [SerializeField] protected ProjectileBase2D _ProjPrefab;
        [SerializeField] protected Transform _FirePoint;
        [SerializeField] protected bool _AlignFireRotation = false;
        [SerializeField] protected bool _IsMultiShot = false;
        [SerializeField, ShowIf(nameof(_IsMultiShot))] protected float _ProjSpreadAngle = 60f;
        [SerializeField, ShowIf(nameof(_IsMultiShot))] protected int _BaseProjCount = 1;

        [Foldout("Events"), SerializeField] protected UnityEvent _OnStart;
        [Foldout("Events"), SerializeField] protected UnityEvent<int> _OnFire;
        [Foldout("Events"), SerializeField] protected UnityEvent _OnEnd;

        bool mIsAttacking = false;
        float mAttackTime = 0f;

        protected override void Awake()
        {
            base.Awake();
        }

        public override void OnInputDown()
        {
            base.OnInputDown();

            if (mPlayerCtrl.IsCurrentState(PlayerState.Normal)
            && IsCooltimeOver())
            {
                StartAttackState();
            }
        }

        public override void OnInputPressing()
        {
            base.OnInputPressing();

            if (mIsAttacking)
            {
                if (!mPlayerCtrl.IsCurrentState(PlayerState.Normal))
                {
                    StopAttackState();
                }
                else
                {
                    bool isAttackable = IsCooltimeOver();
                    mBaseObject.Anim.SetParamBool(AnimatorParams.IsAttacking, isAttackable);
                }
            }
            else
            {
                if (mPlayerCtrl.IsCurrentState(PlayerState.Normal)
                && IsCooltimeOver())
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
                mBaseObject.Anim.SetParamBool(AnimatorParams.IsAttacking, false);
            }
        }

        bool IsCooltimeOver()
        {
            float cooltime = mSkillSpec[SpecFields.Cooltime];
            if (cooltime <= 0)
                return true;

            if (MyUtils.IsCooltimeOver(mAttackTime, cooltime))
                return true;

            return false;
        }

        void StartAttackState()
        {
            mIsAttacking = true;
            mBaseObject.Anim.SetParamBool(AnimatorParams.IsAttacking, true);
            // mBaseObject.Anim.SetParamFloat(AnimatorParams.AttackSpeed, GetAttackSpeedMultiplier());
            mBaseObject.Anim.SetLayerWeight(1, 1);
            mBaseObject.Anim.PlayAnim(AnimStateNameHash.Attack, OnFireAttackState, OnEndAttackState, 1);

            _OnStart.Invoke();
        }
        void OnFireAttackState(int idx)
        {
            mAttackTime = Time.time;
            if (_IsMultiShot)
            {
                CreateProjectiles();
            }
            else
            {
                CreateProjectile();
            }
            _OnFire.Invoke(idx);
        }
        void OnEndAttackState()
        {
            StopAttackState();
        }
        void StopAttackState()
        {
            mIsAttacking = false;
            mBaseObject.Anim.SetLayerWeight(1, 0);
            mBaseObject.Anim.SetParamBool(AnimatorParams.IsAttacking, false);
            // mBaseObject.Anim.SetParamFloat(AnimatorParams.AttackSpeed, 1);
            _OnEnd.Invoke();
        }

        void CreateProjectile()
        {
            Vector3 forwardDir = mBaseObject.Body2D.FrontDirVec2;
            Vector3 startPos = _FirePoint.position;
            int targetLayerMask = 0; //1 << LayerID.Enemy | 1 << LayerID.Terrain;

            ProjectileBase2D.Create(_ProjPrefab, startPos, forwardDir, targetLayerMask, mBaseObject);
        }

        void CreateProjectiles()
        {
            Vector3 forwardDir = mBaseObject.Body2D.FrontDirVec2;
            Vector3 startPos = _FirePoint.position;
            int targetLayerMask = 0; //1 << LayerID.Enemy | 1 << LayerID.Terrain;
            int projCount = _BaseProjCount;

            for (int i = 0; i < projCount; ++i)
            {
                float angleOffset = GetSpreadAngle(i, projCount, _ProjSpreadAngle);
                Vector2 attackDir = Quaternion.AngleAxis(angleOffset, Vector3.forward) * forwardDir;
                ProjectileBase2D.Create(_ProjPrefab, startPos, attackDir, targetLayerMask, mBaseObject);
            }
        }
        float GetSpreadAngle(int index, int count, float totalSpread)
        {
            if (count <= 1)
                return 0f;

            float step = totalSpread / (count - 1);
            return -totalSpread * 0.5f + step * index;
        }
    }
}
