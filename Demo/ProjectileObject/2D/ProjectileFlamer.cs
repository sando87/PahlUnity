using PahlUnity;
using UnityEngine;

namespace PahlUnity.Demo.TwoD
{
    public class ProjectileFlamer : ProjectileBase2D
    {
        [SerializeField] private float _Offset = 0.5f;

        ObjectBody2D mOwnerBody = null;

        public static ProjectileFlamer Create(ProjectileFlamer prefab, Vector2 position, Vector2 direction, int targetLayerMask, ObjectBody2D ownerBody)
        {
            ProjectileBase2D obj = ProjectileBase2D.Create(prefab, position, direction, targetLayerMask);
            ProjectileFlamer flamer = obj as ProjectileFlamer;
            flamer.mOwnerBody = ownerBody;
            return flamer;
        }

        public override void StartProjectile()
        {
            // mPhy.LockGravity = true;
            // mPhy.Velocity = Vector2.zero;
            base.StartProjectile();
        }

        protected override void Update()
        {
            base.Update();

            UpdateFollowOwner();
        }

        void UpdateFollowOwner()
        {
            if (mOwnerBody == null)
                return;

            Vector2 dir = mOwnerBody.FrontDirVec2;
            Vector2 pos = mOwnerBody.Center.ExToVector2() + dir * _Offset;
            transform.position = pos;
            transform.right = dir;
            mPhy.Velocity = Vector2.zero;
        }
    }
}
