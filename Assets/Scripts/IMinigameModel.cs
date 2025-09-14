using Cysharp.Threading.Tasks;
using System;

public interface IMinigameModel : IDisposable
{
    event Action<MinigameResultData> GameplayFinished;

    void Init(MinigameInputData data, MinigameResultData resultData, MinigameServices services);
    UniTask StartGame();
}

public class MinigameServices
{

}