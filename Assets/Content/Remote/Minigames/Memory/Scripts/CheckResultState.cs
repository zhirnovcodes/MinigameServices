using Cysharp.Threading.Tasks;

public class CheckResultState
{
    private readonly BoardModel BoardModel;
    private readonly GameplayDataModel GameplayDataModel;
    private readonly WaitForInputState WaitForInputState;

    public CheckResultState(
        BoardModel boardModel,
        GameplayDataModel gameplayDataModel,
        WaitForInputState waitForInputState)
    {
        BoardModel = boardModel;
        GameplayDataModel = gameplayDataModel;
        WaitForInputState = waitForInputState;
    }

    public async UniTask<MemoryGameResultData> Play()
    {
        UnityEngine.Debug.Log("Check");

        await UniTask.WaitForSeconds(0.5f);

        // Check if both selected blocks are the same
        if (GameplayDataModel.IsSameSelected())
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
        var data = BoardModel.GetBlockData(index);

        return data;
    }
}
