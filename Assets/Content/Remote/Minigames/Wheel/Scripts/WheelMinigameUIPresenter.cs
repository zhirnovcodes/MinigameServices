public class WheelMinigameUIPresenter
{
	private WheelMinigameWaitingView WaitingView;
	private WinUIView WinView;
	private LooseUIView LoseView;

	public WheelMinigameUIPresenter(WheelMinigameWaitingView waitingView, WinUIView winView, LooseUIView loseView)
	{
		WaitingView = waitingView;
		WinView = winView;
		LoseView = loseView;
	}

	public void EnableWaitingView()
	{
		LoseView.Disable();
		WinView.Disable();
		WaitingView.Enable();
		WaitingView.StartAnimation();
	}

	public void DisableWaitingView()
	{
		WaitingView.Disable();
	}

	public void EnableFinishView(WheelGameResultData result)
	{
		bool hasReward = result.IsSuccess;

		if (hasReward)
		{
			WinView.Enable();
			WinView.SetWinData(result.Reward);
			return;
		}

		LoseView.Enable();
		LoseView.SetLoseData(result.Penalty);
	}

	public void DisableFinishView()
	{
		WinView.Disable();
		LoseView.Disable();
	}
}


