[System.Serializable]
public struct WheelGameRewardData
{
	public int Cash;
	public int Diamonds;
	public CharacterCardsData Cards;
}

[System.Serializable]
public struct WheelGamePenaltyData
{
	public WheelGamePenaltyType Penalty;
	public WheelGamePenaltyResource Resource;
	public int Amount;
}


