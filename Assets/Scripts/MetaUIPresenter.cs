using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MetaUIPresenter : IDisposable
{
    private readonly MetaUIView View;
    private readonly IMinigameManager MinigameManager;
    private readonly MinigameConfigModel Config;
    private readonly IPlayerProgressModel PlayerModel;
    private readonly MinigameLoadingCurtainPresenter LoadingCurtain;
    private readonly GameObjectPool Pool;
    private readonly SceneManager SceneManager;

    private int SelectedIndex;
    private List<Transform> CardIcons = new List<Transform>();
    private List<CharacterCardsData> CardData = new List<CharacterCardsData>();

    [Inject]
    public MetaUIPresenter(MetaUIView view,
        IMinigameManager minigameManager,
        MinigameConfigModel config,
        IPlayerProgressModel playerModel,
        GameObjectPool pool,
        MinigameLoadingCurtainPresenter loadingCurtain, 
        SceneManager sceneManager)
    {
        View = view;
        MinigameManager = minigameManager;
        Config = config;

        PlayerModel = playerModel;
        Pool = pool;
        LoadingCurtain = loadingCurtain;
        SceneManager = sceneManager;
    }

    public void Enable()
    {
        View.Inject(Pool);
        View.Enable();
        CreateButtons();
        SubscribeToEvents();
        UpdateResources();
    }

    private void CreateButtons()
    {
        for (int i = 0; i < Config.GetMinigamesCount(); i++)
        {
            View.AddButton(Config.GetMinigameConfig(i).Name);
        }
        HandleMinigameButtonClicked(0);
    }

    public void Disable()
    {
        View.Disable();
        UnsubscribeFromEvents();

        DisposeIcons();
    }

    private void UpdateResources()
    {
        View.SetResources(PlayerModel.GetResources().GetDiamonds(), PlayerModel.GetResources().GetCash());
        
        DisposeIcons();

        PlayerModel.GetResources().GetCharacters(CardData);

        View.SetCards(CardData);
    }

    private void SubscribeToEvents()
    {
        View.MinigameButtonClicked += HandleMinigameButtonClicked;
        View.CleanPrefsClicked += HandleCleanPrefsClicked;
        View.StartGameButtonClicked += HandleStartGameButtonClicked;
    }

    private void UnsubscribeFromEvents()
    {
        View.MinigameButtonClicked -= HandleMinigameButtonClicked;
        View.CleanPrefsClicked -= HandleCleanPrefsClicked;
        View.StartGameButtonClicked -= HandleStartGameButtonClicked;
    }

    private void HandleMinigameButtonClicked(int index)
    {
        SelectedIndex = index;

        var minigame = Config.GetMinigameConfig(index);
        View.SetMinigameName(minigame.Name);
        View.SetMinigameDescription(minigame.Description);
        
        for (int i = 0; i < Config.GetMinigamesCount(); i++)
        {
            View.SetButtonUnselected(i);
        }
        
        View.SetButtonSelected(index);
    }

    private void HandleCleanPrefsClicked()
    {
        // Clear player preferences
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        
        Debug.Log("Player preferences cleared");
    }

    private void HandleStartGameButtonClicked()
    {
        var minigame = Config.GetMinigameConfig(SelectedIndex);

        var _ = StartGame(minigame.ID);
    }

    private async UniTaskVoid StartGame(Minigames selectedMinigame)
    {
        Disable();

        LoadingCurtain.Enable();

        var loadSuccess = await MinigameManager.LoadMinigame(selectedMinigame);

        LoadingCurtain.Disable();

        if (loadSuccess)
        {
            await MinigameManager.StartMinigame();
        }
        else
        {
            Debug.LogError($"Failed to load minigame: {selectedMinigame}");
        }

        MinigameManager.DeloadMinigame();

        LoadingCurtain.Enable();
        await OpenMainScene();
        LoadingCurtain.Disable();

        Enable();
    }

    private async UniTask OpenMainScene()
    {
        const string mainUiScene = "Assets/Content/Local/Scenes/SampleScene.unity";
        await SceneManager.LoadScene(mainUiScene);
    }

    public void Dispose()
    {
        DisposeIcons();
    }

    private void DisposeIcons()
    {
        foreach(var card in CardIcons)
        {
            Pool.Pop(card.gameObject);
        }
        CardIcons.Clear();
    }
}
