using UnityEngine;

namespace PahlUnity.Demo
{
    public class CameraFollow2D : MonoBehaviour
    {
        [SerializeField] Transform _Target = null;
        [SerializeField] Vector2 _Offset = Vector2.zero;
        [SerializeField] float _SmoothTime = 0.15f;
        [SerializeField] bool _FollowX = true;
        [SerializeField] bool _FollowY = true;
        [SerializeField] bool _SnapOnStart = true;

        float mCameraZ;
        Vector3 mSmoothVelocity = Vector3.zero;

        void Awake()
        {
            mCameraZ = transform.position.z;
        }

        void Start()
        {
            if (_SnapOnStart && _Target != null)
                SnapToTarget();
        }

        void LateUpdate()
        {
            if (_Target == null)
            {
                return;
            }

            Vector3 targetPos = BuildTargetPosition();
            Vector3 currentPos = transform.position;

            if (!_FollowX)
                targetPos.x = currentPos.x;
            if (!_FollowY)
                targetPos.y = currentPos.y;

            transform.position = Vector3.SmoothDamp(currentPos, targetPos, ref mSmoothVelocity, _SmoothTime);
        }

        Vector3 BuildTargetPosition()
        {
            return new Vector3(
                _Target.position.x + _Offset.x,
                _Target.position.y + _Offset.y,
                mCameraZ);
        }

        void SnapToTarget()
        {
            Vector3 targetPos = BuildTargetPosition();
            Vector3 currentPos = transform.position;

            if (!_FollowX)
                targetPos.x = currentPos.x;
            if (!_FollowY)
                targetPos.y = currentPos.y;

            transform.position = targetPos;
            mSmoothVelocity = Vector3.zero;
        }
    }
}
