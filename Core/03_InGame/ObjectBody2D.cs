using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace PahlUnity
{
    public class ObjectBody2D : ObjectBodyBase
    {
        [SerializeField] BoxCollider2D _ThinPlatform = null;

        BoxCollider2D mCollider = null;

        public event Action<bool> OnTurn;

        public override Vector3 Center { get => transform.position.ExToVector2() + mCollider.offset; }
        public override Vector3 Size { get => mCollider.size; }
        public override Vector3 Foot { get => Center - (Size.y * 0.5f * transform.up); }
        public override Vector3 Head { get => Center + (Size.y * 0.5f * transform.up); }
        public override Vector3 Front { get => Center + (Size.x * 0.5f * transform.right); }
        public override Vector3 Back { get => Center - (Size.x * 0.5f * transform.right); }

        public override Vector3 FootFront { get => Center + new Vector3(Size.x * 0.5f * FrontDirInt, -Size.y * 0.5f, 0); }
        public override Vector3 FootBack { get => Center + new Vector3(-Size.x * 0.5f * FrontDirInt, -Size.y * 0.5f, 0); }

        public Rect Rect { get => mCollider.ExToRect(); }
        public Vector2 FrontDirVec2 { get => transform.right; }
        public int FrontDirInt { get => transform.right.x > 0 ? 1 : -1; }

        public override bool LockBody { get => !mCollider.enabled; set => mCollider.enabled = !value; }
        public bool LockThinPlatform { get { return _ThinPlatform ? !_ThinPlatform.enabled : false; } set { if (_ThinPlatform) _ThinPlatform.enabled = !value; } }

        void Awake()
        {
            mCollider = GetComponent<BoxCollider2D>();

            // 유닛 이동시 Terrain와 얇은지형 경계에서 자꾸 밑으로 떨어지는 버그 수정
            mCollider.transform.localPosition += new Vector3(0, 0.01f, 0);
            if (_ThinPlatform != null)
                _ThinPlatform.transform.localPosition += new Vector3(0, 0.01f, 0);
        }

        public void LockThinPlatformMomentarily(float duration = 0.2f)
        {
            LockThinPlatform = true;
            this.ExDelayedCoroutine(duration, () => LockThinPlatform = false);
        }

        public void Turn(int worldDir)
        {
            if (worldDir == 0) return;
            if (worldDir == FrontDirInt) return;

            Vector3 front = worldDir > 0 ? Vector3.forward : Vector3.back;
            transform.rotation = Quaternion.LookRotation(front, transform.up);
            OnTurn?.Invoke(worldDir > 0);
        }
        public void Turn(float worldDir)
        {
            if (worldDir.ExIsAlmostZero()) return;

            Turn(worldDir > 0 ? 1 : -1);
        }
        public void Turn(Transform target)
        {
            if (target == null) return;

            int worldDir = target.position.x > transform.position.x ? 1 : -1;
            Turn(worldDir);
        }
        public void Turn(Vector2 targetPos)
        {
            int worldDir = targetPos.x > transform.position.x ? 1 : -1;
            Turn(worldDir);
        }
    }
}