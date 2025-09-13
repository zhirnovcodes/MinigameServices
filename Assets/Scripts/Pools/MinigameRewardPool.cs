using System.Collections.Generic;
using Zenject;

public class MinigameRewardMemoryPool : MemoryPool<MinigameRewardData>
{
    protected override void OnSpawned(MinigameRewardData item)
    {
        ResetRewardData(item);
    }

    protected override void OnDespawned(MinigameRewardData item)
    {
        ResetRewardData(item);
    }

    protected override void OnCreated(MinigameRewardData item)
    {
        item.CharacterCards = new List<CharacterCards>();
    }

    private void ResetRewardData(MinigameRewardData item)
    {
        item.Cash = 0;
        item.Diamonds = 0;
        item.CharacterCards.Clear();
    }
}
