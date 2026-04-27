using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapLevelBlock : MonoBehaviour
{
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Toggle toggleButton;
    
    [SerializeField] private GameObject loadingIndicator; 

    private int _level;
    
    private MapLevelDataSO _levelData;
    private bool _isLoading;

    public void Initialize(int level, MapLevelDataSO levelData, ToggleGroup toggleGroup)
    {
        _level     = level;
        _levelData = levelData;

        levelText.SetText("Level {0}", level);
        toggleButton.group = toggleGroup;
        toggleButton.onValueChanged.AddListener(OnToggle);

        SetLoadingIndicator(false);
    }

    private async void OnToggle(bool isOn)
    {
        if (!isOn)      return;
        if (_isLoading) return;

        _isLoading = true;
        SetLoadingIndicator(true);
        
        MapAssetBundle? result = await MapAddressableLoader.Instance.LoadMapAssetsAsync(_levelData);

        SetLoadingIndicator(false);
        _isLoading = false;

        if (result == null)
        {
            toggleButton.SetIsOnWithoutNotify(false);
            Debug.LogError($"[MapLevelBlock] Asset load failed for level {_level}. Toggle reset.");
            return;
        }

        MapAssetBundle bundle = result.Value;

        GameData.mapLevel = _level;
        GameData.loadedEnvironmentPrefab = bundle.EnvironmentPrefab;
        GameData.loadedTileTexture = bundle.TileTexture;
    }

    private void SetLoadingIndicator(bool visible)
    {
        if (loadingIndicator != null)
            loadingIndicator.SetActive(visible);
    }

    private void OnDestroy()
    {
        toggleButton.onValueChanged.RemoveListener(OnToggle);
    }
}