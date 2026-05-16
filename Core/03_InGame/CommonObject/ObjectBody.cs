using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace PahlBit
{
    public class ObjectBody : MonoBehaviour
    {
        [SerializeField] BoxCollider2D _ThinPlatform = null;

        BoxCollider2D mCollider = null;

        public Vector2 Center { get => transform.position.ExToVector2() + mCollider.offset; }
        public Vector2 Size { get => mCollider.size; }
        public Vector2 Foot { get => Center - (transform.up * Size * 0.5f); }
        public Vector2 Head { get => Center + (transform.up * Size * 0.5f); }
        public Vector2 Front { get => Center + (transform.right * Size * 0.5f); }
        public Vector2 Back { get => Center - (transform.right * Size * 0.5f); }

        public Vector2 FootFront { get => Center + new Vector2(Size.x * 0.5f * FrontDirInt, -Size.y * 0.5f); }
        public Vector2 FootBack { get => Center + new Vector2(-Size.x * 0.5f * FrontDirInt, -Size.y * 0.5f); }

        public Rect Rect { get => mCollider.ExToRect(); }
        public Vector2 FrontDirVec2 { get => transform.right; }
        public int FrontDirInt { get => transform.right.x > 0 ? 1 : -1; }

        public bool LockBody { get => !mCollider.enabled; set => mCollider.enabled = !value; }
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
    }
}