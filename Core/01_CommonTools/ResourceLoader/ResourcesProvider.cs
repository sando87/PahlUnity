using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Data Loader Interface
/// </summary>
namespace PahlUnity
{
    public class ResourcesProvider : IResourceProvider
    {
        public async UniTask<T> LoadAsync<T>(string key) where T : Object
        {
            ResourceRequest request = Resources.LoadAsync<T>(key);

            await request;

            return request.asset as T;
        }
    }
}