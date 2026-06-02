using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

namespace PahlUnity.Demo
{
    /// <summary>
    /// 플레이어 캐릭터의 입력을 받아서 이동, 점프, 상호작용 등을 처리하는 컨트롤러 클래스
    /// </summary>
    public class PlayerController2D : MonoBehaviour
    {
        [SerializeField] float _JumpForce = 25f;
        [SerializeField] float _DashForce = 10f;
        [SerializeField] private LayerMask _LayersForGroundCheck;

        public bool IsGrounded { get => GetGroundState(); }

        public bool LockMove { get; set; } = false;
        public bool LockJump { get; set; } = false;
        public bool LockDash { get; set; } = false;
        public bool LockAll
        {
            get { return LockMove && LockJump && LockDash; }
            set { LockMove = value; LockJump = value; LockDash = value; }
        }

        BaseObject mBaseObj = null;
        ObjectPhysics2D mPhy = null;
        ObjectBody2D mBody = null;
        InputPlayer mPlayerInput = null;
        SpecBaseMono mSpec = null;

        bool mIsSecondJump = false;

        private void Awake()
        {
            mBaseObj = GetComponentInParent<BaseObject>();
            mPhy = mBaseObj.GetComp<ObjectPhysics2D>();
            mBody = mBaseObj.GetComp<ObjectBody2D>();
            mPlayerInput = mBaseObj.GetComp<InputPlayer>();
            mSpec = mBaseObj.GetComp<SpecBaseMono>();
        }

        private void Update()
        {
            DoMovement();
            Jump();
            DropDown();
            Dash();
        }

        void DoMovement()
        {
            if (LockMove)
                return;

            float moveX = mPlayerInput.MoveX;
            moveX = moveX > 0 ? 1 : moveX < 0 ? -1 : 0;
            mPhy.VelocityX = moveX * mSpec[SpecFields.MoveSpeed];
        }
        void Jump()
        {
            if (LockJump)
                return;

            if (mPlayerInput.JustPressed(InputActionName.Jump)
            && mPlayerInput.MoveY >= 0)
            {
                if (IsGrounded)
                {
                    mIsSecondJump = false;
                    mPhy.DoJump(_JumpForce);
                }
                else
                {
                    if (!mIsSecondJump)
                    {
                        mIsSecondJump = true;
                        mPhy.DoJump(_JumpForce);
                    }
                }
            }
            else if (mPlayerInput.JustReleased(InputActionName.Jump))
            {
                mPhy.StopJump();
            }
        }
        void DropDown()
        {
            if (LockJump)
                return;

            if (mPlayerInput.JustPressed(InputActionName.Jump)
            && mPlayerInput.MoveY < 0
            && IsGrounded)
            {
                mBody.LockThinPlatform = true;
                this.ExDelayedCoroutine(0.2f, () => mBody.LockThinPlatform = false);
            }
        }
        void Dash()
        {
            if (LockDash)
                return;

            if (mPlayerInput.JustPressed(InputActionName.Dash))
            {
                mPhy.AddForce(mBody.FrontDirVec2 * _DashForce, ForceMode2D.Impulse);
            }
        }

        private bool GetGroundState()
        {
            int layerMask = _LayersForGroundCheck.value;
            Vector2 footPos = mBody.Foot;

            bool isOverlapped = Physics2D.OverlapCircle(footPos + new Vector2(0, 0.1f), 0.05f, layerMask);

            Vector2 bodySize = mBody.Size;
            Rect box = new Rect();
            box.size = new Vector2(bodySize.x, 0.1f);
            box.center = footPos + new Vector2(0, 0.05f);
            bool isCasted = Physics2D.BoxCast(box.center, box.size, 0, Vector2.down, 0.1f, layerMask);

            return !isOverlapped && isCasted;
        }

    }
}