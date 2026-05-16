using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;




#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;

#endif

namespace PahlBit
{
    public static partial class MyExtensions
    {
        public static int ToInt(this float val)
        {
            return (int)(val + 0.0001f);
        }
        public static bool IsEquals(this float val, float targetVal)
        {
            return Mathf.Abs(val - targetVal) < 0.0001f;
        }

        public static UniTask ExDelayedTask(
            this MonoBehaviour mono,
            float delaySeconds,
            Action action,
            DelayType delayType = DelayType.DeltaTime,
            CancellationToken? externalToken = null)
        {
            var destroyToken = mono.GetCancellationTokenOnDestroy();

            CancellationToken ct = externalToken.HasValue
                ? CancellationTokenSource.CreateLinkedTokenSource(destroyToken, externalToken.Value).Token
                : destroyToken;

            return ExDelayTaskInternal(action, delaySeconds, delayType, ct);
        }

        static async UniTask ExDelayTaskInternal(
            Action action,
            float delaySeconds,
            DelayType delayType,
            CancellationToken ct)
        {
            try
            {
                await UniTask.Delay(
                    TimeSpan.FromSeconds(delaySeconds),
                    delayType,
                    cancellationToken: ct
                );
                action?.Invoke();
            }
            catch (OperationCanceledException)
            {
                // 정상 취소
            }
        }

        public static T ExGetRandom<T>(this List<T> list)
        {
            return list.Count > 0 ? list[UnityEngine.Random.Range(0, list.Count)] : default;
        }


    }
}