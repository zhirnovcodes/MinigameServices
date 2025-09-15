using Cysharp.Threading.Tasks;
using System;
using Zenject;

public interface IMinigameModel : IDisposable
{
    event Action<MinigameResultData> GameplayFinished;

    UniTask Init(MinigameInputData data, MinigameResultData resultData, IMinigameServices services);
    UniTask StartGame();
}
