using UnityEngine;

namespace PahlUnity.Demo
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public class TopDownCameraController : MonoBehaviour
    {
        [SerializeField] private Transform _Target = null;
        [SerializeField] private Vector3 _LookOffset = new Vector3(0f, 1f, 0f);
        [SerializeField] private float _Height = 12f;
        [SerializeField] private float _PitchAngle = 60f;
        [SerializeField] private float _YawAngle = 0f;
        [SerializeField] private float _FollowDamping = 0.15f;
        [SerializeField] private float _RotationDamping = 12f;

        private Vector3 mFollowVelocity = Vector3.zero;

        private void Awake()
        {
            SnapToTarget();
        }

        private void LateUpdate()
        {
            if (_Target == null)
                return;

            Vector3 targetPoint = _Target.position + _LookOffset;
            Vector3 desiredPosition = CalculateCameraPosition(targetPoint);
            Quaternion desiredRotation = Quaternion.LookRotation(targetPoint - desiredPosition, Vector3.up);

            transform.position = _FollowDamping <= 0f
                ? desiredPosition
                : Vector3.SmoothDamp(transform.position, desiredPosition, ref mFollowVelocity, _FollowDamping);

            transform.rotation = _RotationDamping <= 0f
                ? desiredRotation
                : Quaternion.Slerp(transform.rotation, desiredRotation, _RotationDamping * Time.deltaTime);
        }

        private void OnValidate()
        {
            _Height = Mathf.Max(0.1f, _Height);
            _PitchAngle = Mathf.Clamp(_PitchAngle, 10f, 89f);
            _FollowDamping = Mathf.Max(0f, _FollowDamping);
            _RotationDamping = Mathf.Max(0f, _RotationDamping);
        }

        private Vector3 CalculateCameraPosition(Vector3 targetPoint)
        {
            float pitchRad = _PitchAngle * Mathf.Deg2Rad;
            float horizontalDistance = _Height / Mathf.Tan(pitchRad);
            Vector3 horizontalDir = Quaternion.Euler(0f, _YawAngle, 0f) * Vector3.back;

            return targetPoint + horizontalDir * horizontalDistance + Vector3.up * _Height;
        }

        private void SnapToTarget()
        {
            if (_Target == null)
                return;

            Vector3 targetPoint = _Target.position + _LookOffset;
            transform.position = CalculateCameraPosition(targetPoint);
            transform.rotation = Quaternion.LookRotation(targetPoint - transform.position, Vector3.up);
            mFollowVelocity = Vector3.zero;
        }
    }
}
