public class MemoryGameConfigModel
{
    readonly MemoryGameRewardsConfig rewardsConfig;
    readonly MemoryGamePenaltiesConfig penaltiesConfig;

    public MemoryGameConfigModel(MemoryGameRewardsConfig rewardsConfig, MemoryGamePenaltiesConfig penaltiesConfig) 
    {
        this.rewardsConfig = rewardsConfig;
        this.penaltiesConfig = penaltiesConfig;
    }

    public MemoryGameRewardData GetRewardData(int index)
    {
        return rewardsConfig.Rewards[index];
    }

    public int GetRewardsCount()
    {
        return rewardsConfig.Rewards.Length;
    }

    public MemoryGamePenaltyData GetPenalty(int index)
    {
        return penaltiesConfig.Penalties[index];
    }

    public int GetPenaltiesCount()
    {
        return penaltiesConfig.Penalties.Length;
    }
}


