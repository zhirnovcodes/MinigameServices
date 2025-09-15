using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public interface IMinigamesConfigLoader
{
    UniTask<ScriptableObject> LoadConfig<T>(
        T enumValue,
        ILoadingPercentHandler percentHandler = null) where T : struct, IConvertible, IComparable, IFormattable;
}

public sealed class MinigamesConfigLoader : IMinigamesConfigLoader
{
    private IResourceLoader ResourceLoader;

    public MinigamesConfigLoader(IResourceLoader resourceLoader)
    {
        ResourceLoader = resourceLoader;
    }

    private string Root;

    public void SetMinigame(Minigames id)
    {
        Root = $"Assets/Content/Remote/Minigames/{id}";
    }

    public async UniTask<ScriptableObject> LoadConfig<T>(
        T enumValue,
        ILoadingPercentHandler percentHandler = null) where T : struct, IConvertible, IComparable, IFormattable
    {
        var root = Root;
        return await ResourceLoader.LoadConfig(enumValue, percentHandler, root);
    }
 }
