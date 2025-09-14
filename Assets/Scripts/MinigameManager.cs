using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class MinigameManager : IMinigameManager
{
    private readonly IResourceLoader ResourceLoader;

    private ILoadingPercentHandler PercentHandler;
    private IMinigameModel MinigameModel;

    private IPlayerProgressModel Progress;

    private MinigameInputDataMemoryPool InputPool;
    private MinigamePlayerDataMemoryPool PlayerDataPool;
    private MinigameRewardMemoryPool RewardPool;
    private MinigamePenaltyMemoryPool PenaltyPool;

    private MinigameInputData Input;
    private MinigameResultData Result;

    private MinigameServices Services;

    [Inject]
    public MinigameManager(
        IResourceLoader resourceLoader,
        ILoadingPercentHandler percentHandler,
        IPlayerProgressModel progress,
        MinigameInputDataMemoryPool inputPool,
        MinigamePlayerDataMemoryPool playerDataPool,
        MinigameRewardMemoryPool rewardPool, 
        MinigamePenaltyMemoryPool penaltyPool,
        MinigameServices services)
    {
        ResourceLoader = resourceLoader;
        PercentHandler = percentHandler;
        Progress = progress;
        InputPool = inputPool;
        PlayerDataPool = playerDataPool;
        Services = services;
        RewardPool = rewardPool;
        PenaltyPool = penaltyPool;

        Result = new MinigameResultData();
    }

    public async UniTask<bool> LoadMinigame(Minigames minigame)
    {
        Debug.Assert(MinigameModel == null, "You forgot to call Deload");

        var loadResult = await ResourceLoader.LoadMinigameScene(minigame, PercentHandler);

        if (loadResult.Status == ResourceLoadStatus.Fail)
        {
            Debug.LogError($"Failed to load minigame scene: {minigame}");
            return false;
        }

        var currentSceneInstance = loadResult.SuccessData;

        await currentSceneInstance.ActivateAsync();

        var scene = currentSceneInstance.Scene;

        MinigameModel = FindModelInScene(scene);

        if (MinigameModel == null)
        {
            Debug.LogError($"No IMinigameModel implementation found in scene: {minigame}");
            return false;
        }

        SubscribeEvents();

        CreateInputData();
        CreateResutData();

        MinigameModel.Init(Input, Result, Services);
        return true;
    }

    private void CreateResutData()
    {
        Result = Result ?? new MinigameResultData();

        Result.Reward = RewardPool.Spawn();
        Result.Penalties = PenaltyPool.Spawn();
    }

    private void CreateInputData()
    {
        Input = InputPool.Spawn();

        var playerData = PlayerDataPool.Spawn();

        playerData.Cash = Progress.GetResources().GetCash();
        playerData.Diamonds = Progress.GetResources().GetDiamonds();

        Progress.GetResources().GetCharacters(playerData.CharacterCards);

        Input.PlayerData.Add(playerData);
    }

    private void DestroyInputData()
    {
        foreach (var data in Input.PlayerData)
        {
            PlayerDataPool.Despawn(data);
        }
        InputPool.Despawn(Input);
    }

    private void DestroyResultData()
    {
        RewardPool.Despawn(Result.Reward);
        PenaltyPool.Despawn(Result.Penalties);

        Result.Reward = null;
        Result.Penalties = null;
    }


    public async UniTask StartMinigame()
    {
        await MinigameModel.Start();
    }


    public void DeloadMinigame()
    {
        if (MinigameModel == null)
        {
            return;
        }

        DestroyInputData();
        DestroyResultData();

        UnsubscribeEvents();

        MinigameModel.Dispose();
        MinigameModel = null;
    }

    private void SubscribeEvents()
    {
        MinigameModel.GameplayFinished += HandleGameplayFinished;
    }

    private void UnsubscribeEvents()
    {
        MinigameModel.GameplayFinished -= HandleGameplayFinished;
    }

    private void HandleGameplayFinished(MinigameResultData data)
    {
        Progress.GetStatistics().AddGamePlayed(data.Status);
        ApplyRewards(data.Reward);
        ApplyPenalties(data.Penalties);
        Progress.Save();
    }

    private void ApplyRewards(MinigameRewardData rewards)
    {
        Progress.GetResources().AddCash(rewards.Cash);
        Progress.GetResources().AddDiamonds(rewards.Diamonds);

        foreach (var card in rewards.CharacterCards)
        {
            Progress.GetResources().AddCharacterCard(card.ID, card.Count);
        }
    }

    private void ApplyPenalties(MinigamePenaltiesData penalties)
    {
        foreach (var penalty in penalties.Penalties)
        {
            switch (penalty.Penalty)
            {
                case Penalties.Cash:
                {
                        // TODO remove actually part
                    int removeCount = 
                        penalty.Operation == PenaltyOperations.Remove ?
                            penalty.Amount :
                            Mathf.FloorToInt(Progress.GetResources().GetCash() / (float)penalty.Amount);
                    Progress.GetResources().RemoveCash(removeCount);
                    break;
                }
                case Penalties.Diamonds:
                {
                    int removeCount =
                    penalty.Operation == PenaltyOperations.Remove ?
                        penalty.Amount :
                        Mathf.FloorToInt(Progress.GetResources().GetDiamonds() / (float)penalty.Amount);
                    Progress.GetResources().RemoveDiamonds(removeCount);
                    break;
                }
            }
        }
    }

    private IMinigameModel FindModelInScene(Scene scene)
    {
        var rootObjects = scene.GetRootGameObjects();
        
        foreach (var rootObject in rootObjects)
        {
            var minigameModel = rootObject.GetComponent<IMinigameModel>();
            if (minigameModel != null)
            {
                return minigameModel;
            }
            
            // Also check in children
            minigameModel = rootObject.GetComponentInChildren<IMinigameModel>();
            if (minigameModel != null)
            {
                return minigameModel;
            }
        }
        
        return null;
    }
}
