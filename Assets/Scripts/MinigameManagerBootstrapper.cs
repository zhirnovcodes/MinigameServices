using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;
using Content.Local.Prefabs;
using Content.Local.Configs;

public sealed class MinigameManagerBootstrapper : MonoBehaviour
{
	private DiContainer Container;

	private void Awake()
	{
		Container = ProjectContext.Instance.Container;
		InitializeGameAsync().Forget();
	}

	private async UniTaskVoid InitializeGameAsync()
    {
		await CreateResourceLoader();
		await CreatePrefabLibrary();
		await InstantiateMainUICanvas();
		await InstantiateLoadingCurtain();
		await LoadConfigs();
		await LoadPlayerProgress();
		await CreateMemoryPools();
		await CreateMinigameManager();
	}

	private async UniTask CreateResourceLoader()
	{
		try
		{
			Debug.Log("Initializing Addressables...");
			// Initialize Addressables
			await Addressables.InitializeAsync();
			Debug.Log("Addressables initialized successfully");
			
			Container.Bind<IResourceLoader>().To<AddressablesResourceLoader>().AsSingle();
			Debug.Log("IResourceLoader bound to AddressablesResourceLoader");
		}
		catch (System.Exception ex)
		{
			Debug.LogError($"Failed to initialize Addressables: {ex.Message}");
			throw;
		}
	}

	private async UniTask CreatePrefabLibrary()
	{
		try
		{
			Debug.Log("Creating PrefabLibrary...");
			var resourceLoader = Container.Resolve<IResourceLoader>();
			var prefabLibrary = new PrefabLibrary(resourceLoader);
			Container.Bind<IPrefabLibrary>().FromInstance(prefabLibrary).AsSingle();
			Debug.Log("IPrefabLibrary bound successfully");
			
			Debug.Log("Preloading all assets...");
			await prefabLibrary.PreloadAllAssets();
			Debug.Log("All assets preloaded successfully");
		}
		catch (System.Exception ex)
		{
			Debug.LogError($"Failed to create PrefabLibrary or preload assets: {ex.Message}");
			throw;
		}
	}

	private async UniTask InstantiateMainUICanvas()
	{
		try
		{
			Debug.Log("Instantiating MainUICanvas...");
			var prefabLibrary = Container.Resolve<IPrefabLibrary>();
			var mainUICanvas = prefabLibrary.InstantiatePrefab(UI.Pages.MainUICanvas);
			
			// Bind the main UI canvas GameObject to Zenject container
			Container.Bind<GameObject>().WithId("MainUICanvas").FromInstance(mainUICanvas);
			Debug.Log("MainUICanvas instantiated and bound to Zenject container");
		}
		catch (System.Exception ex)
		{
			Debug.LogError($"Failed to instantiate MainUICanvas: {ex.Message}");
			throw;
		}
	}

	private async UniTask InstantiateLoadingCurtain()
	{
		try
		{
			Debug.Log("Instantiating MinigameLoadingCurtain...");
			var prefabLibrary = Container.Resolve<IPrefabLibrary>();
			var loadingCurtain = prefabLibrary.InstantiatePrefab(UI.Pages.MinigameLoadingCurtain);
			
			// Get the MinigameLoadingCurtainView component and enable it
			var loadingCurtainView = loadingCurtain.GetComponent<MinigameLoadingCurtainView>();
			
			// Bind to Zenject container for dependency injection
			Container.Bind<MinigameLoadingCurtainView>().FromInstance(loadingCurtainView).AsSingle();
			
			// Create and bind the presenter
			var presenter = Container.Instantiate<MinigameLoadingCurtainPresenter>();
			Container.Bind<MinigameLoadingCurtainPresenter>().FromInstance(presenter).AsSingle();
			Container.Bind<ILoadingPercentHandler>().FromInstance(presenter).AsSingle();
			
			Debug.Log("MinigameLoadingCurtain instantiated, enabled, and bound to Zenject container");
		}
		catch (System.Exception ex)
		{
			Debug.LogError($"Failed to instantiate MinigameLoadingCurtain: {ex.Message}");
			throw;
		}
	}

	private async UniTask LoadConfigs()
	{
		try
		{
			Debug.Log("Loading configs...");
			var resourceLoader = Container.Resolve<IResourceLoader>();
			var loadingHandler = Container.Resolve<ILoadingPercentHandler>();
			
			// Load MinigamesConfig using LoadConfig extension method
			var minigamesConfig = await resourceLoader.LoadConfig(Content.Local.Configs.Minigames.MinigamesConfig, loadingHandler);
			
			if (minigamesConfig != null && minigamesConfig is MinigamesConfig config)
			{
				// Create and bind the config model
				var configModel = new MinigameConfigModel(config);
				Container.Bind<MinigameConfigModel>().FromInstance(configModel).AsSingle();
				
				Debug.Log("MinigamesConfig loaded and bound to Zenject container");
			}
			else
			{
				Debug.LogError("Failed to load MinigamesConfig");
				throw new System.Exception("Failed to load MinigamesConfig");
			}
		}
		catch (System.Exception ex)
		{
			Debug.LogError($"Failed to load configs: {ex.Message}");
			throw;
		}
	}

	private async UniTask LoadPlayerProgress()
	{
		try
		{
			Debug.Log("Loading player progress...");
			var playerProgressModel = new PlayerProgressModel();
			playerProgressModel.Load();
			
			Container.Bind<IPlayerProgressModel>().FromInstance(playerProgressModel).AsSingle();
			Debug.Log("Player progress loaded and bound to Zenject container");
		}
		catch (System.Exception ex)
		{
			Debug.LogError($"Failed to load player progress: {ex.Message}");
			throw;
		}
	}

	private async UniTask CreateMemoryPools()
	{
		try
		{
			Debug.Log("Creating memory pools...");
			
			// Create memory pools
			var inputDataPool = new MinigameInputDataMemoryPool();
			var playerDataPool = new MinigamePlayerDataMemoryPool();
			var rewardPool = new MinigameRewardMemoryPool();
			var penaltyPool = new MinigamePenaltyMemoryPool();
			
			// Bind memory pools to Zenject container
			Container.Bind<MinigameInputDataMemoryPool>().FromInstance(inputDataPool).AsSingle();
			Container.Bind<MinigamePlayerDataMemoryPool>().FromInstance(playerDataPool).AsSingle();
			Container.Bind<MinigameRewardMemoryPool>().FromInstance(rewardPool).AsSingle();
			Container.Bind<MinigamePenaltyMemoryPool>().FromInstance(penaltyPool).AsSingle();
			
			Debug.Log("Memory pools created and bound to Zenject container");
		}
		catch (System.Exception ex)
		{
			Debug.LogError($"Failed to create memory pools: {ex.Message}");
			throw;
		}
	}

	private async UniTask CreateMinigameManager()
	{
		try
		{
			Debug.Log("Creating MinigameServices...");
			var minigameServices = Container.Instantiate<MinigameServices>();
			Container.Bind<MinigameServices>().FromInstance(minigameServices).AsSingle();
			Debug.Log("MinigameServices created and bound to Zenject container");
			
			Debug.Log("Creating MinigameManager...");
			var minigameManager = Container.Instantiate<MinigameManager>();
			
			Container.Bind<IMinigameManager>().FromInstance(minigameManager).AsSingle();
			
			Debug.Log("MinigameManager created and bound to Zenject container");
		}
		catch (System.Exception ex)
		{
			Debug.LogError($"Failed to create MinigameManager: {ex.Message}");
			throw;
		}
	}
}
