using UnityEngine;

[CreateAssetMenu(menuName = "Minigames/Wheel/Rewards Config", fileName = "RewardsConfig")]
public class WheelGameRewardsConfig : ScriptableObject
{
	public WheelGameRewardData[] Rewards;
}

[CreateAssetMenu(menuName = "Minigames/Wheel/Penalties Config", fileName = "PenaltiesConfig")]
public class WheelGamePenaltiesConfig : ScriptableObject
{
	public WheelGamePenaltyData[] Penalties;
}


