using Cysharp.Threading.Tasks;

public interface IMinigameManager
{
    UniTask<bool> LoadMinigame(Minigames minigame);
    UniTask StartMinigame();
    void DeloadMinigame();
}
