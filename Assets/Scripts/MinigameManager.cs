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

    private MinigameInputData Input;


    [Inject]
    public MinigameManager(
        IResourceLoader resourceLoader,
        ILoadingPercentHandler percentHandler,
        IPlayerProgressModel progress,
        MinigameInputDataMemoryPool inputPool, 
        MinigamePlayerDataMemoryPool playerDataPool)
    {
        ResourceLoader = resourceLoader;
        PercentHandler = percentHandler;
        Progress = progress;
        InputPool = inputPool;
        PlayerDataPool = playerDataPool;
    }

    public async UniTask<bool> LoadMinigame(Minigames minigame)
    {
        Debug.Assert(MinigameModel != null, "You forgot to call Deload");

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

        MinigameModel.Init(Input);
        return true;
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


    public async UniTaskVoid StartMinigame()
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
            Progress.GetResources().AddCharacterCard(card);
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
