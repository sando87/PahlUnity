using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace PahlUnity.Demo
{
    public enum PlayerActionState
    {
        Normal, Dash, Hit, Death
    }
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] float _DashSpeed = 15f;
        [SerializeField] float _DashDuration = 0.2f;
        [SerializeField] float _DashCooltime = 1.0f;
        [SerializeField] float _TurnSpeed = 720f;
        [SerializeField] UnityEvent _OnDash = null;

        public bool IsGrounded { get => mBaseObj.Physics3D != null && mBaseObj.Physics3D.IsGrounded; }

        public bool LockMove { get; set; } = false;

        BaseObject mBaseObj = null;
        float mDashTime = 0;
        Dictionary<PlayerActionState, FiniteStateBase> mStates = new();

        private void OnValidate()
        {
            _DashSpeed = Mathf.Max(0f, _DashSpeed);
            _DashDuration = Mathf.Max(0f, _DashDuration);
            _TurnSpeed = Mathf.Max(0f, _TurnSpeed);
        }

        private void Awake()
        {
            mBaseObj = GetComponentInParent<BaseObject>();

            BindStates();
        }

        void Start()
        {
            mBaseObj.Health.OnDamaged += OnDamaged;
            mBaseObj.Health.OnDied += OnDied;

            mBaseObj.FSM.TryChangeState(GetState(PlayerActionState.Normal));
        }

        void BindStates()
        {
            FiniteStateBase normalState = new();
            normalState.EventUpdate += UpdateNormalState;
            mBaseObj.FSM.SetDefaultState(normalState);
            mStates.Add(PlayerActionState.Normal, normalState);

            FiniteStateBase dashState = new();
            dashState.EventEnter += EnterDashState;
            dashState.EventLeave += LeaveDashState;
            mStates.Add(PlayerActionState.Dash, dashState);

            FiniteStateBase damagedState = new();
            damagedState.EventEnter += EnterDamagedState;
            mStates.Add(PlayerActionState.Hit, damagedState);

            FiniteStateBase deathState = new();
            deathState.EventEnter += EnterDeathState;
            mStates.Add(PlayerActionState.Death, deathState);
        }

        FiniteStateBase GetState(PlayerActionState state)
        {
            return mStates[state];
        }
        public bool IsCurrentState(PlayerActionState state)
        {
            return mStates[state] == mBaseObj.FSM.CurrentState;
        }

        void UpdateNormalState()
        {
            if (TryDash())
                return;

            DoMovement();
        }

        void EnterDashState()
        {
            mDashTime = Time.time;
            StopMovingForAction();
            Vector3 dashDir = new(mBaseObj.Input.MoveX, 0f, mBaseObj.Input.MoveY);
            dashDir = dashDir.magnitude > 0.0001f ? dashDir.normalized : mBaseObj.Body3D.FrontDirVec3;
            mBaseObj.Body3D.Turn(dashDir);
            mBaseObj.Physics3D.DoDash(dashDir, _DashSpeed, _DashDuration);
            _OnDash?.Invoke();
            mBaseObj.Anim.PlayAnim(AnimStateNameHash.Death, null, (isCanceled) =>
            {
                if (!isCanceled)
                {
                    mBaseObj.FSM.TryChangeState(GetState(PlayerActionState.Normal));
                }
            });
        }

        void LeaveDashState()
        {
            mBaseObj.Physics3D.StopDash();
        }

        void EnterDamagedState()
        {
            StopMovingForAction();

            mBaseObj.Anim.PlayAnim(AnimStateNameHash.Hit, null, (isCanceled) =>
            {
                if (!isCanceled)
                {
                    mBaseObj.FSM.TryChangeState(GetState(PlayerActionState.Normal));
                }
            });
        }

        void EnterDeathState()
        {
            mBaseObj.Anim.CancelAndThrowException(0);
            StopMovingForAction();
            mBaseObj.Anim.PlayAnim(AnimStateNameHash.Death);
            mBaseObj.Body3D.LockBody = true;
        }


        public void DoMovement()
        {
            if (LockMove)
                return;

            if (TryGetMoveInput(out Vector3 moveDir))
            {
                float moveSpeed = mBaseObj.Spec[SpecFields.MoveSpeed];
                mBaseObj.Physics3D.Move(moveDir, moveSpeed);
                Turn(moveDir);
                mBaseObj.Anim.SetParamBool(AnimatorParams.IsMoving, true);
            }
            else
            {
                StopMovingForAction();
            }
        }

        public bool TryDash()
        {
            if (IsCurrentState(PlayerActionState.Hit)
            || IsCurrentState(PlayerActionState.Death))
                return false;

            if (mBaseObj.Input.JustPressed(InputActionNameHash.Dash))
            {
                float dashCooltime = _DashCooltime;
                if (MyUtils.IsCooltimeOver(mDashTime, dashCooltime))
                {
                    mBaseObj.FSM.TryChangeState(GetState(PlayerActionState.Dash));
                    return true;
                }
            }

            return false;
        }

        void OnDied(BaseObject attacker)
        {
            mBaseObj.FSM.TryChangeState(GetState(PlayerActionState.Death));
        }

        void OnDamaged(IDamageInfo damage, BaseObject attacker)
        {
            if (damage is DamageInfo damageInfo)
            {
                if (damageInfo.IsPowerAttack)
                    mBaseObj.FSM.TryChangeState(GetState(PlayerActionState.Hit), true);
            }
        }

        public void StopMovingForAction()
        {
            mBaseObj.Physics3D.StopMoving();
            mBaseObj.Physics3D.StopDash();
            mBaseObj.Anim.SetParamBool(AnimatorParams.IsMoving, false);
        }

        void Turn(Vector3 moveDir)
        {
            if (moveDir.sqrMagnitude <= 0.0001f)
                return;

            if (_TurnSpeed <= 0f)
            {
                mBaseObj.Body3D.Turn(moveDir);
            }
            else
            {
                mBaseObj.Body3D.Turn(moveDir, _TurnSpeed * Time.deltaTime);
            }
        }

        bool TryGetMoveInput(out Vector3 moveDir)
        {
            Vector2 moveInput = mBaseObj.Input.MoveXY;
            if (moveInput.sqrMagnitude <= 0.0001f)
            {
                moveDir = Vector3.zero;
                return false;
            }

            moveDir = new Vector3(moveInput.x, 0f, moveInput.y);
            moveDir = moveDir.normalized;
            return true;
        }
    }
}