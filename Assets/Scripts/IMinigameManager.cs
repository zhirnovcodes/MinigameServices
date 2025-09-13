using Cysharp.Threading.Tasks;

public interface IMinigameManager
{
    UniTask<bool> LoadMinigame(Minigames minigame);
    UniTaskVoid StartMinigame();
    void DeloadMinigame();
}
