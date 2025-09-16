using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class MemoryMinigameModel : MonoBehaviour, IMinigameModel
{
    public event Action<MinigameResultData> GameplayFinished;

    public BoardModel Board;
    public GameplayTimerModel TimerModel;
    public GameplayTimerView TimerView;
    public GameplayStartUIView StartUiView;
    public GameplayResultView ResultView;
    
    // State dependencies
    public GameplayDataModel GameplayDataModel;
    private IGameplayUIPresenter GameplayUIPresenter;
    private ITimerUIPresenter TimerPresenter;
    
    // States
    private WaitForInputState WaitForInputState;
    private CheckResultState CheckResultState;

    public void Dispose()
    {
        // Dispose board
        Board?.Dispose();
    }

    public async UniTask Init(MinigameInputData data, MinigameResultData resultData, IMinigameServices services)
    {
        await services.GetMinigamePool().PreloadPrefabs<Icons>();

        var rewardConfig = await services.GetConfigLoader().LoadConfig(Configs.RewardsConfig);
        var penaltiesConfig = await services.GetConfigLoader().LoadConfig(Configs.PenaltiesConfig);
        var configModel = new MemoryGameConfigModel((MemoryGameRewardsConfig)rewardConfig, 
            (MemoryGamePenaltiesConfig)penaltiesConfig);

        Board.Construct(services.GetMinigamePool(), configModel);
        Board.Build();

        // Create gameplay data
        var gameplayData = new GameplayData();
        GameplayDataModel = new GameplayDataModel(gameplayData);

        // Create timer ui

        TimerPresenter = new TimerUIPresenter(TimerView, TimerModel);

        // Create UI presenter
        GameplayUIPresenter = new GameplayUIPresenter(StartUiView, ResultView);

        // Create states
        CheckResultState = new CheckResultState(Board, GameplayDataModel, null);
        WaitForInputState = new WaitForInputState(Board, GameplayDataModel, TimerModel, CheckResultState);

        // Set up circular reference for CheckResultState
        CheckResultState = new CheckResultState(Board, GameplayDataModel, WaitForInputState);
    }

    public async UniTask StartGame()
    {
        // Play start sequence
        await GameplayUIPresenter.PlayStart();

        TimerPresenter.Enable();

        // Start the timer
        TimerModel.Run();

        // Start the state machine with WaitForInputState
        var result = await WaitForInputState.Play();

        HandleGameplayFinished(result);

        TimerPresenter.Disable();

        await GameplayUIPresenter.PlayFinishGame(result);
    }

    private void HandleGameplayFinished(MemoryGameResultData resultData)
    {
        // Convert MemoryGameResultData to MinigameResultData and trigger event
        var minigameResult = new MinigameResultData
        {
            Status = resultData.IsSuccess ? MinigameStatuses.Success : MinigameStatuses.Fail,
            Reward = new MinigameRewardData
            {
                Cash = resultData.Reward.Cash,
                Diamonds = resultData.Reward.Diamonds,
                CharacterCards = new System.Collections.Generic.List<CharacterCardsData> { resultData.Reward.Cards }
            },
            Penalties = new MinigamePenaltiesData
            {
                Penalties = new System.Collections.Generic.List<MinigamePenaltyData>()
            }
        };

        GameplayFinished.Invoke(minigameResult);
    }
}
