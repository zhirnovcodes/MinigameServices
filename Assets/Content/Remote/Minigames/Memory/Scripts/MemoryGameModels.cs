using UnityEngine;

public class MemoryGameConfigModel
{
    readonly MemoryGameRewardsConfig rewardsConfig;

    public MemoryGameConfigModel(MemoryGameRewardsConfig rewardsConfig)
    {
        this.rewardsConfig = rewardsConfig;
    }

    public MemoryGameRewardData GetRewardData(int index)
    {
        return rewardsConfig.Rewards[index];
    }

    public int GetCount()
    {
        return rewardsConfig.Rewards.Length;
    }
}

public class MemoryGamePenaltiesModel
{
    readonly MemoryGamePenaltiesConfig penaltiesConfig;

    public MemoryGamePenaltiesModel(MemoryGamePenaltiesConfig penaltiesConfig)
    {
        this.penaltiesConfig = penaltiesConfig;
    }

    public MemoryGamePenaltyData GetPenalty(int index)
    {
        return penaltiesConfig.Penalties[index];
    }

    public int GetCount()
    {
        return penaltiesConfig.Penalties.Length;
    }
}


