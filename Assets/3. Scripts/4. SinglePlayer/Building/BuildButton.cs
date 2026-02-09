using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuildButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ScenarioBuildingType buildingType;
    public PlayerResourceManager prmInstance;
    public TileUIPanel tileUIPanel;
    public CostPanelManager costPanelManager;

    [SerializeField] private ScenarioOffenseType offenseType;
    [SerializeField] private ScenarioDefenseType defenseType;
    [SerializeField] private ScenarioResourceType resourceType;

    public GameObject buildingToSpawn;
    private Button button;
    [SerializeField] internal Image ButtonIcon;
    private BuildCost[] cachedCosts;

    private void Start()
    {
        button = GetComponent<Button>();
        UpdateButtonUI();

        button.onClick.AddListener(OnClick);

        buildingToSpawn = GetSlot(GameData.SelectedFaction, buildingType);

        if (buildingToSpawn.TryGetComponent<BuildingStats>(out var spawnBuildingStats))
        {
            cachedCosts = spawnBuildingStats.buildingStats.buildingBuildCost;
        }
        else
        {
            Debug.Log($"<color=red>No BuildingStats or WallStats found on {buildingToSpawn.name}</color>");
        }
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

        costPanelManager.Hide();

        if (buildingToSpawn != null && buildingToSpawn != null)
        {
            tileUIPanel.PlaceBuilding(buildingToSpawn);

            if (cachedCosts != null && !prmInstance.HasResources(cachedCosts))
            {
                costPanelManager.Show(cachedCosts);
            }
        }
    }

    private GameObject GetSlot(FactionName faction, ScenarioBuildingType type)
    {
        var data = GameData.AllFactionsData;
        switch (faction)
        {
            case FactionName.Medieval:
                if (type == ScenarioBuildingType.MainBuilding) return data.medievalMainBuilding;
                if (type == ScenarioBuildingType.DefenseBuilding) return GetMedievalDefenseBuilding(data);
                if (type == ScenarioBuildingType.OffenseBuilding) return GetMedievalOffenseBuilding(data);
                if (type == ScenarioBuildingType.ResourceBuilding) return GetMedievalResourceBuilding(data);
                break;
            case FactionName.Present:
                if (type == ScenarioBuildingType.MainBuilding) return data.presentMainBuilding;
                if (type == ScenarioBuildingType.DefenseBuilding) return GetPresentDefenseBuilding(data);
                if (type == ScenarioBuildingType.OffenseBuilding) return GetPresentOffenseBuilding(data);
                if (type == ScenarioBuildingType.ResourceBuilding) return GetPresentResourceBuilding(data);
                break;
            case FactionName.Futuristic:
                if (type == ScenarioBuildingType.MainBuilding) return data.futureMainBuilding;
                if (type == ScenarioBuildingType.DefenseBuilding) return GetFuturisticDefenseBuilding(data);
                if (type == ScenarioBuildingType.OffenseBuilding) return GetFuturisticOffenseBuilding(data);
                if (type == ScenarioBuildingType.ResourceBuilding) return GetFuturisticResourceBuilding(data);
                break;
            case FactionName.Galvadore:
                if (type == ScenarioBuildingType.MainBuilding) return data.galvadoreMainBuilding;
                if (type == ScenarioBuildingType.DefenseBuilding) return GetGalvadoreDefenseBuilding(data);
                if (type == ScenarioBuildingType.OffenseBuilding) return GetGalvadoreOffenseBuilding(data);
                if (type == ScenarioBuildingType.ResourceBuilding) return GetGalvadoreResourceBuilding(data);
                break;
        }
        return null;
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

    public GameObject GetMedievalResourceBuilding(AllFactionsData factionData)
    {
        switch (resourceType)
        {
            case ScenarioResourceType.Food:
                return factionData.medievalFoodBuilding;
            case ScenarioResourceType.Gold:
                return factionData.medievalGoldBuilding;
            case ScenarioResourceType.Metal:
                return factionData.medievalMetalBuilding;
            case ScenarioResourceType.Power:
                return factionData.medievalPowerBuilding;
            default:
                return null;
        }
    }

    private GameObject GetPresentResourceBuilding(AllFactionsData factionData)
    {
        switch (resourceType)
        {
            case ScenarioResourceType.Food:
                return factionData.presentFoodBuilding;
            case ScenarioResourceType.Gold:
                return factionData.presentGoldBuilding;
            case ScenarioResourceType.Metal:
                return factionData.presentMetalBuilding;
            case ScenarioResourceType.Power:
                return factionData.presentPowerBuilding;
            default:
                return null;
        }
    }

    private GameObject GetFuturisticResourceBuilding(AllFactionsData factionData)
    {
        switch (resourceType)
        {
            case ScenarioResourceType.Food:
                return factionData.futureFoodBuilding;
            case ScenarioResourceType.Gold:
                return factionData.futureGoldBuilding;
            case ScenarioResourceType.Metal:
                return factionData.futureMetalBuilding;
            case ScenarioResourceType.Power:
                return factionData.futurePowerBuilding;
            default:
                return null;
        }
    }

    private GameObject GetGalvadoreResourceBuilding(AllFactionsData factionData)
    {
        switch (resourceType)
        {
            case ScenarioResourceType.Food:
                return factionData.galvadoreFoodBuilding;
            case ScenarioResourceType.Gold:
                return factionData.galvadoreGoldBuilding;
            case ScenarioResourceType.Metal:
                return factionData.galvadoreMetalBuilding;
            case ScenarioResourceType.Power:
                return factionData.galvadorePowerBuilding;
            default:
                return null;
        }
    }

    private GameObject GetMedievalDefenseBuilding(AllFactionsData factionData)
    {
        switch (defenseType)
        {
            case ScenarioDefenseType.AntiAir:
                return factionData.medievalAntiAirBuilding;
            case ScenarioDefenseType.AntiTank:
                return factionData.medievalAntiTankBuilding;
            case ScenarioDefenseType.Turret:
                return factionData.pastTurretBuilding;
            case ScenarioDefenseType.Wall:
                return factionData.medievalWallBuilding;
            default:
                return null;
        }
    }

    private GameObject GetPresentDefenseBuilding(AllFactionsData factionData)
    {
        switch (defenseType)
        {
            case ScenarioDefenseType.AntiAir:
                return factionData.presentAntiAirBuilding;
            case ScenarioDefenseType.AntiTank:
                return factionData.presentAntiTankBuilding;
            case ScenarioDefenseType.Turret:
                return factionData.presentTurretBuilding;
            case ScenarioDefenseType.Wall:
                return factionData.presentWallBuilding;
            default:
                return null;
        }
    }

    private GameObject GetFuturisticDefenseBuilding(AllFactionsData factionData)
    {
        switch (defenseType)
        {
            case ScenarioDefenseType.AntiAir:
                return factionData.futureAntiAirBuilding;
            case ScenarioDefenseType.AntiTank:
                return factionData.futureAntiTankBuilding;
            case ScenarioDefenseType.Turret:
                return factionData.futureTurretBuilding;
            case ScenarioDefenseType.Wall:
                return factionData.futureWallBuilding;
            default:
                return null;
        }
    }

    private GameObject GetGalvadoreDefenseBuilding(AllFactionsData factionData)
    {
        switch (defenseType)
        {
            case ScenarioDefenseType.AntiAir:
                return factionData.galvadoreAntiAirBuilding;
            case ScenarioDefenseType.AntiTank:
                return factionData.galvadoreAntiTankBuilding;
            case ScenarioDefenseType.Turret:
                return factionData.galvadoreTurretBuilding;
            case ScenarioDefenseType.Wall:
                return factionData.galvadoreWallBuilding;
            default:
                return null;
        }
    }

    private GameObject GetMedievalOffenseBuilding(AllFactionsData factionData)
    {
        switch (offenseType)
        {
            case ScenarioOffenseType.AirBuilding:
                return factionData.medievalAirBuilding;
            case ScenarioOffenseType.MeleeBuilding:
                return factionData.medievalMeleeBuilding;
            case ScenarioOffenseType.AOERangedBuilding:
                return factionData.medievalAOERangedBuilding;
            case ScenarioOffenseType.RangedBuilding:
                return factionData.medievalRangedBuilding;
            default:
                return null;
        }
    }

    private GameObject GetPresentOffenseBuilding(AllFactionsData factionData)
    {
        switch (offenseType)
        {
            case ScenarioOffenseType.AirBuilding:
                return factionData.presentAirBuilding;
            case ScenarioOffenseType.MeleeBuilding:
                return factionData.presentMeleeBuilding;
            case ScenarioOffenseType.RangedBuilding:
                return factionData.presentRangedBuilding;
            case ScenarioOffenseType.AOERangedBuilding:
                return factionData.presentAOERangedBuilding;
            default:
                return null;
        }
    }

    private GameObject GetFuturisticOffenseBuilding(AllFactionsData factionData)
    {
        switch (offenseType)
        {
            case ScenarioOffenseType.AirBuilding:
                return factionData.futureAirBuilding;
            case ScenarioOffenseType.MeleeBuilding:
                return factionData.futureMeleeBuilding;
            case ScenarioOffenseType.RangedBuilding:
                return factionData.futureRangedBuilding;
            case ScenarioOffenseType.AOERangedBuilding:
                return factionData.futureAOERangedBuilding;
            default:
                return null;
        }
    }

    private GameObject GetGalvadoreOffenseBuilding(AllFactionsData factionData)
    {
        switch (offenseType)
        {
            case ScenarioOffenseType.AirBuilding:
                return factionData.galvadoreAirBuilding;
            case ScenarioOffenseType.MeleeBuilding:
                return factionData.galvadoreMeleeBuilding;
            case ScenarioOffenseType.RangedBuilding:
                return factionData.galvadoreRangedBuilding;
            case ScenarioOffenseType.AOERangedBuilding:
                return factionData.galvadoreAOERangedBuilding;
            default:
                return null;
        }
    }

    private void UpdateButtonUI()
    {
        /*switch(buildingType)
        {
            case ScenarioBuildingType.ResourceBuilding:
                
                break;
                
        }*/
    }

}
