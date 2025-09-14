using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class SelectableButton : MonoBehaviour
{
    [Header("Button States")]
    public Button Selected;
    public Button Unselected;
    public TextMeshProUGUI Text;

    public event Action OnClicked = () => { };

    private void Awake()
    {
        Selected.onClick.AddListener(HandleClick);
        
        Unselected.onClick.AddListener(HandleClick);
    }

    private void OnDestroy()
    {
        // Unsubscribe from button clicks
        Selected.onClick.RemoveListener(HandleClick);
        
        Unselected.onClick.RemoveListener(HandleClick);
    }

    public void SetSelected()
    {
        Selected.gameObject.SetActive(true);
        
        Unselected.gameObject.SetActive(false);
    }

    public void SetUnselected()
    {
        Selected.gameObject.SetActive(false);
        
        Unselected.gameObject.SetActive(true);
    }

    public void SetText(string text)
    {
        Text.text = text;
    }

    private void HandleClick()
    {
        OnClicked?.Invoke();
    }
}
