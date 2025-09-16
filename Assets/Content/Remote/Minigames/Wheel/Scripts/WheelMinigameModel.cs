using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using Content.Local.Prefabs;

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

    private WheelGameRewardModel RewardsModel;
    private WheelGamePenaltiesModel PenaltiesModel;

    private IMinigameServices Services;

    private WheelGameResultData[] WheelResultData;
    private WheelMinigameUIPresenter Presenter;

    private void ConstructStateMachine()
	{
		StateMachine.Construct(
            Presenter,
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

        Presenter = new WheelMinigameUIPresenter(WaitingView, WinView, LoseView);

        await services.GetMinigamePool().PreloadPrefabs<Prefabs>();

        await LoadConfig(services);

        ConstructWheel(services);

		ConstructStateMachine();
    }

    private async UniTask LoadConfig(IMinigameServices services)
    {
        var rev = await services.GetConfigLoader().LoadConfig(Configs.RewardsConfig);
        var pen = await services.GetConfigLoader().LoadConfig(Configs.PenaltiesConfig);

        RewardsModel = new WheelGameRewardModel(rev as WheelGameRewardsConfig);
        PenaltiesModel = new WheelGamePenaltiesModel(pen as WheelGamePenaltiesConfig);
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
                var reward = RewardsModel.GetRandomRewardData();
                var iconRew = services.GetMinigamePool().Pool(reward.Icon);

                iconRew.transform.SetParent(t, false);

                WheelResultData[i] = new WheelGameResultData
                {
                    IsSuccess = true,
                    Reward = reward
                };
                continue;
            }

            var penalty = PenaltiesModel.GetRandomPenalty();

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

        Presenter.EnableFinishView(result);

        if (result.IsSuccess)
        {
            var pos = Vector3.zero;
			await UniTask.WhenAll(
				ShowRewardAnimation(pos, result.Reward),
				PlayRewardParticles(pos, result.Reward)
			);
        }
    }


    private async UniTask ShowRewardAnimation(Vector3 startPosition, WheelGameRewardData result)
    {
		var pool = Services.GetCommonObjectPool();
		GameObject effect = null;

		if (result.Diamonds > 0)
		{
			effect = pool.Pool(Effects.GameFinish.Res.Diamond);
            effect.transform.position = startPosition;
            await effect.GetComponent<WinRewardAnimation>().PlayAnimation();
		}
		else if (result.Cash > 0)
		{
			effect = pool.Pool(Effects.GameFinish.Res.Cash);
            startPosition.y = 0.5f;
            effect.transform.position = startPosition;
            await effect.GetComponent<WinRewardAnimation>().PlayAnimation();
        }
		else if (result.Cards.Count > 0)
		{
			var firstCard = result.Cards.ID;
			switch (firstCard)
			{
				case CharacterCards.Cat:
					effect = pool.Pool(Effects.GameFinish.CharacterCards.Cat);
                    effect.transform.position = startPosition + Vector3.up;
                    await effect.GetComponent<WinCardAnimation>().PlayAnimation();
                    break;
				case CharacterCards.Squid:
					effect = pool.Pool(Effects.GameFinish.CharacterCards.Squid);
                    effect.transform.position = startPosition + Vector3.up;
                    await effect.GetComponent<WinCardAnimation>().PlayAnimation();
                    break;
			}
		}
    }

    private async UniTask PlayRewardParticles(Vector3 position, WheelGameRewardData result)
    {
        var pool = Services.GetCommonObjectPool();
        GameObject go = null;

        if (result.Diamonds > 0)
        {
            go = pool.Pool(Effects.Particles.DiamondsRain);
        }
        else if (result.Cash > 0)
        {
            go = pool.Pool(Effects.Particles.CashFlow);
        }
        else if (result.Cards.Count > 0)
        {
            go = pool.Pool(Effects.Particles.Victory);
        }

        if (go == null)
        {
            return;
        }

        go.transform.position = position;
        var pe = go.GetComponent<ParticlesEffect>();
        if (pe != null)
        {
            pe.Enable();
            pe.Play();
            await UniTask.Delay(System.TimeSpan.FromSeconds(pe.GetDuration()));
            pe.Disable();
        }
    }

    public void Dispose()
    {
        ResultData = null;
        Services.GetMinigamePool().Dispose();
    }
}
