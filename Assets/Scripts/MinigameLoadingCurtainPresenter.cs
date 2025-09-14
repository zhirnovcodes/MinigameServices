using UnityEngine;
using Zenject;

public class MinigameLoadingCurtainPresenter : ILoadingPercentHandler
{
    private readonly MinigameLoadingCurtainView View;
    private readonly GameObject MainUICanvas;

    [Inject]
    public MinigameLoadingCurtainPresenter(MinigameLoadingCurtainView view, [Inject(Id = "MainUICanvas")] GameObject mainUICanvas)
    {
        View = view;
        MainUICanvas = mainUICanvas;
        
        // Set the view's parent to the MainUICanvas
        View.transform.SetParent(MainUICanvas.transform, false);
    }

    public void Enable()
    {
        View.Enable();
    }

    public void Disable()
    {
        View.Disable();
    }

    public void SetLoadingPercent(float value)
    {
        View.SetLoadingPercent(value);
    }
}
