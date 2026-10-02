using PahlUnity;
using UnityEngine;

namespace PahlUnity.Demo.TwoD
{
    public class ProjectileKatana : ProjectileBase2D
    {
        [SerializeField] private float _Offset = 0.5f;

        ObjectBody2D mOwnerBody = null;

        public static ProjectileKatana Create(ProjectileKatana prefab, Vector2 position, Vector2 direction, int targetLayerMask, ObjectBody2D ownerBody)
        {
            ProjectileBase2D obj = ProjectileBase2D.Create(prefab, position, direction, targetLayerMask);
            ProjectileKatana katana = obj as ProjectileKatana;
            katana.mOwnerBody = ownerBody;
            return katana;
        }

        public override void StartProjectile()
        {
            // mPhy.LockGravity = true;
            // mPhy.Velocity = Vector2.zero;
            base.StartProjectile();
        }

        protected override void Update()
        {
            UpdateFollowOwner();

            if (Stats.RotateSpeed != 0)
                transform.Rotate(0, 0, Stats.RotateSpeed * Time.deltaTime);
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
