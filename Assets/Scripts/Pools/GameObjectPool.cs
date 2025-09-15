using System;
using System.Collections.Generic;
using UnityEngine;

public interface IGameObjectPool : IDisposable
{
    GameObject Pool<T>(T key) where T : struct, System.IConvertible, System.IComparable, System.IFormattable;
    void Pop(GameObject gameObject);
    void Clear();
}

/// <summary>
/// GameObject pool that uses PrefabLibrary to instantiate prefabs with enum keys.
/// Provides efficient GameObject pooling to reduce instantiation overhead and garbage collection.
/// </summary>
public class GameObjectPool : IGameObjectPool 
{
    private readonly IPrefabLibrary PrefabLibrary;
    private readonly Dictionary<System.Type, Dictionary<int, Queue<GameObject>>> PooledObjects = new Dictionary<System.Type, Dictionary<int, Queue<GameObject>>>();
    private readonly Dictionary<GameObject, KeyValuePair<System.Type, int>> ObjectToKeyMap = new Dictionary<GameObject, KeyValuePair<System.Type, int>>();

    public GameObjectPool(IPrefabLibrary prefabLibrary)
    {
        PrefabLibrary = prefabLibrary;
    }

    /// <summary>
    /// Gets a GameObject from the pool using the specified enum key.
    /// If no pooled GameObject is available, creates a new one using PrefabLibrary.
    /// </summary>
    /// <typeparam name="T">Enum type for the prefab key</typeparam>
    /// <param name="key">Enum value representing the prefab to spawn</param>
    /// <returns>GameObject from pool or newly instantiated</returns>
    public GameObject Pool<T>(T key) where T : struct, System.IConvertible, System.IComparable, System.IFormattable
    {
        var enumType = typeof(T);
        var enumValue = System.Convert.ToInt32(key);
        
        // Get or create the queue for this enum type and value
        if (!PooledObjects.TryGetValue(enumType, out var typeDictionary))
        {
            typeDictionary = new Dictionary<int, Queue<GameObject>>();
            PooledObjects[enumType] = typeDictionary;
        }
        
        if (!typeDictionary.TryGetValue(enumValue, out var objectQueue))
        {
            objectQueue = new Queue<GameObject>();
            typeDictionary[enumValue] = objectQueue;
        }
        
        // Try to get an object from the pool
        if (objectQueue.Count > 0)
        {
            var pooledObject = objectQueue.Dequeue();
            if (pooledObject != null)
            {
                pooledObject.SetActive(true);
                // Ensure reverse mapping exists
                ObjectToKeyMap[pooledObject] = new KeyValuePair<System.Type, int>(enumType, enumValue);
                return pooledObject;
            }
        }
        
        // No pooled object available, create a new one
        var createdObject = PrefabLibrary.InstantiatePrefab(key);
        ObjectToKeyMap[createdObject] = new KeyValuePair<System.Type, int>(enumType, enumValue);
        return createdObject;
    }

    /// <summary>
    /// Returns a GameObject to the pool for reuse.
    /// </summary>
    /// <param name="gameObject">The GameObject to return to the pool</param>
    public void Pop(GameObject gameObject)
    {
        // Deactivate the GameObject
        gameObject.SetActive(false);
        
        // Enqueue back to the correct pool using the reverse mapping
        if (!ObjectToKeyMap.TryGetValue(gameObject, out var typeAndKey))
        {
            Debug.LogWarning($"No mapping found for GameObject: {gameObject.name}. The object will not be pooled.");
            return;
        }

        if (!PooledObjects.TryGetValue(typeAndKey.Key, out var typeDictionary))
        {
            typeDictionary = new Dictionary<int, Queue<GameObject>>();
            PooledObjects[typeAndKey.Key] = typeDictionary;
        }

        if (!typeDictionary.TryGetValue(typeAndKey.Value, out var objectQueue))
        {
            objectQueue = new Queue<GameObject>();
            typeDictionary[typeAndKey.Value] = objectQueue;
        }

        objectQueue.Enqueue(gameObject);
    }

    /// <summary>
    /// Clears all pooled objects and destroys them.
    /// </summary>
    public void Clear()
    {
        ObjectToKeyMap.Clear();

        foreach (var typeDictionary in PooledObjects.Values)
        {
            foreach (var objectQueue in typeDictionary.Values)
            {
                while (objectQueue.Count > 0)
                {
                    var gameObject = objectQueue.Dequeue();
                    if (gameObject != null)
                    {
                        UnityEngine.Object.Destroy(gameObject);
                    }
                }
            }
            typeDictionary.Clear();
        }
        
        PooledObjects.Clear();
    }

    public void Dispose()
    {
        Clear();
    }
}
