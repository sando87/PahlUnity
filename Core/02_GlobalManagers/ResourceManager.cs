using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 리소스 관리
/// </summary>

namespace PahlUnity
{
    public class ResourceManager : SingletonMono<ResourceManager>
    {
        public IResourceProvider provider { get; private set; } = null;

        private readonly Dictionary<string, ResourceHandle> cache = new();

        public async UniTask<T> LoadAsync<T>(string key) where T : Object
        {
            // 캐시 확인
            if (cache.TryGetValue(key, out var handle))
            {
                handle.RefCount++;

                return handle.Asset as T;
            }

            // 실제 로딩
            T asset = await provider.LoadAsync<T>(key);

            if (asset == null)
            {
                Debug.LogError($"Load Failed : {key}");
                return null;
            }

            cache[key] = new ResourceHandle
            {
                Asset = asset,
                RefCount = 1,
            };

            return asset;
        }

        public void Release(string key)
        {
            if (!cache.TryGetValue(key, out var handle))
                return;

            handle.RefCount--;

            if (handle.RefCount > 0)
                return;

            cache.Remove(key);

            Resources.UnloadUnusedAssets();
        }

    }

    public class ResourceHandle
    {
        public Object Asset;
        public int RefCount;
    }
}