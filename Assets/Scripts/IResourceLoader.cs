using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public interface IResourceLoader
{
    UniTask<ResourceLoadResult> LoadAsset(string name, ILoadingPercentHandler percentHandler = null);
    UniTask<SceneLoadResult> LoadScene(string name, ILoadingPercentHandler percentHandler = null);
}

public class ResourceLoadResult
{
    public ResourceLoadStatus Status;
    public UnityEngine.Object SuccessData;
}

public class SceneLoadResult
{
    public ResourceLoadStatus Status;
    public Scene SuccessData;
}

public interface ILoadingPercentHandler
{
    void SetLoadingPercent(float value);
}