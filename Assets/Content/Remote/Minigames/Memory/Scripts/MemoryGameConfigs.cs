using UnityEngine;

[CreateAssetMenu(menuName = "Minigames/Memory/Rewards Config", fileName = "RewardsConfig")]
public class MemoryGameRewardsConfig : ScriptableObject
{
    public MemoryGameRewardData[] Rewards;
}

[CreateAssetMenu(menuName = "Minigames/Memory/Penalties Config", fileName = "PenaltiesConfig")]
public class MemoryGamePenaltiesConfig : ScriptableObject
{
    public MemoryGamePenaltyData[] Penalties;
}


