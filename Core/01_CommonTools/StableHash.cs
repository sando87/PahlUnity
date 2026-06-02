namespace PahlUnity
{
    /// <summary>
    /// FNV-1a 64bit, 32bit 해시 함수
    /// GetHashCode() 대신 사용.
    /// GetHashCode()는 플랫폼이나 개발 및 실행 환경에 따라 값이 달라질 수 있기 때문에
    /// 해시 충돌을 완전히 피할 수는 없지만 GetHashCode()보다 해시 충돌 확률이 낮음
    /// </summary>
    public static class StableHash
    {
        private const ulong Offset64 = 14695981039346656037UL;
        private const ulong Prime64 = 1099511628211UL;

        private const uint Offset32 = 2166136261U;
        private const uint Prime32 = 16777619U;

        public static int ExGetStableHash32(this string val)
        {
            return ToInt32(val);
        }
        public static long ExGetStableHash64(this string val)
        {
            return ToInt64(val);
        }

        public static long ToInt64(string value)
        {
            ulong hash = Compute64(value);
            return (long)(hash & 0x7FFFFFFFFFFFFFFFUL);
        }

        public static int ToInt32(string value)
        {
            uint hash = Compute32(value);
            return (int)(hash & 0x7FFFFFFFU);
        }

        private static ulong Compute64(string value)
        {
            if (value == null)
                return 0;

            ulong hash = Offset64;
            for (int i = 0; i < value.Length; i++)
            {
                hash ^= value[i];
                hash *= Prime64;
            }

            return hash;
        }

        private static uint Compute32(string value)
        {
            if (value == null)
                return 0;

            uint hash = Offset32;
            for (int i = 0; i < value.Length; i++)
            {
                hash ^= value[i];
                hash *= Prime32;
            }

            return hash;
        }
    }
}
