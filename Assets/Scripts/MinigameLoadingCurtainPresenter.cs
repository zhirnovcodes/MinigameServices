using Zenject;

public class MinigameLoadingCurtainPresenter : ILoadingPercentHandler
{
    private readonly MinigameLoadingCurtainView View;

    [Inject]
    public MinigameLoadingCurtainPresenter(MinigameLoadingCurtainView view)
    {
        View = view;
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
