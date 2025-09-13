using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class AddressablesResourceLoader : IResourceLoader
{
    public async UniTask<ResourceLoadResult> LoadAsset(string name, ILoadingPercentHandler percentHandler = null)
    {
        try
        {
            var handle = Addressables.LoadAssetAsync<UnityEngine.Object>(name);
            
            if (percentHandler == null)
            {
                await handle;
            }
            else
            {
                while (handle.IsDone == false)
                {
                    percentHandler.SetLoadingPercent(handle.PercentComplete);
                    await UniTask.Yield();
                }
            }

            switch (handle.Status)
            {
                case AsyncOperationStatus.Succeeded:
                {
                    return new ResourceLoadResult
                    {
                        Status = ResourceLoadStatus.Success,
                        SuccessData = handle.Result
                    };
                }
                case AsyncOperationStatus.Failed:
                {
                    return new ResourceLoadResult
                    {
                        Status = ResourceLoadStatus.Fail
                    };
                }
                default:
                {
                    return new ResourceLoadResult
                    {
                        Status = ResourceLoadStatus.Fail
                    };
                }

            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Exception while loading asset '{name}': {ex.Message}");
            return new ResourceLoadResult
            {
                Status = ResourceLoadStatus.Fail,
                SuccessData = null
            };
        }
    }

    public async UniTask<SceneLoadResult> LoadScene(string name, ILoadingPercentHandler percentHandler = null)
    {
        try
        {
            var handle = Addressables.LoadSceneAsync(name, LoadSceneMode.Additive);

            if (percentHandler == null)
            {
                await handle;
            }
            else
            {
                while (handle.IsDone == false)
                {
                    percentHandler.SetLoadingPercent(handle.PercentComplete);
                    await UniTask.Yield();
                }
            }

            switch (handle.Status)
            {
                case AsyncOperationStatus.Succeeded:
                    {
                        return new SceneLoadResult
                        {
                            Status = ResourceLoadStatus.Success,
                            SuccessData = handle.Result.Scene
                        };
                    }
                case AsyncOperationStatus.Failed:
                    {
                        return new SceneLoadResult
                        {
                            Status = ResourceLoadStatus.Fail
                        };
                    }
                default:
                    {
                        return new SceneLoadResult
                        {
                            Status = ResourceLoadStatus.Fail
                        };
                    }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Exception while loading scene '{name}': {ex.Message}");
            return new SceneLoadResult
            {
                Status = ResourceLoadStatus.Fail,
            };
        }
    }
}
