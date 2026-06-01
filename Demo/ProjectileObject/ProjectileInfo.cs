
using Unity.VisualScripting.Antlr3.Runtime.Misc;

namespace PahlUnity.Demo
{
    [System.Serializable]
    public class ProjectileInfo
    {
        public float MoveSpeed;
        public float FireAngle;
        public float AttackRange;
        public float SplashRange;
        public float Duration;
        public float Interval;
        public float StartDelay;
        public float RotateSpeed;
        public bool AimToVelocity;
    }
}