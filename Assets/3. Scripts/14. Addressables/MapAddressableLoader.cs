using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MapAddressableLoader : MonoBehaviour
{
    public static MapAddressableLoader Instance { get; private set; }

    public event Action<float> OnLoadProgress;
    public event Action<MapAssetBundle> OnAssetsReady;
    public event Action<string> OnLoadFailed;

    private AsyncOperationHandle<GameObject> _prefabHandle;
    private AsyncOperationHandle<Texture2D>  _textureHandle;
    private bool _hasLoadedAssets;

   
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        ReleaseCurrentAssets();
    }
    
    public async Task<MapAssetBundle?> LoadMapAssetsAsync(MapLevelDataSO levelData)
    {
        if (levelData == null)
        {
            Debug.LogError("[MapAddressableLoader] levelData is null.");
            return null;
        }

        ReleaseCurrentAssets();

        try
        {
            _prefabHandle  = levelData.environmentPrefabRef.LoadAssetAsync<GameObject>();
            _textureHandle = levelData.tileTextureRef.LoadAssetAsync<Texture2D>();
            
            while (!_prefabHandle.IsDone || !_textureHandle.IsDone)
            {
                float progress = (_prefabHandle.PercentComplete + _textureHandle.PercentComplete) * 0.5f;
                OnLoadProgress?.Invoke(progress);
                await Task.Yield();
            }

            OnLoadProgress?.Invoke(1f);

            if (_prefabHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Fail($"Failed to load environment prefab for level {levelData.level}.");
                return null;
            }

            if (_textureHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Fail($"Failed to load tile texture for level {levelData.level}.");
                return null;
            }

            _hasLoadedAssets = true;

            var bundle = new MapAssetBundle(environmentPrefab : _prefabHandle.Result,
                tileTexture : _textureHandle.Result
            );

            OnAssetsReady?.Invoke(bundle);
            return bundle;
        }
        catch (Exception ex)
        {
            Fail($"Exception during load: {ex}");
            return null;
        }
    }


    public void ReleaseCurrentAssets()
    {
        if (!_hasLoadedAssets) return;

        if (_prefabHandle.IsValid())
            Addressables.Release(_prefabHandle);

        if (_textureHandle.IsValid())
            Addressables.Release(_textureHandle);

        _hasLoadedAssets = false;
    }


    private void Fail(string message)
    {
        string full = $"[MapAddressableLoader] {message}";
        Debug.LogError(full);
        OnLoadFailed?.Invoke(full);
        ReleaseCurrentAssets();
    }
}

public readonly struct MapAssetBundle
{
    public readonly GameObject EnvironmentPrefab;
    public readonly Texture2D  TileTexture;

    public MapAssetBundle(GameObject environmentPrefab, Texture2D tileTexture)
    {
        EnvironmentPrefab = environmentPrefab;
        TileTexture       = tileTexture;
    }
}