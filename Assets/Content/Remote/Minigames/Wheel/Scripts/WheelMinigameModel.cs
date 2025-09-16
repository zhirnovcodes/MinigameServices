using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class WheelMinigameModel : MonoBehaviour, IMinigameModel
{
    public event Action<MinigameResultData> GameplayFinished = (data) => { };

    private MinigameResultData ResultData;

	[SerializeField] private WheelMinigameStateMachine StateMachine;
	[SerializeField] private WheelMinigameWaitingView WaitingView;
	[SerializeField] private WinUIView WinView;
	[SerializeField] private LooseUIView LoseView;
	[SerializeField] private WheelManualRotationModel ManualRotationModel;
	[SerializeField] private WheelMotorModel MotorModel;
	[SerializeField] private WheelModel WheelModel;
	[SerializeField] private WheelCalibratorModel CalibratorModel;
	[SerializeField] private WheelTouchHandler Handler;

    private WheelGameRewardModel Rewards;
    private WheelGamePenaltiesModel Penalties;

    private IMinigameServices Services;

    private WheelGameResultData[] WheelResultData;

    private void ConstructStateMachine()
	{
		var uiPresenter = new WheelMinigameUIPresenter(WaitingView, WinView, LoseView);
		StateMachine.Construct(
			uiPresenter,
			ManualRotationModel,
			WheelModel,
			MotorModel,
			CalibratorModel,
            Handler);
	}

    public async UniTask Init(MinigameInputData data, MinigameResultData resultData, IMinigameServices services)
    {
        Services = services;

        ResultData = resultData;

        await services.GetMinigamePool().PreloadPrefabs<Prefabs>();

        await LoadConfig(services);

        ConstructWheel(services);

		ConstructStateMachine();
    }

    private async UniTask LoadConfig(IMinigameServices services)
    {
        var rev = await services.GetConfigLoader().LoadConfig(Configs.RewardsConfig);
        var pen = await services.GetConfigLoader().LoadConfig(Configs.PenaltiesConfig);

        Rewards = new WheelGameRewardModel(rev as WheelGameRewardsConfig);
        Penalties = new WheelGamePenaltiesModel(pen as WheelGamePenaltiesConfig);
    }

    private void ConstructWheel(IMinigameServices services)
    {
        var rewardChancePercent = 0.8f;

        var sectionsCount = WheelModel.GetSectionsCount();

        WheelResultData = new WheelGameResultData[sectionsCount];

        for (int i = 0; i < sectionsCount; i++)
        {
            var t = WheelModel.GetPlaceholderTransform(i);
            var isReward = UnityEngine.Random.value < rewardChancePercent;

            if (isReward)
            {
                var reward = Rewards.GetRandomRewardData();
                var iconRew = services.GetMinigamePool().Pool(reward.Icon);

                iconRew.transform.SetParent(t, false);

                WheelResultData[i] = new WheelGameResultData
                {
                    IsSuccess = true,
                    Reward = reward
                };
                continue;
            }

            var penalty = Penalties.GetRandomPenalty();

            var icon = services.GetMinigamePool().Pool(penalty.Icon);

            icon.transform.SetParent(t, false);

            WheelResultData[i] = new WheelGameResultData
            {
                IsSuccess = false,
                Penalty = penalty
            };
        }
    }

    public async UniTask StartGame()
    {
        await StateMachine.Play();

        Debug.Log(WheelModel.GetCurrentSectionId());

        var result = WheelResultData[WheelModel.GetCurrentSectionId()];
		if (result.IsSuccess)
		{
			ResultData.Status = MinigameStatuses.Success;
			ResultData.Reward.Cash += result.Reward.Cash;
			ResultData.Reward.Diamonds += result.Reward.Diamonds;
			if (result.Reward.Cards.Count > 0)
			{
				ResultData.Reward.CharacterCards.Add(result.Reward.Cards);
			}
		}
		else
		{
			ResultData.Status = MinigameStatuses.Fail;
			if (result.Penalty.Penalty != WheelGamePenaltyType.Nothing)
			{
				var penaltyData = new MinigamePenaltyData
				{
					Penalty = result.Penalty.Resource == WheelGamePenaltyResource.Cash ? Penalties.Cash : Penalties.Diamonds,
					Operation = result.Penalty.Penalty == WheelGamePenaltyType.LoseAmount ? PenaltyOperations.Remove : PenaltyOperations.Divide,
					Amount = result.Penalty.Penalty == WheelGamePenaltyType.LoseHalf ? 2 : (result.Penalty.Penalty == WheelGamePenaltyType.LoseAll ? 1 : result.Penalty.Amount)
				};
				ResultData.Penalties.Penalties.Add(penaltyData);
			}
		}
		GameplayFinished(ResultData);
        /*
        // Wait 2 seconds at the beginning
        await UniTask.Delay(2000);

        ResultData.Reward.Cash = 10;
        ResultData.Reward.Diamonds = 1;
        ResultData.Reward.CharacterCards.Add(new CharacterCardsData
        {
            ID = CharacterCards.Cat,
            Count = 1
        });
        GameplayFinished(ResultData);
        
        // Wait 2 seconds at the end
        await UniTask.Delay(2000);
        */
    }

    public void Dispose()
    {
        ResultData = null;
        Services.GetMinigamePool().Dispose();
            }
}
