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
    private readonly IPlayerResourcesModel Resources;
    private readonly IPrefabLibrary PrefabLibrary;

    private int SelectedIndex;
    private List<Transform> CardIcons = new List<Transform>();
    private List<CharacterCardsData> CardData = new List<CharacterCardsData>();

    [Inject]
    public MetaUIPresenter(MetaUIView view,
        IMinigameManager minigameManager,
        MinigameConfigModel config,
        IPlayerResourcesModel resources, 
        IPrefabLibrary prefabLibrary)
    {
        View = view;
        MinigameManager = minigameManager;
        Config = config;

        CreateButtons();

        HandleMinigameButtonClicked(0);
        Resources = resources;
        PrefabLibrary = prefabLibrary;
    }

    private void CreateButtons()
    {
        for (int i = 0; i < Config.GetMinigamesCount(); i++)
        {
            View.AddButton(Config.GetMinigameConfig(i).Name);
        }
    }

    public void Enable()
    {
        View.Enable();
        SubscribeToEvents();
        UpdateResources();
    }

    private void UpdateResources()
    {
        View.SetResources(Resources.GetDiamonds(), Resources.GetCash());
        DisposeIcons();

        CardData.Clear();
        Resources.GetCharacters(CardData);

        foreach (var card in CardData)
        {
            var inst = PrefabLibrary.InstantiatePrefab(GetPrefabID(card.ID));
            var tr = inst.GetComponent<Transform>();
            CardIcons.Add(tr);
        }

        View.SetCards(CardData, CardIcons);
    }

    private Content.Local.Prefabs.CharacterCards.UI.S128 GetPrefabID(CharacterCards characterCard)
    {
        // TODO another way
        switch (characterCard)
        {
            case CharacterCards.Cat:
                {
                    return Content.Local.Prefabs.CharacterCards.UI.S128.Cat128;
                }
            case CharacterCards.Squid:
                {
                    return Content.Local.Prefabs.CharacterCards.UI.S128.Squid128;
                }
        }

        throw new NotImplementedException();
    }

    public void Disable()
    {
        View.Disable();
        UnsubscribeFromEvents();
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

        var s = StartGame(minigame.ID);
    }

    private async UniTaskVoid StartGame(Minigames selectedMinigame)
    {
        var loadSuccess = await MinigameManager.LoadMinigame(selectedMinigame);

        if (loadSuccess)
        {
            await MinigameManager.StartMinigame();
        }
        else
        {
            Debug.LogError($"Failed to load minigame: {selectedMinigame}");
        }
    }

    public void Dispose()
    {
        DisposeIcons();
    }

    private void DisposeIcons()
    {
        foreach(var card in CardIcons)
        {
            GameObject.Destroy(card.gameObject);
        }
        CardIcons.Clear();
    }
}
