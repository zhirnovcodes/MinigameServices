using System.Collections.Generic;

[System.Serializable]
public class MinigameConfig
{
    public Minigames ID;
    public string Name;
    public string Description;
}

public class MinigameResultData
{
    public MinigameStatuses Status;
    public MinigameRewardData Reward;
    public MinigamePenaltiesData Penalties;
}

public class MinigameInputData
{
    public List<MinigamePlayerData> PlayerData;
}

public class MinigamePlayerData
{
    public string Name;
    public int Cash;
    public int Diamonds;
    public List<CharacterCards> CharacterCards;
}

public class MinigameRewardData
{
    public int Cash;
    public int Diamonds;
    public List<CharacterCards> CharacterCards;
}

public class MinigamePenaltiesData
{
    public List<MinigamePenaltyData> Penalties;
}

public struct MinigamePenaltyData
{
    public Penalties Penalty;
    public PenaltyOperations Operation;
    public int Amount;
}

public struct MinigameErrorData
{
    public string Error;
}
