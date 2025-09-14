using Cysharp.Threading.Tasks;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public class PrefabLibrary : IPrefabLibrary
{
    private readonly IResourceLoader ResourceLoader;
    private readonly Dictionary<Type, Dictionary<int, GameObject>> LoadedPrefabs = new Dictionary<Type, Dictionary<int, GameObject>>();

    public PrefabLibrary(IResourceLoader resourceLoader)
    {
        ResourceLoader = resourceLoader;
    }

    public async UniTask PreloadAllAssets()
    {
        // Get all enum types from the LocalPrefabs namespace
        var enumTypes = GetLocalPrefabEnumTypes();
        
        var loadTasks = new List<UniTask>();
        
        foreach (var enumType in enumTypes)
        {
            loadTasks.Add(PreloadPrefabs(enumType));
        }

        await UniTask.WhenAll(loadTasks);
        Debug.Log(LoadedPrefabs);
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

    public void ReleasePrefab<T>(string key)
    {
        var enumType = typeof(T);
        
        if (LoadedPrefabs.TryGetValue(enumType, out var prefabDictionary))
        {
            // Remove the specific prefab from the dictionary
            // Note: The key parameter should ideally be the enum value converted to string
            // This is a simplified implementation
            
            // If no prefabs left for this enum type, remove the entire type entry
            if (prefabDictionary.Count == 0)
            {
                LoadedPrefabs.Remove(enumType);
            }
        }
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

    private async UniTask PreloadPrefabs(Type enumType) 
    {
        foreach (object prefabKey in Enum.GetValues(enumType))
        {
            var result = await ResourceLoader.LoadPrefab(enumType, prefabKey);

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

    private List<System.Type> GetLocalPrefabEnumTypes()
    {
        var assembly = Assembly.GetExecutingAssembly();
        return assembly.GetTypes()
            .Where(t => t.IsEnum && t.Namespace?.StartsWith("Content.Local.Prefabs") == true)
            .ToList();
    }
}
