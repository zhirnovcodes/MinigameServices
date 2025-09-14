using System.Collections.Generic;
using Zenject;

public class MinigamePlayerDataMemoryPool : MemoryPool<MinigamePlayerData>
{
    protected override void OnSpawned(MinigamePlayerData item)
    {
        item.CharacterCards.Clear();
    }

    protected override void OnDespawned(MinigamePlayerData item)
    {
        item.CharacterCards.Clear();
    }

    protected override void OnCreated(MinigamePlayerData item)
    {
        // Initialize with default values
        item.CharacterCards = new List<CharacterCardsData>();
    }
}
