using Zenject;

public class MinigameConfigModel
{
    private MinigamesConfig Config;

    [Inject]
    public MinigameConfigModel(MinigamesConfig config)
    {
        Config = config;
    }

    public MinigameConfig GetMinigameConfig(int index)
    {
        return Config.Minigames[index];
    }

    public int GetMinigamesCount()
    {
        return Config.Minigames.Count;
    }
}