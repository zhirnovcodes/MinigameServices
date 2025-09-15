using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceProviders;

public class SceneManager
{
    private readonly IResourceLoader ResourceLoader;

    public SceneManager(IResourceLoader resourceLoader)
    {
        ResourceLoader = resourceLoader;
    }

    public async UniTask<object> LoadScene(string path, ILoadingPercentHandler handler = null)
    {
        var result = await ResourceLoader.LoadScene(path, handler);
        
        if (result.Status == ResourceLoadStatus.Success)
        {
            return result.SuccessData;
        }
        
        return null;
    }

    public async UniTask OpenScene(object handler)
    {
        await ((SceneInstance)handler).ActivateAsync();
    }

    public async UniTask DeloadScene(object handler)
    {
        await ResourceLoader.DeloadScene((SceneInstance)handler);
    }

    public T FindInScene<T>(object sceneInstance) where T : class
    {
        var scene = ((SceneInstance)sceneInstance).Scene;

        var rootObjects = scene.GetRootGameObjects();

        foreach (var rootObject in rootObjects)
        {
            var res = rootObject.GetComponent<T>();
            if (res != null)
            {
                return res;
            }

            // Also check in children
            res = rootObject.GetComponentInChildren<T>();
            if (res != null)
            {
                return res;
            }
        }

        return null;
    }
}
