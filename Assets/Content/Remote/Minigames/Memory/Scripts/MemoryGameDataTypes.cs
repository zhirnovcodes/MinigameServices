using System.Collections.Generic;

[System.Serializable]
public struct BlockData
{
    public int Index;
    public int RewardIndex;
    public MemoryGameResultData Content;
}

public class GameplayData
{
    public int? SelectedId1;
    public int? SelectedId2;
}


