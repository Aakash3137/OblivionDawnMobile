using UnityEngine;
using UnityEngine.UI;

public class TileUIPanelNetwork : MonoBehaviour
{
    [Header("UI")]
    public Button BackButton;

    private NetworkTile currentTile;
   // private FloatingUiManagerNetwork manager;
    private BuildUi[] buildButtons;

    private void Awake()
    {
       // manager = GetComponentInParent<FloatingUiManagerNetwork>();
        buildButtons = GetComponentsInChildren<BuildUi>(true);
    }

    private void Start()
    {
        if (BackButton != null)
            BackButton.onClick.AddListener(OnBackClicked);
            
        UpdateBuildButtonNames();
    }
    
    private void UpdateBuildButtonNames()
    {
        if (GameData.SelectedMPFaction == null || buildButtons == null) return;
        
        foreach (var buildButton in buildButtons)
        {
            if (buildButton.prefabNameText.Contains("Defence") || buildButton.prefabNameText.Contains("Turret"))
            {
                if (GameData.SelectedMPFaction.defenceBuildingPrefab != null)
                    buildButton.prefabNameText = GameData.SelectedMPFaction.defenceBuildingPrefab.name;
            }
            else if (buildButton.prefabNameText.Contains("Unit"))
            {
                if (GameData.SelectedMPFaction.unitBuildingPrefab != null)
                    buildButton.prefabNameText = GameData.SelectedMPFaction.unitBuildingPrefab.name;
            }
            else if (buildButton.prefabNameText.Contains("Resource") || buildButton.prefabNameText.Contains("Gold"))
            {
                if (GameData.SelectedMPFaction.resourceBuildingPrefab != null)
                    buildButton.prefabNameText = GameData.SelectedMPFaction.resourceBuildingPrefab.name;
            }
        }
    }

    public void Open(NetworkTile tile)
    {
        currentTile = tile;
        UpdateBuildButtonNames();
    }

    private void OnBackClicked()
    {
        currentTile = null;

      //  if (manager != null)
       //     manager.CloseUI();
    }
}