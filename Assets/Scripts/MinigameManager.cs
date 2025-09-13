using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class MinigameManager : IMinigameManager
{
    private readonly IResourceLoader ResourceLoader;
    private readonly MinigameLoadingCurtainPresenter LoadingCurtainPresenter;

    private Minigames CurrentMinigame;
    private MinigameInputData CurrentInputData;
    private bool IsMinigameLoaded = false;
    private IMinigameModel MinigameModel;

    [Inject]
    public MinigameManager(
        IResourceLoader resourceLoader,
        MinigameLoadingCurtainPresenter loadingCurtainPresenter)
    {
        ResourceLoader = resourceLoader;
        LoadingCurtainPresenter = loadingCurtainPresenter;
    }

    public async UniTaskVoid LoadMinigame(Minigames minigame, MinigameInputData input)
    {
        LoadingCurtainPresenter.Enable();
        
        CurrentMinigame = minigame;
        CurrentInputData = input;
        
        var result = await ResourceLoader.LoadMinigameScene(minigame, LoadingCurtainPresenter);
        
        if (result.Status == ResourceLoadStatus.Success)
        {
            // Find IMinigameModel implementation in the loaded scene
            MinigameModel = FindMinigameModelInScene(result.SuccessData);
            
            if (MinigameModel != null)
            {
                IsMinigameLoaded = true;
                MinigameModel.Init(CurrentInputData);
            }
            else
            {
                Debug.LogError($"No IMinigameModel implementation found in scene: {minigame}");
            }
        }
        else
        {
            Debug.LogError($"Failed to load minigame scene: {minigame}");
        }
        LoadingCurtainPresenter.Disable();
    }

    public async UniTaskVoid StartMinigame()
    {
        if (!IsMinigameLoaded)
        {
            Debug.LogError("Cannot start minigame: minigame is not loaded");
            return;
        }

        try
        {
            MinigameModel.Start();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error starting minigame: {ex.Message}");
        }
    }

    public void DeloadMinigame()
    {
        if (IsMinigameLoaded)
        {
            MinigameModel.Dispose();
            MinigameModel = null;
            IsMinigameLoaded = false;
            CurrentMinigame = default;
            CurrentInputData = null;
        }
    }

    private IMinigameModel FindMinigameModelInScene(Scene scene)
    {
        var rootObjects = scene.GetRootGameObjects();
        
        foreach (var rootObject in rootObjects)
        {
            var minigameModel = rootObject.GetComponent<IMinigameModel>();
            if (minigameModel != null)
            {
                return minigameModel;
            }
            
            // Also check in children
            minigameModel = rootObject.GetComponentInChildren<IMinigameModel>();
            if (minigameModel != null)
            {
                return minigameModel;
            }
        }
        
        return null;
    }
}
