using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.ResourceProviders;
using Zenject;

public class MinigameManager : IMinigameManager
{
    private readonly SceneManager SceneManager;

    private ILoadingPercentHandler PercentHandler;
    private IMinigameModel MinigameModel;

    private IPlayerProgressModel Progress;

    private MinigameInputDataMemoryPool InputPool;
    private MinigamePlayerDataMemoryPool PlayerDataPool;
    private MinigameRewardMemoryPool RewardPool;
    private MinigamePenaltyMemoryPool PenaltyPool;

    private MinigameInputData Input;
    private MinigameResultData Result;

    private IMinigameServicesController Services;
    
    private object CurrentSceneInstance;

    [Inject]
    public MinigameManager(
        SceneManager sceneManager,
        ILoadingPercentHandler percentHandler,
        IPlayerProgressModel progress,
        MinigameInputDataMemoryPool inputPool,
        MinigamePlayerDataMemoryPool playerDataPool,
        MinigameRewardMemoryPool rewardPool, 
        MinigamePenaltyMemoryPool penaltyPool,
        IMinigameServicesController services)
    {
        SceneManager = sceneManager;
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

        var loadResult = await SceneManager.LoadMinigameScene(minigame, PercentHandler);

        if (loadResult == null)
        {
            Debug.LogError($"Failed to load minigame scene: {minigame}");
            return false;
        }

        CurrentSceneInstance = loadResult;

        await SceneManager.OpenScene(CurrentSceneInstance);

        MinigameModel = SceneManager.FindInScene<IMinigameModel>(CurrentSceneInstance);

        if (MinigameModel == null)
        {
            Debug.LogError($"No IMinigameModel implementation found in scene: {minigame}");
            return false;
        }

        SubscribeEvents();

        CreateInputData();
        CreateResutData();

        Services.SetUp(minigame);

        await MinigameModel.Init(Input, Result, Services);
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
        await MinigameModel.StartGame();
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
        
        Services.Clear();

        CloseScene();

        DeloadScene();
    }

    private void CloseScene()
    {
    }

    private void DeloadScene()
    {
        if (CurrentSceneInstance == null)
        {
            return;
        }

        var _ = SceneManager.DeloadScene(CurrentSceneInstance);
        CurrentSceneInstance = null;
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
}
