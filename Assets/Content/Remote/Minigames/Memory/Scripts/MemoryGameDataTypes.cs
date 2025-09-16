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
    public int? SelectedIndex1;
    public int? SelectedIndex2;
    public List<BlockData> Blocks = new List<BlockData>();
}


