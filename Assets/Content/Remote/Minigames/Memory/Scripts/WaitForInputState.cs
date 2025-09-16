using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class WaitForInputState
{
    private readonly BoardModel BoardModel;
    private readonly GameplayDataModel GameplayDataModel;
    private readonly IGameplayTimerModel GameplayTimerModel;
    private CheckResultState CheckResultState;

    public WaitForInputState(
        BoardModel boardModel,
        GameplayDataModel gameplayDataModel,
        IGameplayTimerModel gameplayTimerModel)
    {
        BoardModel = boardModel;
        GameplayDataModel = gameplayDataModel;
        GameplayTimerModel = gameplayTimerModel;
    }

    public void InjectState(CheckResultState state)
    {
        CheckResultState = state;
    }

    public async UniTask<MemoryGameResultData> Play()
    {
        Debug.Log("Wait");

        // Subscribe to button events
        SubscribeToButtonEvents();
        
        // Set all buttons clickable
        BoardModel.SetAllClickable();

        // Wait while both blocks are not selected and time is not up
        while (!GameplayDataModel.IsBothSelected() && !GameplayTimerModel.GetIsTimeUp())
        {
            await UniTask.Yield();
        }

        // After cycle - unsubscribe from events
        UnsubscribeFromButtonEvents();
        
        // Set all buttons unclickable
        BoardModel.SetAllButtonsUnclickable();

        // Check if time is up
        if (GameplayTimerModel.GetIsTimeUp())
        {
            return new MemoryGameResultData
            {
                IsSuccess = false,
                IsTimeout = true
            };
        }
        else
        {
            return await CheckResultState.Play();   
        }
    }

    private void SubscribeToButtonEvents()
    {
        for (int i = 0; i < BoardModel.GetBlocksCount(); i++)
        {
            var block = BoardModel.GetBlock(i);
            block.Clicked += HandleButtonClicked;
        }
    }

    private void UnsubscribeFromButtonEvents()
    {
        for (int i = 0; i < BoardModel.GetBlocksCount(); i++)
        {
            var block = BoardModel.GetBlock(i);
            block.Clicked -= HandleButtonClicked;
        }
    }

    private void HandleButtonClicked(int index)
    {
        var block = BoardModel.GetBlock(index);
        var data = block.GetResultData();

        // Set unclickable and flip
        block.SetNonInteractive();
        block.PlayFlipShowAnimation().Forget();

        // Check if first selection is empty
        if (!GameplayDataModel.IsFirstSelected())
        {
            GameplayDataModel.SetFirstSelected(index);
        }
        // Check if second selection is empty
        else if (!GameplayDataModel.IsSecondSelected())
        {
            GameplayDataModel.SetSecondSelected(index);
        }
    }
}
