using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace PahlUnity.Demo
{
    public static class InteractMaskDemo
    {
        public const InteractMask Projectile = (InteractMask)(1 << 16);

        public const InteractMask Item = (InteractMask)(1 << 17);

        public const InteractMask NPC = (InteractMask)(1 << 18);
    }


}