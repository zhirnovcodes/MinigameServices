using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;
using Content.Local.Prefabs;

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
			Container.Bind<ILoadingPercentHandler>().To<MinigameLoadingCurtainPresenter>().FromInstance(presenter).AsSingle();
			
			Debug.Log("MinigameLoadingCurtain instantiated, enabled, and bound to Zenject container");
		}
		catch (System.Exception ex)
		{
			Debug.LogError($"Failed to instantiate MinigameLoadingCurtain: {ex.Message}");
			throw;
		}
	}
}
