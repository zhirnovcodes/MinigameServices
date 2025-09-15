using Cysharp.Threading.Tasks;
using UnityEngine;
using System;
using System.Collections.Generic;

public class PrefabLibrary : IPrefabLibrary
{
    private readonly IResourceLoader ResourceLoader;
    private readonly Dictionary<Type, Dictionary<int, GameObject>> LoadedPrefabs = new Dictionary<Type, Dictionary<int, GameObject>>();

    private string RootFolder = "Assets";

    public PrefabLibrary(IResourceLoader resourceLoader)
    {
        ResourceLoader = resourceLoader;
    }

    public void SetRootFolderName(string name)
    {
        RootFolder = name;
    }

    public GameObject GetPrefab<T>(T key) where T : struct, IConvertible, IComparable, IFormattable
    {
        var enumType = typeof(T);
        var enumValue = Convert.ToInt32(key);
        
        if (LoadedPrefabs.TryGetValue(enumType, out var prefabDictionary) && 
            prefabDictionary.TryGetValue(enumValue, out var prefab))
        {
            return prefab;
        }

        throw new InvalidOperationException($"Prefab not found for enum value: {key} of type: {enumType.Name}. Make sure to call PreloadAllAssets() first.");
    }

    public GameObject InstantiatePrefab<T>(T key) where T : struct, IConvertible, IComparable, IFormattable
    {
        var prefab = GetPrefab(key);
        return UnityEngine.Object.Instantiate(prefab);
    }

    public void Clear()
    {
        foreach (var dict in LoadedPrefabs.Values)
        {
            dict.Clear();
        }

        LoadedPrefabs.Clear();
    }

    public void Dispose()
    {
        // Clean up all loaded prefabs
        foreach (var prefabDictionary in LoadedPrefabs.Values)
        {
            foreach (var prefab in prefabDictionary.Values)
            {
                if (prefab != null)
                {
                    UnityEngine.Object.Destroy(prefab);
                }
            }
        }

        LoadedPrefabs.Clear();
    }

    public async UniTask PreloadPrefabs(Type enumType) 
    {
        foreach (object prefabKey in Enum.GetValues(enumType))
        {
            var result = await ResourceLoader.LoadPrefab(enumType, prefabKey, null, RootFolder);

            if (result == null == false)
            {
                var enumValueInt = Convert.ToInt32(prefabKey);

                if (!LoadedPrefabs.ContainsKey(enumType))
                {
                    LoadedPrefabs[enumType] = new Dictionary<int, GameObject>();
                }

                LoadedPrefabs[enumType][enumValueInt] = result;
            }
        }
    }
}
