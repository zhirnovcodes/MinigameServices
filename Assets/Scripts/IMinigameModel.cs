using Cysharp.Threading.Tasks;
using System;

public interface IMinigameModel : IDisposable
{
    event Action<MinigameResultData> GameplayFinished;

    void Init(MinigameInputData data);
    UniTask Start();
}
