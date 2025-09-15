using Cysharp.Threading.Tasks;
using UnityEngine;
using System;


/*
Content/Local/Prefabs/CharacterCards/3D/256
Content/Local/Prefabs/CharacterCards/UI/64
...128
...256

Content/Local/Prefabs/MinigameResources/UI/64
...128
...256

Content/Local/Prefabs/Scenes


Content/Remote/Minigames

 */

public static class ResourceLoaderHelper
{
    public static string GetMinigameSceneName(Minigames minigameId)
    {
        return $"Assets/Content/Remote/Minigames/{minigameId}/main.unity";
    }

    public static async UniTask<SceneLoadResult> LoadMinigameScene(
        this IResourceLoader resourceLoader, 
        Minigames minigameId, 
        ILoadingPercentHandler percentHandler = null)
    {
        var sceneName = GetMinigameSceneName(minigameId);
        return await resourceLoader.LoadScene(sceneName, percentHandler);
    }

    public static async UniTask<object> LoadMinigameScene(
        this SceneManager manager,
        Minigames minigameId,
        ILoadingPercentHandler percentHandler = null)
    {
        var sceneName = GetMinigameSceneName(minigameId);
        return await manager.LoadScene(sceneName, percentHandler);
    }

    public static async UniTask<GameObject> LoadPrefab<T>(
        this IResourceLoader resourceLoader,
        T enumValue,
        ILoadingPercentHandler percentHandler = null,
        string rootFolder = "Assets") where T : struct, IConvertible, IComparable, IFormattable
    {
        var enumType = typeof(T);
        var fullName = enumType.FullName;
        fullName = fullName.Replace('+', '/');

        // Convert namespace dots to forward slashes for path
        var assetPath = fullName.Replace('.', '/');
        assetPath = $"{rootFolder}/{assetPath.Replace('.', '/')}/{enumValue}.prefab";

        var result = await resourceLoader.LoadAsset(assetPath, percentHandler);
        
        if (result.Status == ResourceLoadStatus.Success && result.SuccessData is GameObject gameObject)
        {
            return gameObject;
        }
        
        Debug.LogError($"Failed to load prefab: {assetPath}");
        return null;
    }

    public static async UniTask<GameObject> LoadPrefab(
        this IResourceLoader resourceLoader,
        Type enumType,
        object enumValue,
        ILoadingPercentHandler percentHandler = null,
        string rootFolder = "Assets")
    {
        var fullName = enumType.FullName;
        fullName = fullName.Replace('+', '/');

        // Convert namespace dots to forward slashes for path and append enum value
        var assetPath = $"{rootFolder}/{fullName.Replace('.', '/')}/{enumValue}.prefab";

        var result = await resourceLoader.LoadAsset(assetPath, percentHandler);
        
        if (result.Status == ResourceLoadStatus.Success && result.SuccessData is GameObject gameObject)
        {
            return gameObject;
        }
        
        Debug.LogError($"Failed to load prefab: {assetPath}");
        return null;
    }

    public static async UniTask<ScriptableObject> LoadConfig<T>(
        this IResourceLoader resourceLoader,
        T enumValue,
        ILoadingPercentHandler percentHandler = null,
        string rootFolder = "Assets") where T : struct, IConvertible, IComparable, IFormattable
    {
        var enumType = typeof(T);
        var fullName = enumType.FullName;
        fullName = fullName.Replace('+', '/');

        // Convert namespace dots to forward slashes for path
        var assetPath = fullName.Replace('.', '/');
        assetPath = $"{rootFolder}/{assetPath.Replace('.', '/')}/{enumValue}.asset";

        var result = await resourceLoader.LoadAsset(assetPath, percentHandler);

        if (result.Status == ResourceLoadStatus.Success && result.SuccessData is ScriptableObject so)
        {
            return so;
        }

        Debug.LogError($"Failed to load config: {assetPath}");
        return null;
    }
}
