using UnityEngine;
using Zenject;

public class MinigameLoadingCurtainView : MonoBehaviour
{
    public GameObject Content;
    public LoadingProgressBar Bar;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void Enable()
    {
        Content.SetActive(true);
    }

    public void Disable()
    {
        Content.SetActive(false);
    }

    public void SetLoadingPercent(float value)
    {
        Bar.SetLoadingPercent(value);
    }
}