using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IGameplayUIPresenter
{
    UniTask PlayStart();
    UniTask PlayFinishGame(MemoryGameResultData resultData);
}

public class GameplayUIPresenter : IGameplayUIPresenter
{
    private GameplayStartUIView StartUIView;
    private GameplayResultView ResultView;
    
    public event Action<MemoryGameResultData> GameplayFinished = (result) => { };
    
    public GameplayUIPresenter(
        GameplayStartUIView startUIView,
        GameplayResultView resultView)
    {
        StartUIView = startUIView;
        ResultView = resultView;
    }
    
    public async UniTask PlayStart()
    {
        // Show countdown sequence
        StartUIView.Enable();
        StartUIView.Show3();
        await UniTask.Delay(1000);
        
        StartUIView.Show2();
        await UniTask.Delay(1000);
        
        StartUIView.Show1();
        await UniTask.Delay(1000);
        
        StartUIView.ShowGo();
        await UniTask.Delay(500);
        
        StartUIView.Disable();
    }
    
    public async UniTask PlayFinishGame(MemoryGameResultData resultData)
    {
        ResultView.SetResult(resultData);
        ResultView.Enable();
        
        // Play appropriate animation based on result
        if (resultData.IsTimeout)
        {
            await ResultView.PlayTimeIsUpAnimation();
        }
        else if (resultData.IsSuccess)
        {
            await ResultView.PlayRewardAnimation();
        }
        else
        {
            await ResultView.PlayPenaltyAnimation();
        }
        
        // Trigger the finished event
        GameplayFinished?.Invoke(resultData);
    }

}
