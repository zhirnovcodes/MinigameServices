public interface IMinigameServices
{
    IMinigamesConfigLoader GetConfigLoader();
    IMinigameGameObjectPool GetMinigamePool();
    IGameObjectPool GetCommonObjectPool();
}

public interface IMinigameServicesController : IMinigameServices
{
    void SetUp(Minigames id);
    void Clear();
}

public class MinigameServices : IMinigameServicesController
{
    private PrefabLibrary MinigameLibrary;
    private MinigamesConfigLoader ConfigLoader;
    private IGameObjectPool CommonPool;
    private IGameObjectPool MinigameGOPool;
    private MinigameGameObjectPool MinigamePool;

    public MinigameServices(IResourceLoader loader, IPrefabLibrary commonLibrary)
    {
        CommonPool = new GameObjectPool(commonLibrary);
        
        MinigameLibrary = new PrefabLibrary(loader);
        MinigameGOPool = new GameObjectPool(MinigameLibrary);
        MinigamePool = new MinigameGameObjectPool(MinigameGOPool, MinigameLibrary);
        ConfigLoader = new MinigamesConfigLoader(loader);
    }

    public void SetUp(Minigames id)
    {
        MinigameLibrary.SetRootFolderName($"Assets/Content/Remote/Minigames/{id}");
        ConfigLoader.SetMinigame(id);
    }

    public void Clear()
    {
        CommonPool.Clear();
        MinigamePool.Clear();
    }

    public IMinigamesConfigLoader GetConfigLoader() => ConfigLoader;

    public IMinigameGameObjectPool GetMinigamePool() => MinigamePool;

    public IGameObjectPool GetCommonObjectPool() => CommonPool;
}