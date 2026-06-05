namespace PahlUnity.Demo
{
    public static class InteractMask
    {
        public const uint Nothing = 0u;
        public const uint Unit = 1u << 0;
        public const uint Skill = 1u << 1;
        public const uint Terrain = 1u << 2;
        public const uint Projectile = 1u << 3;
        public const uint Props = 1u << 4;
        public const uint Item = 1u << 5;
        public const uint TriggerSignal = 1u << 6;
        public const uint Everything = 127u;
    }
}
