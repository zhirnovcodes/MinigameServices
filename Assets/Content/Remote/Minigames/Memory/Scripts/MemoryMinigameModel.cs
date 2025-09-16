using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class MemoryMinigameModel : MonoBehaviour, IMinigameModel
{
    public event Action<MinigameResultData> GameplayFinished;

    public void Dispose()
    {
    }

    public async UniTask Init(MinigameInputData data, MinigameResultData resultData, IMinigameServices services)
    {
    }

    public async UniTask StartGame()
    {
    }
}
