using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public interface IMinigameGameObjectPool : IGameObjectPool
{
    GameObject Instantiate<T>(T key) where T : struct, IConvertible, IComparable, IFormattable;
    UniTask PreloadPrefabs<T>() where T : struct, IConvertible, IComparable, IFormattable;
}

public class MinigameGameObjectPool : IMinigameGameObjectPool
{
    private IGameObjectPool MinigamePool;
    private IPrefabLibrary Library;

    public MinigameGameObjectPool(IGameObjectPool minigamePool, IPrefabLibrary library)
    {
        MinigamePool = minigamePool;
        Library = library;
    }

    public void Clear()
    {
        MinigamePool.Clear();
        Library.Clear();
    }

    public void Dispose()
    {
        MinigamePool.Dispose();
        Library.Dispose();
    }

    public GameObject Instantiate<T>(T key) where T : struct, IConvertible, IComparable, IFormattable
    {
        return Library.InstantiatePrefab(key);
    }

    public GameObject Pool<T>(T key) where T : struct, IConvertible, IComparable, IFormattable
    {
        return MinigamePool.Pool(key);
    }

    public void Pop(GameObject gameObject)
    {
        MinigamePool.Pop(gameObject);
    }

    public async UniTask PreloadPrefabs<T>() where T : struct, IConvertible, IComparable, IFormattable
    {
        await Library.PreloadPrefabs(typeof(T));
    }
}