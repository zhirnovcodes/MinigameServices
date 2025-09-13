using Cysharp.Threading.Tasks;


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
}
