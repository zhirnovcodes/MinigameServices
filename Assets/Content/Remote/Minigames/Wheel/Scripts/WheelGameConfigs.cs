using UnityEngine;

[CreateAssetMenu(menuName = "Minigames/Wheel/Rewards Config", fileName = "WheelGameRewardsConfig")]
public class WheelGameRewardsConfig : ScriptableObject
{
	public WheelGameRewardData[] Rewards;
}

[CreateAssetMenu(menuName = "Minigames/Wheel/Penalties Config", fileName = "WheelGamePenaltiesConfig")]
public class WheelGamePenaltiesConfig : ScriptableObject
{
	public WheelGamePenaltyData[] Penalties;
}


