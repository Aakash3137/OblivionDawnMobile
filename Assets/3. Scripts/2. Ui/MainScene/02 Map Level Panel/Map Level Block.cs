using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapLevelBlock : MonoBehaviour
{
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Toggle toggleButton;
    private MapLevelDataSO mapLevelDataSO;
    private int level;

    public void Initialize(int level, ToggleGroup toggleGroup, MapLevelDataSO mapLevelDataSO)
    {
        this.level = level;
        levelText.SetText("Level {0}", level);
        toggleButton.group = toggleGroup;
        this.mapLevelDataSO = mapLevelDataSO;

        toggleButton.onValueChanged.AddListener(OnToggle);
    }

    private void OnToggle(bool value)
    {
        if (value)
        {
            GameData.mapLevelData = mapLevelDataSO;
            GameData.mapLevel = level;
        }
    }

    private void OnDestroy()
    {
        toggleButton.onValueChanged.RemoveListener(OnToggle);
    }
}
