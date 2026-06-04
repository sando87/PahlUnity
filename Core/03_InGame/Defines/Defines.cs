#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define IS_DEBUG_MODE
#endif

using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace PahlUnity
{
    [System.Flags]
    public enum InteractMask : uint
    {
        Nothing = 0,
        Unit = 1 << 0,
        Skill = 1 << 1,
        Terrain = 1 << 2,
        Projectile = 1 << 3,
        Props = 1 << 4,
        Item = 1 << 5,
        Everything = 0xffffffff
    }
}