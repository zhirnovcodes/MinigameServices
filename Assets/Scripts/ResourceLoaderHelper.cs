using Cysharp.Threading.Tasks;

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
