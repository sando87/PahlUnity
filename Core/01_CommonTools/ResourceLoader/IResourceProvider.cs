using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Unity Resource Object Interface
/// </summary>
namespace PahlUnity
{
    public interface IResourceProvider
    {
        UniTask<T> LoadAsync<T>(string key) where T : Object;
    }
}