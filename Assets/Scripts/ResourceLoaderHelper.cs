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
        return $"Resources/Remote/Minigames/{minigameId}/main.scene";
    }

    public static async UniTask<SceneLoadResult> LoadMinigameScene(
        this IResourceLoader resourceLoader, 
        Minigames minigameId, 
        ILoadingPercentHandler percentHandler = null)
    {
        var sceneName = GetMinigameSceneName(minigameId);
        return await resourceLoader.LoadScene(sceneName, percentHandler);
    }

    public static async UniTask<GameObject> GetUICharacterImage(
        this IResourceLoader resourceLoader,
        TextureSizes textureSize,
        CharacterCards characterCard,
        ILoadingPercentHandler percentHandler = null)
    {
        var sizeString = textureSize.ToString(); // Remove 'S' prefix
        var characterString = characterCard.ToString();
        var assetName = $"Content/Local/Prefabs/CharacterCards/UI/{sizeString}/{characterString}{sizeString}";
        
        var result = await resourceLoader.LoadAsset(assetName, percentHandler);
        
        if (result.Status == ResourceLoadStatus.Success && result.SuccessData is GameObject gameObject)
        {
            return gameObject;
        }
        
        throw new System.Exception($"Failed to load UI character image: {assetName}");
    }

    public static async UniTask<GameObject> LoadPrefab<T>(
        this IResourceLoader resourceLoader,
        T enumValue,
        ILoadingPercentHandler percentHandler = null) where T : struct, IConvertible, IComparable, IFormattable
    {
        var enumType = typeof(T);
        var fullName = enumType.FullName;
        
        // Convert namespace dots to forward slashes for path
        var assetPath = fullName.Replace('.', '/');
        
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
        ILoadingPercentHandler percentHandler = null)
    {
        var fullName = enumType.FullName;
        fullName = fullName.Replace('+', '/');

        // Convert namespace dots to forward slashes for path and append enum value
        var assetPath = $"Assets/{fullName.Replace('.', '/')}/{enumValue}.prefab";

        var result = await resourceLoader.LoadAsset(assetPath, percentHandler);
        
        if (result.Status == ResourceLoadStatus.Success && result.SuccessData is GameObject gameObject)
        {
            return gameObject;
        }
        
        Debug.LogError($"Failed to load prefab: {assetPath}");
        return null;
    }
}
