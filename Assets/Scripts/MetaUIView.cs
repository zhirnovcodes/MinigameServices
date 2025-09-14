using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class MetaUIView : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject Content;
    public LayoutGroup LayoutGroup;
    public Transform CharacterCardLayout;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI DescriptionText;
    public Button StartButton;
    public Button ClearButton;
    public TextMeshProUGUI DiamondsCount;
    public TextMeshProUGUI CashCount;


    [Header("Button Prefab")]
    public SelectableButton ButtonPrefab;
    public CharacterCardMiniView CardPrefab;

    public event Action<int> MinigameButtonClicked = (index) => { };
    public event Action CleanPrefsClicked = () => { };
    public event Action StartGameButtonClicked = () => { };

    private List<SelectableButton> MinigameButtons = new List<SelectableButton>();

    private void Awake()
    {
        // Subscribe to start button click
        StartButton.onClick.AddListener(HandleStartGameClicked);
        ClearButton.onClick.AddListener(HandleCleanPrefsClicked);
    }

    private void OnDestroy()
    {
        // Unsubscribe from start button click
        StartButton.onClick.RemoveListener(HandleStartGameClicked);
        ClearButton.onClick.RemoveListener(HandleCleanPrefsClicked);

        // Clean up minigame buttons
        foreach (var button in MinigameButtons)
        {
            GameObject.Destroy(button);
        }

        MinigameButtons.Clear();
    }

    public int AddButton(string name)
    {
        // Create new button instance
        var buttonInstanceGameObject = Instantiate(ButtonPrefab.gameObject, LayoutGroup.transform, false);
        var buttonInstance = buttonInstanceGameObject.GetComponent<SelectableButton>();
        buttonInstance.name = $"MinigameButton_{name}";
        
        // Set button text if available
        buttonInstance.SetText(name);

        // Subscribe to button click
        int buttonIndex = MinigameButtons.Count;
        buttonInstance.OnClicked += () => HandleMinigameButtonClicked(buttonIndex);
        
        MinigameButtons.Add(buttonInstance);
        
        return buttonIndex;
    }

    public void SetResources(int diamonds, int cash)
    {
        DiamondsCount.text = diamonds.ToString();
        CashCount.text = cash.ToString();
    }

    public void SetCards(List<CharacterCardsData> characterCards, List<Transform> cardsImages)
    {
        for (int i = 0; i < characterCards.Count; i++)
        {
            var inst = Instantiate(CardPrefab.gameObject);
            var card = inst.GetComponent<CharacterCardMiniView>();

            card.SetParent(CharacterCardLayout);
            card.SetCount(characterCards[i].Count);
            cardsImages[i].SetParent(card.GetPlaceholder(), false);
        }
    }

    public void SetMinigameName(string name)
    {
        NameText.text = name;
    }

    public void SetMinigameDescription(string description)
    {
        DescriptionText.text = description;
    }

    public void SetButtonSelected(int index)
    {
        MinigameButtons[index].SetSelected();
    }

    public void SetButtonUnselected(int index)
    {
        MinigameButtons[index].SetUnselected();
    }

    public void Enable()
    {
        Content.SetActive(true);
    }

    public void Disable()
    {
        Content.SetActive(false);
    }

    private void HandleMinigameButtonClicked(int index)
    {
        MinigameButtonClicked.Invoke(index);
    }

    private void HandleStartGameClicked()
    {
        StartGameButtonClicked.Invoke();
    }

    public void HandleCleanPrefsClicked()
    {
        CleanPrefsClicked.Invoke();
    }
}


