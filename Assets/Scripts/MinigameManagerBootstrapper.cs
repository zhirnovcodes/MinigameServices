using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;
using Content.Local.Prefabs;
using Content.Local.Configs;

public sealed class MinigameManagerBootstrapper : MonoBehaviour
{
	private static MinigameManagerBootstrapper Instance;
	private DiContainer Container;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this.gameObject);
			return;
		}
		
		Instance = this;
		DontDestroyOnLoad(this.gameObject);
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
		await CreateSceneManager();
		await CreateMinigameManager();
		await CreateMetaUI();
	}

	private async UniTask CreateResourceLoader()
	{
		Debug.Log("Initializing Addressables...");
		// Initialize Addressables
		await Addressables.InitializeAsync();
		Debug.Log("Addressables initialized successfully");
		
		Container.Bind<IResourceLoader>().To<AddressablesResourceLoader>().AsSingle();
		Debug.Log("IResourceLoader bound to AddressablesResourceLoader");
	}

	private async UniTask CreatePrefabLibrary()
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

	private async UniTask InstantiateMainUICanvas()
	{
		Debug.Log("Instantiating MainUICanvas...");
		var prefabLibrary = Container.Resolve<IPrefabLibrary>();
		var mainUICanvas = prefabLibrary.InstantiatePrefab(UI.Pages.MainUICanvas);

		DontDestroyOnLoad(mainUICanvas);
		
		// Bind the main UI canvas GameObject to Zenject container
		Container.Bind<GameObject>().WithId("MainUICanvas").FromInstance(mainUICanvas);
		Debug.Log("MainUICanvas instantiated and bound to Zenject container");
	}

	private async UniTask InstantiateLoadingCurtain()
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

	private async UniTask LoadConfigs()
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

	private async UniTask LoadPlayerProgress()
	{
		Debug.Log("Loading player progress...");
		var playerProgressModel = new PlayerProgressModel();
		playerProgressModel.Load();
		
		Container.Bind<IPlayerProgressModel>().FromInstance(playerProgressModel).AsSingle();
		Debug.Log("Player progress loaded and bound to Zenject container");
	}

	private async UniTask CreateMemoryPools()
	{
		Debug.Log("Creating memory pools...");
		
		// Bind memory pools using Zenject's proper memory pool binding
		Container.BindMemoryPool<MinigameInputData, MinigameInputDataMemoryPool>();
		Container.BindMemoryPool<MinigamePlayerData, MinigamePlayerDataMemoryPool>();
		Container.BindMemoryPool<MinigameRewardData, MinigameRewardMemoryPool>();
		Container.BindMemoryPool<MinigamePenaltiesData, MinigamePenaltyMemoryPool>();
		
		var prefabLibrary = Container.Resolve<IPrefabLibrary>();
		var gameObjectPool = new GameObjectPool(prefabLibrary);
		Container.Bind<GameObjectPool>().FromInstance(gameObjectPool).AsSingle();
		
		Debug.Log("Memory pools created and bound to Zenject container");
	}

	private async UniTask CreateSceneManager()
	{
		Debug.Log("Creating SceneManager...");
		var resourceLoader = Container.Resolve<IResourceLoader>();
		var sceneManager = new SceneManager(resourceLoader);
		Container.Bind<SceneManager>().FromInstance(sceneManager).AsSingle();
		Debug.Log("SceneManager created and bound to Zenject container");
	}

	private async UniTask CreateMinigameManager()
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

	private async UniTask CreateMetaUI()
	{
		Debug.Log("Creating MetaUI...");
		var prefabLibrary = Container.Resolve<IPrefabLibrary>();
		var metaUI = prefabLibrary.InstantiatePrefab(UI.Pages.MetaUI);
		
		// Get the MetaUIView component
		var metaUIView = metaUI.GetComponent<MetaUIView>();
		if (metaUIView == null)
		{
			Debug.LogError("MetaUIView component not found on MetaUI prefab");
			throw new System.Exception("MetaUIView component not found");
		}

		// Set the view's parent to the MainUICanvas
		var mainUICanvas = Container.ResolveId<GameObject>("MainUICanvas");
		metaUI.transform.SetParent(mainUICanvas.transform, false);
		
		// Bind the view to Zenject container
		Container.Bind<MetaUIView>().FromInstance(metaUIView).AsSingle();
		
		// Create and bind the presenter
		var presenter = Container.Instantiate<MetaUIPresenter>();
		Container.Bind<MetaUIPresenter>().FromInstance(presenter).AsSingle();
		
		presenter.Enable(); 

		Debug.Log("MetaUI created and bound to Zenject container");
	}
}
