using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class WheelMinigameModel : MonoBehaviour, IMinigameModel
{
    public event Action<MinigameResultData> GameplayFinished = (data) => { };

    private MinigameResultData ResultData;

    public async UniTask Init(MinigameInputData data, MinigameResultData resultData, IMinigameServices services)
    {
        ResultData = resultData;

        await services.GetMinigamePool().PreloadPrefabs<Prefabs>();
    }

    public async UniTask StartGame()
    {
        // Wait 2 seconds at the beginning
        await UniTask.Delay(2000);



        ResultData.Reward.Cash = 10;
        ResultData.Reward.Diamonds = 1;
        ResultData.Reward.CharacterCards.Add(new CharacterCardsData
        {
            ID = CharacterCards.Cat,
            Count = 1
        });
        GameplayFinished(ResultData);
        
        // Wait 2 seconds at the end
        await UniTask.Delay(2000);
    }

    public void Dispose()
    {
        ResultData = null;
    }
}
