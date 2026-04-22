using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapLevelBlock : MonoBehaviour
{
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Toggle toggleButton;
    private int level;

    public void Initialize(int level, ToggleGroup toggleGroup)
    {
        this.level = level;
        levelText.SetText("Level {0}", level);
        toggleButton.group = toggleGroup;

        toggleButton.onValueChanged.AddListener(OnToggle);
    }

    private void OnToggle(bool isOn)
    {
        if (isOn)
        {
            GameData.mapLevel = level;
        }
    }

    private void OnDestroy()
    {
        toggleButton.onValueChanged.RemoveListener(OnToggle);
    }
}
