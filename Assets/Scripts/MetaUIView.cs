using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class MetaUIView : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject ContentRoot;
    public LayoutGroup LayoutGroup;
    public Transform CharacterCardLayout;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI DescriptionText;
    public Button StartButton;
    public Button ClearButton;
    public TextMeshProUGUI DiamondsCount;
    public TextMeshProUGUI CashCount;

    private GameObjectPool Pool;

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

        Disable();
    }

    public void Inject(GameObjectPool pool)
    {
        Pool = pool;
    }

    public int AddButton(string name)
    {
        // Create new button instance
        var buttonId = Content.Local.Prefabs.UI.Elements.SelectableButton;
        var buttonInstanceGameObject = Pool.Pool(buttonId);
        buttonInstanceGameObject.transform.SetParent(LayoutGroup.transform, false);
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

    public void SetCards(List<CharacterCardsData> characterCards)
    {
        // Clear any existing cards first to avoid duplicates and leaks
        DisposeCards();

        for (int i = 0; i < characterCards.Count; i++)
        {
            var cardData = characterCards[i];

            // Create card container (CharacterCardMiniView) via pool and parent it under CharacterCardLayout
            var cardContainerGo = Pool.Pool(Content.Local.Prefabs.UI.Elements.CharacterCardMini);
            cardContainerGo.transform.SetParent(CharacterCardLayout, false);

            var cardView = cardContainerGo.GetComponent<CharacterCardMiniView>();

            // Set count on the card view
            cardView.SetCount(cardData.Count);

            // Create the image for this card and parent it into the card's placeholder
            var imagePrefabId = GetCardImageId(cardData.ID);
            var imageGo = Pool.Pool(imagePrefabId);
            imageGo.transform.SetParent(cardView.GetPlaceholder(), false);
        }
    }


    private Content.Local.Prefabs.CharacterCards.UI.S128 GetCardImageId(CharacterCards characterCard)
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
        ContentRoot.SetActive(true);
    }

    public void Disable()
    {
        ContentRoot.SetActive(false);
        DisposeCards();
        DisposeButtons();
        Pool = null;
    }

    private void DisposeButtons()
    {
        // Return all created minigame buttons to the pool and clear the list
        for (int i = 0; i < MinigameButtons.Count; i++)
        {
            var button = MinigameButtons[i];
            Pool.Pop(button.gameObject);
        }

        MinigameButtons.Clear();
    }

    private void DisposeCards()
    {
        // Iterate through all card containers under CharacterCardLayout and return their children and themselves to the pool
        for (int i = CharacterCardLayout.childCount - 1; i >= 0; i--)
        {
            var cardContainer = CharacterCardLayout.GetChild(i);
            var cardView = cardContainer.GetComponent<CharacterCardMiniView>();

            // First, return any images inside the placeholder
            if (cardView != null)
            {
                var placeholder = cardView.GetPlaceholder();
                for (int j = placeholder.childCount - 1; j >= 0; j--)
                {
                    var imageChild = placeholder.GetChild(j);
                    imageChild.SetParent(null, false);
                    Pool.Pop(imageChild.gameObject);
                    break;
                }
            }

            // Then, return the card container itself
            cardContainer.SetParent(null, false);
            Pool.Pop(cardContainer.gameObject);
        }
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


