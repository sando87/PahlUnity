#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define IS_DEBUG_MODE
#endif

using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace PahlUnity.Demo
{
    public struct SaveUserPlayData : IEventParam
    {
        public readonly bool ImmediateSave;
        public SaveUserPlayData(bool immediateSave) => ImmediateSave = immediateSave;
    }
    public struct SaveUserSettingData : IEventParam
    {
    }

}