using Cysharp.Threading.Tasks;
using System;
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
    public MinigamePenaltiesData PenaltiesData;
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
    public int Count;
}

public struct MinigameErrorData
{
    public string Error;
}

public interface IMinigameModel : IDisposable
{
    event Action<MinigameResultData> Finished;
    event Action<MinigameErrorData> Faulted;
    event Action Closed;

    void Init(MinigameInputData data);
    void Start();
}

public interface IMinigameManager
{
    UniTaskVoid LoadMinigame(Minigames minigame, MinigameInputData input);
    UniTaskVoid StartMinigame();
    void DeloadMinigame();
}

[System.Serializable]
public class MinigamesListConfig
{
    public List<MinigameConfig> Configs;
}