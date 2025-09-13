using System.Collections.Generic;
using Zenject;

public class MinigamePenaltyMemoryPool : MemoryPool<MinigamePenaltiesData>
{
    protected override void OnSpawned(MinigamePenaltiesData item)
    {
        item.Penalties.Clear();
    }

    protected override void OnDespawned(MinigamePenaltiesData item)
    {
        item.Penalties.Clear();
    }

    protected override void OnCreated(MinigamePenaltiesData item)
    {
        // Initialize with default values
        item.Penalties = new List<MinigamePenaltyData>();
    }
}
