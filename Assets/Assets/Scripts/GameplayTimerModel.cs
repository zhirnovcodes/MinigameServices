using System;
using UnityEngine;

public interface IGameplayTimerModel
{
    event Action TimerTicked;
    void SetDuration(float duration);
    bool GetIsTimeUp();
    void Run();
}

public class GameplayTimerModel : MonoBehaviour, IGameplayTimerModel
{
	public event Action TimerTicked = () => {};

	public float DurationSeconds = 60f;

	private float LastTickTime;

    private float TimeElapsed;

	public void Run()
	{
        TimeElapsed = 0;
        this.enabled = true;
	}

    public void SetDuration(float duration)
    {
        DurationSeconds = duration;
    }

	public bool GetIsTimeUp()
	{
		return TimeElapsed >= DurationSeconds;
	}

	private void Update()
	{
        if (TimeElapsed - LastTickTime >= 1f)
        {
            TimerTicked();
            LastTickTime = TimeElapsed;
        }

		if (GetIsTimeUp())
		{
			this.enabled = false;
            return;
		}

        TimeElapsed += Time.deltaTime;
	}
}


