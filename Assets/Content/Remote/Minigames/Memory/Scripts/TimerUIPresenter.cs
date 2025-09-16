using UnityEngine;

public class TimerUIPresenter : ITimerUIPresenter
{

    private GameplayTimerView TimerView;
    private IGameplayTimerModel Model;

    public TimerUIPresenter(GameplayTimerView timerView, IGameplayTimerModel model)
    {
        TimerView = timerView;
        Model = model;
    }

    public void Enable()
    {
        TimerView.Enable();

        Model.TimerTicked += OnTimerTicked;
    }

    public void Disable()
    {
        TimerView.Disable();

        Model.TimerTicked -= OnTimerTicked;
    }

    private void OnTimerTicked()
    {
        TimerView.SetTime(Mathf.FloorToInt(Model.GetTime()));
        if (Model.GetTime() <= 3)
        {
            TimerView.SetColor(TimeColor.Attention);
        }
        else
        {
            TimerView.SetColor(TimeColor.Normal);
        }
    }
}
