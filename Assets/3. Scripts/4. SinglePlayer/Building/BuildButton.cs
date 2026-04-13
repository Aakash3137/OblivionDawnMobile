using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuildButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button button;

    private BuildingStats buildingPrefab;
    [SerializeField] private Image iconImage;

    private PlayerResourceManager prmInstance;
    private TileUIPanel tileUIPanel;
    private CostPanelManager costPanelManager;

    private BuildCost[] cachedCosts;
    public UnitProduceStatsSO unitSO;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);

        prmInstance = PlayerResourceManager.Instance;

        if (prmInstance != null)
        {
            prmInstance.OnResourcesChanged += UpdateButtonInteractivity;
        }
    }

    public void Initialize(BuildingStats buildingPrefab, Sprite sprite)
    {
        this.buildingPrefab = buildingPrefab;
        iconImage.sprite = sprite;

        cachedCosts = buildingPrefab.buildingStatsSO.buildingBuildCost;
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

        tileUIPanel.PlaceBuilding(buildingPrefab, unitSO);

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

    public void SetManagers(TileUIPanel tileUIPanel, CostPanelManager costPanelManager)
    {
        this.tileUIPanel = tileUIPanel;
        this.costPanelManager = costPanelManager;
    }

}
