using UnityEngine;

namespace PahlUnity
{
    public class ObjectBody3D : ObjectBodyBase
    {
        [SerializeField] Collider _BodyCollider = null;

        BaseObject mBaseObj = null;
        Collider mCollider = null;

        public override Vector3 Center { get => Bounds.center; }
        public override Vector3 Size { get => Bounds.size; }
        public override Vector3 Foot { get => Center - (Size.y * 0.5f * transform.up); }
        public override Vector3 Head { get => Center + (Size.y * 0.5f * transform.up); }
        public override Vector3 Front { get => Center + (Size.z * 0.5f * transform.forward); }
        public override Vector3 Back { get => Center - (Size.z * 0.5f * transform.forward); }

        public override Vector3 FootFront { get => Center + (Size.z * 0.5f * transform.forward) - (Size.y * 0.5f * transform.up); }
        public override Vector3 FootBack { get => Center - (Size.z * 0.5f * transform.forward) - (Size.y * 0.5f * transform.up); }

        public Bounds Bounds { get => mCollider.bounds; }
        public Vector3 FrontDirVec3 { get => transform.forward; }

        public override bool LockBody { get => !mCollider.enabled; set => mCollider.enabled = !value; }

        void Awake()
        {
            mBaseObj = this.ExGetBase();
            InitCollider();
        }

        void InitCollider()
        {
            mCollider = _BodyCollider;
            if (mCollider == null)
            {
                mCollider = GetComponent<Collider>();
            }

            if (mCollider == null)
            {
                mCollider = mBaseObj.GetComponentInChildren<Collider>();
            }
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