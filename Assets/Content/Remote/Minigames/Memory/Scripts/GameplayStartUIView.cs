using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayStartUIView : MonoBehaviour
{
    [SerializeField] private GameObject StartUI;
    [SerializeField] private TextMeshProUGUI CountdownText;
    [SerializeField] private TextMeshProUGUI GoText;
    
    public void Show3()
    {
        CountdownText.text = "3";
        CountdownText.gameObject.SetActive(true);
        GoText.gameObject.SetActive(false);
    }
    
    public void Show2()
    {
        CountdownText.text = "2";
        CountdownText.gameObject.SetActive(true);
        GoText.gameObject.SetActive(false);
    }
    
    public void Show1()
    {
        CountdownText.text = "1";
        CountdownText.gameObject.SetActive(true);
        GoText.gameObject.SetActive(false);
    }
    
    public void ShowGo()
    {
        CountdownText.gameObject.SetActive(false);
        GoText.gameObject.SetActive(true);
    }
    
    public void Enable()
    {
        StartUI.SetActive(true);
    }
    
    public void Disable()
    {
        StartUI.SetActive(false);
    }
}
