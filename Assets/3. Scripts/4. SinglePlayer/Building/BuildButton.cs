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

        if (buildingPrefab.buildingStatsSO.buildingIcon != null)
            iconImage.sprite = buildingPrefab.buildingStatsSO.buildingIcon;

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
            cachedCosts = buildingPrefab.buildingStatsSO.buildingBuildCost;
        }
        else
            Debug.Log($"<color=red> [BuildButton] No BuildingStats or found on {buildingPrefab.name}</color>");


        //// BUILD BUTTON TESTING
        prmInstance = PlayerResourceManager.Instance;

        if (prmInstance != null)
        {
            prmInstance.OnResourcesChanged += UpdateButtonInteractivity;
        }

        UpdateButtonInteractivity();


    }

    private void UpdateButtonInteractivity()
    {
        if (cachedCosts == null || !prmInstance.HasResources(cachedCosts))
        {
            button.interactable = false;
            SetDesaturated(true);
        }
        else
        {
            button.interactable = true;
            SetDesaturated(false);
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
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        costPanelManager.Hide();
#endif
    }
    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnClick);
        //prmInstance.OnResourcesChanged -= UpdateButtonInteractivity;

        button.onClick.RemoveListener(OnClick);

        if (prmInstance != null)
            prmInstance.OnResourcesChanged -= UpdateButtonInteractivity;


    }

    private void SetDesaturated(bool state)
    {
        if (iconImage == null) return;

        iconImage.color = state ? new Color(0.2f, 0.2f, 0.2f, 1f) : Color.white;
    }

}
