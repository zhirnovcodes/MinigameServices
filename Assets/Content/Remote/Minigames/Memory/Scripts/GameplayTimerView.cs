using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayTimerView : MonoBehaviour
{
    [SerializeField] private GameObject Root;
    [SerializeField] private TextMeshProUGUI TimerText;
    [SerializeField] private Color NormalColor = Color.white;
    [SerializeField] private Color AttentionColor = Color.red;
    
    public void SetTime(int seconds)
    {
        TimerText.text = seconds.ToString();
    }
    
    public void SetColor(TimeColor timeColor)
    {
        switch (timeColor)
        {
            case TimeColor.Normal:
                TimerText.color = NormalColor;
                break;
            case TimeColor.Attention:
                TimerText.color = AttentionColor;
                break;
        }
    }
    
    public void Enable()
    {
        Root.SetActive(true);
    }
    
    public void Disable()
    {
        Root.SetActive(false);
    }
}
