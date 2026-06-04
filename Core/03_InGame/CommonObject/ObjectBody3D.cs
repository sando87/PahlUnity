using UnityEngine;

namespace PahlUnity
{
    public class ObjectBody3D : MonoBehaviour
    {
        [SerializeField] BoxCollider _ThinPlatform = null;

        BaseObject mBaseObj = null;
        BoxCollider mCollider = null;

        public Vector3 Center { get => mCollider.transform.TransformPoint(mCollider.center); }
        public Vector3 Size { get => mCollider.size; }
        public Vector3 Foot { get => Center - (transform.up * Size.y * 0.5f); }
        public Vector3 Head { get => Center + (transform.up * Size.y * 0.5f); }
        public Vector3 Front { get => Center + (transform.forward * Size.z * 0.5f); }
        public Vector3 Back { get => Center - (transform.forward * Size.z * 0.5f); }

        public Vector3 FootFront { get => Center + (transform.forward * Size.z * 0.5f) - (transform.up * Size.y * 0.5f); }
        public Vector3 FootBack { get => Center - (transform.forward * Size.z * 0.5f) - (transform.up * Size.y * 0.5f); }

        public Bounds Bounds { get => mCollider.bounds; }
        public Vector3 FrontDirVec3 { get => transform.forward; }

        public bool LockBody { get => !mCollider.enabled; set => mCollider.enabled = !value; }
        public bool LockThinPlatform { get { return _ThinPlatform ? !_ThinPlatform.enabled : false; } set { if (_ThinPlatform) _ThinPlatform.enabled = !value; } }

        void Awake()
        {
            mBaseObj = this.ExGetBase();
            mCollider = GetComponent<BoxCollider>();

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

        public void Turn(Vector3 worldDir)
        {
            if (worldDir.sqrMagnitude <= 0.0001f) return;

            mBaseObj.transform.rotation = Quaternion.LookRotation(worldDir.normalized, mBaseObj.transform.up);
        }
        public void Turn(Vector3 worldDir, float delta)
        {
            if (worldDir.sqrMagnitude <= 0.0001f || delta <= 0f) return;

            mBaseObj.transform.rotation = Quaternion.RotateTowards(
                mBaseObj.transform.rotation,
                Quaternion.LookRotation(worldDir.normalized, mBaseObj.transform.up),
                delta);
        }
    }
}