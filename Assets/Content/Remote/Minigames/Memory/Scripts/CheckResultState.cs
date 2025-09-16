using Cysharp.Threading.Tasks;

public class CheckResultState
{
    private readonly BoardModel BoardModel;
    private readonly GameplayDataModel GameplayDataModel;
    private WaitForInputState WaitForInputState;

    public CheckResultState(
        BoardModel boardModel,
        GameplayDataModel gameplayDataModel
        )
    {
        BoardModel = boardModel;
        GameplayDataModel = gameplayDataModel;
    }

    public void InjectState(WaitForInputState waitForInputState)
    {
        WaitForInputState = waitForInputState;
    }

    public async UniTask<MemoryGameResultData> Play()
    {
        UnityEngine.Debug.Log("Check");

        await UniTask.WaitForSeconds(0.5f);

        // Check if both selected blocks are the same
        if (IsSameSelected())
        {
            return CreateResult();
        }

        // Blocks don't match - flip both back and continue
        await FlipBothSelected();
            
        // Deselect all blocks
        GameplayDataModel.DeselectAll();
            
        // Go back to wait for input
        return await WaitForInputState.Play();
    }

    private bool IsSameSelected()
    {
        var id1 = GameplayDataModel.GetFirstSelectedIndex().Value;
        var id2 = GameplayDataModel.GetSecondSelectedIndex().Value;

        var data1 = BoardModel.GetBlock(id1).GetResultData();
        var data2 = BoardModel.GetBlock(id2).GetResultData();

        var icon1 = data1.IsSuccess ? data1.Reward.Icon : data1.Penalty.Icon;
        var icon2 = data2.IsSuccess ? data2.Reward.Icon : data2.Penalty.Icon;

        return icon1 == icon2;
    }

    private async UniTask FlipBothSelected()
    {
        // Get the selected block indices
        var firstIndex = GameplayDataModel.GetFirstSelectedIndex();
        var secondIndex = GameplayDataModel.GetSecondSelectedIndex();
        
        if (firstIndex.HasValue && secondIndex.HasValue)
        {
            // Get the blocks and flip them back to hidden state
            var firstBlock = BoardModel.GetBlock(firstIndex.Value);
            var secondBlock = BoardModel.GetBlock(secondIndex.Value);
            
            // Flip both blocks back to hidden state
            await UniTask.WhenAll(
                firstBlock.PlayFlipHideAnimation(),
                secondBlock.PlayFlipHideAnimation()
            );
        }
    }

    private MemoryGameResultData CreateResult()
    {
        var index = GameplayDataModel.GetFirstSelectedIndex().Value;
        var data = BoardModel.GetBlock(index).GetResultData();

        return data;
    }
}
