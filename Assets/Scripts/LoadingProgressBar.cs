using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingProgressBar : MonoBehaviour
{
    public Slider Slider;
    public TextMeshProUGUI Text;

    private void Awake()
    {
        Slider.minValue = 0;
        Slider.maxValue = 100;
    }

    public void SetLoadingPercent(float value)
    {
        Slider.value = value;

        Text.text = value + "%";
    }
}
