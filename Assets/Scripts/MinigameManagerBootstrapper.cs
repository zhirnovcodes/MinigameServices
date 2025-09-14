using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

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
}
