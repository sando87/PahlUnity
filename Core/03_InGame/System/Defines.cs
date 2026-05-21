#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define IS_DEBUG_MODE
#endif

using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace PahlUnity
{
    public readonly partial struct AnimStateHash
    {
        public readonly int mVal;
        public AnimStateHash(string value) => mVal = Animator.StringToHash(value);
        public AnimStateHash(int value) => mVal = value;

        public static implicit operator int(AnimStateHash info) => info.mVal;
        public static implicit operator AnimStateHash(int val) => new AnimStateHash(val);
        public static implicit operator AnimStateHash(string val) => new AnimStateHash(val);

        public override bool Equals(object obj) => obj is AnimStateHash other && mVal == other.mVal;
        public override int GetHashCode() => mVal;

        public static bool operator ==(AnimStateHash a, AnimStateHash b) => a.mVal == b.mVal;
        public static bool operator !=(AnimStateHash a, AnimStateHash b) => a.mVal != b.mVal;

        // Example
        // public static readonly AnimStateNameHash Idle = new("Idle");
        // public static readonly AnimStateNameHash Jump = new("Jump");
        // public static readonly AnimStateNameHash Attack = new("Attack");
        // public static readonly AnimStateNameHash Death = new("Death");
    }

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