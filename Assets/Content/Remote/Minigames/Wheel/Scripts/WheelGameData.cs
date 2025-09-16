[System.Serializable]
public struct WheelGameRewardData
{
	public int Cash;
	public int Diamonds;
	public CharacterCardsData Cards;
	public Prefabs Icon;
}

[System.Serializable]
public struct WheelGamePenaltyData
{
	public WheelGamePenaltyType Penalty;
	public WheelGamePenaltyResource Resource;
	public int Amount;
	public Prefabs Icon;
}

[System.Serializable]
public struct WheelGameResultData
{
	public bool IsSuccess;
	public WheelGamePenaltyData Penalty;
	public WheelGameRewardData Reward;
}
