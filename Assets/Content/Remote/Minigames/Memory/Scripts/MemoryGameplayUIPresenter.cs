using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MemoryGameplayUIPresenter : IGameplayUIPresenter
{

    public async UniTask PlayFinishGame(MemoryGameResultData resultData)
    {
        // Placeholder implementation
        Debug.Log($"Playing finish game with result: Success={resultData.IsSuccess}");
        
        // Simulate some UI animation time
        await UniTask.Delay(1000);
    }

    public UniTask PlayStart()
    {
        throw new NotImplementedException();
    }
}
