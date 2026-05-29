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
        public IResourceProvider Provider { get; private set; } = null;

        private readonly Dictionary<string, ResourceHandle> mCache = new();

        public void Initialize(IResourceProvider provider)
        {
            Provider = provider;
        }

        public async UniTask<T> LoadAsync<T>(string key) where T : Object
        {
            LOG.errorif(Provider == null);

            // 캐시 확인
            if (mCache.TryGetValue(key, out var handle))
            {
                handle.RefCount++;

                return handle.Asset as T;
            }

            // 실제 로딩
            T asset = await Provider.LoadAsync<T>(key);

            if (asset == null)
            {
                Debug.LogError($"Load Failed : {key}");
                return null;
            }

            mCache[key] = new ResourceHandle
            {
                Asset = asset,
                RefCount = 1,
            };

            return asset;
        }

        public void Release(string key)
        {
            if (!mCache.TryGetValue(key, out var handle))
                return;

            handle.RefCount--;

            if (handle.RefCount > 0)
                return;

            mCache.Remove(key);

            Resources.UnloadUnusedAssets();
        }

    }

    public class ResourceHandle
    {
        public Object Asset;
        public int RefCount;
    }
}