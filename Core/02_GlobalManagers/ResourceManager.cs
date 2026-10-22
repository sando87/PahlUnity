using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 리소스 관리
/// </summary>

namespace PahlUnity
{
    public class ResourceManager : SingletonMono<ResourceManager>
    {
        public IResourceProvider Provider { get; private set; } = null;

        [SerializeField] private List<PrefabEntry> _PrefabEntries = new();

        [SerializeField] private List<ScriptableObject> _ScriptableObjects = new();

        private readonly Dictionary<string, ResourceHandle> mCache = new();
        private readonly Dictionary<string, GameObject> mPrefabTable = new();
        private readonly Dictionary<string, ScriptableObject> mScriptableObjectTable = new();

        protected override void Awake()
        {
            base.Awake();

            if (Instance != this)
                return;

            BuildPrefabTable();
            BuildScriptableObjectTable();
        }

        public void Initialize(IResourceProvider provider)
        {
            Provider = provider;
        }

        public GameObject GetPrefab(string key)
        {
            if (mPrefabTable.TryGetValue(key, out GameObject prefab))
                return prefab;

            Debug.LogError($"Prefab Not Found : {key}");
            return null;
        }

        public ScriptableObject GetScriptableObject(string key)
        {
            if (mScriptableObjectTable.TryGetValue(key, out ScriptableObject scriptableObject))
                return scriptableObject;

            Debug.LogError($"Scriptable Object Not Found : {key}");
            return null;
        }

        public IReadOnlyList<ScriptableObject> GetAllScriptableObjects()
        {
            return _ScriptableObjects;
        }

        public IReadOnlyList<ScriptableObject> GetScriptableObjects(System.Func<ScriptableObject, bool> predicate)
        {
            return _ScriptableObjects.Where(predicate).ToList();
        }

        public async UniTask<T> LoadAsync<T>(string key) where T : Object
        {
            if (mPrefabTable.TryGetValue(key, out GameObject registeredPrefab))
            {
                T registered = registeredPrefab as T;
                if (registered != null)
                    return registered;
            }

            if (mScriptableObjectTable.TryGetValue(key, out ScriptableObject registeredScriptableObject))
            {
                T registered = registeredScriptableObject as T;
                if (registered != null)
                    return registered;
            }

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

        private void BuildPrefabTable()
        {
            mPrefabTable.Clear();

            for (int i = 0; i < _PrefabEntries.Count; i++)
            {
                PrefabEntry entry = _PrefabEntries[i];
                if (entry == null || string.IsNullOrEmpty(entry.Key) || entry.Prefab == null)
                    continue;

                bool duplicated = mPrefabTable.ContainsKey(entry.Key);
                LOG.errorif(duplicated, $"Duplicate prefab key : {entry.Key}");
                if (duplicated)
                    continue;

                mPrefabTable.Add(entry.Key, entry.Prefab);
            }
        }

        private void BuildScriptableObjectTable()
        {
            mScriptableObjectTable.Clear();

            for (int i = 0; i < _ScriptableObjects.Count; i++)
            {
                ScriptableObject scriptableObject = _ScriptableObjects[i];
                if (scriptableObject == null)
                    continue;

                bool duplicated = mScriptableObjectTable.ContainsKey(scriptableObject.name);
                LOG.errorif(duplicated, $"Duplicate scriptable object key : {scriptableObject.name}");
                if (duplicated)
                    continue;

                mScriptableObjectTable.Add(scriptableObject.name, scriptableObject);
            }
        }

    }

    [System.Serializable]
    public class PrefabEntry
    {
        [SerializeField] private string _Key;
        [SerializeField] private GameObject _Prefab;

        public string Key => _Key;
        public GameObject Prefab => _Prefab;
    }

    public class ResourceHandle
    {
        public Object Asset;
        public int RefCount;
    }
}