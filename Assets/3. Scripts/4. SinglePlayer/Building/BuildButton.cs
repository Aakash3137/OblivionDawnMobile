using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuildButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public BuildingStats buildingPrefab;
    [SerializeField] private Image iconImage;

    public ScenarioBuildingType buildingType;
    public PlayerResourceManager prmInstance;
    public TileUIPanel tileUIPanel;
    public CostPanelManager costPanelManager;
    private Button button;
    private BuildCost[] cachedCosts;

    private void Start()
    {
        if (buildingPrefab == null)
            Debug.Log($"<color=red> [BuildButton] No BuildingStats or found on {buildingPrefab.name}</color>");

        if (buildingPrefab.buildingStats.buildingIcon != null)
            iconImage.sprite = buildingPrefab.buildingStats.buildingIcon;

        button = GetComponent<Button>();

        button.onClick.AddListener(OnClick);

        // buildingToSpawn = GetSlot(GameData.SelectedFaction, buildingType);

        // if (buildingToSpawn.TryGetComponent<BuildingStats>(out var spawnBuildingStats))
        // {
        //     cachedCosts = spawnBuildingStats.buildingStats.buildingBuildCost;
        // }
        // else
        // {
        //     Debug.Log($"<color=red>No BuildingStats or WallStats found on {buildingToSpawn.name}</color>");
        // }

        if (buildingPrefab != null)
        {
            cachedCosts = buildingPrefab.buildingStats.buildingBuildCost;
        }
        else
            Debug.Log($"<color=red> [BuildButton] No BuildingStats or found on {buildingPrefab.name}</color>");
    }

    private void UpdateButtonInteractivity()
    {
        if (cachedCosts == null || !prmInstance.HasResources(cachedCosts))
        {
            button.interactable = false;
        }
        else
        {
            button.interactable = true;
        }
    }

    private void OnClick()
    {
        if (prmInstance == null)
            prmInstance = PlayerResourceManager.Instance;

        tileUIPanel.PlaceBuilding(buildingPrefab);

        if (cachedCosts != null && !prmInstance.HasResources(cachedCosts))
        {
            costPanelManager.Show(cachedCosts);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        costPanelManager.Show(cachedCosts);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
#if UNITY_EDITOR
        costPanelManager.Hide();
#endif
    }
    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnClick);
        //prmInstance.OnResourcesChanged -= UpdateButtonInteractivity;
    }
}
