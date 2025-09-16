[System.Serializable]
public struct MemoryGameRewardData
{
    public int Cash;
    public int Diamonds;
    public CharacterCardsData Cards;
}

[System.Serializable]
public struct MemoryGamePenaltyData
{
    public MemoryGamePenaltyType Penalty;
    public MemoryGamePenaltyResource Resource;
    public int Amount;
}

[System.Serializable]
public struct MemoryGameResultData
{
    public bool IsSuccess;
    public bool IsTimeout;
    public MemoryGameRewardData Reward;
    public MemoryGamePenaltyData Penalty;
}


