using Cysharp.Threading.Tasks;
using UnityEngine;
using System;

public interface IPrefabLibrary : IDisposable
{
    UniTask PreloadAllAssets();
    GameObject GetPrefab<T>(T key) where T : struct, IConvertible, IComparable, IFormattable;
    GameObject InstantiatePrefab<T>(T key) where T : struct, IConvertible, IComparable, IFormattable;
    void ReleasePrefab<T>(string key);
}
