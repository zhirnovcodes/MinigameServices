using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class MinigameManager : IMinigameManager
{
    private readonly IResourceLoader ResourceLoader;
    private readonly MinigameLoadingCurtainPresenter LoadingCurtainPresenter;

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
        /*
        LoadingCurtainPresenter.Enable();
        
        CurrentInputData = input;
        
        var result = await ResourceLoader.LoadMinigameScene(minigame, LoadingCurtainPresenter);
        
        if (result.Status == ResourceLoadStatus.Success)
        {
            // Load the scene asynchronously
            var scene = ResourceLoaderHelper.GetMinigameSceneName(minigame);
            var loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            
            await loadOperation;
            
            // Find IMinigameModel implementation in the loaded scene
            var loadedScene = SceneManager.GetSceneByName(sceneName);
            MinigameModel = FindMinigameModelInScene(loadedScene);
            
            if (MinigameModel == null)
            {
                Debug.LogError($"No IMinigameModel implementation found in scene: {minigame}");
            }
            else
            {
                IsMinigameLoaded = true;
                SubscribeToMinigameEvents();
                MinigameModel.Init(CurrentInputData);
            }
        }
        else
        {
            Debug.LogError($"Failed to load minigame scene: {minigame}");
        }
        LoadingCurtainPresenter.Disable();
        */
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
            UnsubscribeFromMinigameEvents();
            MinigameModel.Dispose();
            MinigameModel = null;
            IsMinigameLoaded = false;
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

    private void SubscribeToMinigameEvents()
    {
        if (MinigameModel != null)
        {
            MinigameModel.Finished += OnMinigameFinished;
            MinigameModel.Faulted += OnMinigameFaulted;
            MinigameModel.Closed += OnMinigameClosed;
        }
    }

    private void UnsubscribeFromMinigameEvents()
    {
        if (MinigameModel != null)
        {
            MinigameModel.Finished -= OnMinigameFinished;
            MinigameModel.Faulted -= OnMinigameFaulted;
            MinigameModel.Closed -= OnMinigameClosed;
        }
    }

    private void OnMinigameFinished(MinigameResultData result)
    {
        Debug.Log($"Minigame finished with status: {result.Status}");
        // Handle minigame completion
    }

    private void OnMinigameFaulted(MinigameErrorData error)
    {
        Debug.LogError($"Minigame faulted: {error.Error}");
        // Handle minigame error
    }

    private void OnMinigameClosed()
    {
        Debug.Log("Minigame closed");
        // Handle minigame closure
        DeloadMinigame();
    }
}
