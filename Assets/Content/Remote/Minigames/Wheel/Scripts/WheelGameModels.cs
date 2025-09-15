using UnityEngine;

public class WheelGameRewardModel
{
	readonly WheelGameRewardsConfig rewardsConfig;

	public WheelGameRewardModel(WheelGameRewardsConfig rewardsConfig)
	{
		this.rewardsConfig = rewardsConfig;
	}

	public WheelGameRewardData GetRandomRewardData()
	{
		int index = Random.Range(0, rewardsConfig.Rewards.Length);
		return rewardsConfig.Rewards[index];
	}
}

public class WheelGamePenaltiesModel
{
	readonly WheelGamePenaltiesConfig penaltiesConfig;

	public WheelGamePenaltiesModel(WheelGamePenaltiesConfig penaltiesConfig)
	{
		this.penaltiesConfig = penaltiesConfig;
	}

	public WheelGamePenaltyData GetRandomPenalty()
	{
		int index = Random.Range(0, penaltiesConfig.Penalties.Length);
		return penaltiesConfig.Penalties[index];
	}
}


