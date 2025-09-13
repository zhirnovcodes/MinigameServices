using System.Collections.Generic;
using Zenject;

public class MinigameInputDataMemoryPool : MemoryPool<MinigameInputData>
{
    protected override void OnSpawned(MinigameInputData item)
    {
        item.PlayerData.Clear();
    }

    protected override void OnDespawned(MinigameInputData item)
    {
        item.PlayerData.Clear();
    }

    protected override void OnCreated(MinigameInputData item)
    {
        // Initialize with default values
        item.PlayerData = new List<MinigamePlayerData>();
    }
}
