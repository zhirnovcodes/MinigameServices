using Cysharp.Threading.Tasks;
using UnityEngine;
using System;

public interface IPrefabLibrary : IDisposable
{
    UniTask PreloadPrefabs(Type enumType);
    GameObject GetPrefab<T>(T key) where T : struct, IConvertible, IComparable, IFormattable;
    GameObject InstantiatePrefab<T>(T key) where T : struct, IConvertible, IComparable, IFormattable;
    void Clear();
}
