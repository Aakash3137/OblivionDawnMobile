using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuildButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ScenarioBuildingType buildingType;
    public PlayerResourceManager prmInstance;
    public TileUIPanel tileUIPanel;
    public CostPanelManager costPanelManager;

    [SerializeField] private ScenarioUnitType offenseType;
    [SerializeField] private ScenarioDefenseType defenseType;
    [SerializeField] private ScenarioResourceType resourceType;

    public GameObject buildingToSpawn;
    private Button button;
    private BuildCost[] cachedCosts;

    private void Start()
    {
        button = GetComponent<Button>();

        button.onClick.AddListener(OnClick);

        buildingToSpawn = GetSlot(GameData.SelectedFaction, buildingType);

        //prmInstance.OnResourcesChanged += UpdateButtonInteractivity;

        ///
        /// To do buttons should get SO data when starting game based on units selected 
        /// 

        if (buildingToSpawn.TryGetComponent<BuildingStats>(out var spawnBuildingStats))
        {
            //int spawnLevel = spawnBuildingStats.buildingStats.buildingSpawnLevel;
            cachedCosts = spawnBuildingStats.buildingStats.buildingBuildCost;
        }
        // else if (buildingToSpawn.TryGetComponent<WallStats>(out var spawnWallStats))
        // {
        //     int spawnLevel = spawnWallStats.wallStats.wallSpawnLevel;
        //     cachedCosts = spawnWallStats.wallStats.wallLevelData[spawnLevel].wallBuildCosts;
        // }
        else if (buildingToSpawn.TryGetComponent<BuildingBlueprint>(out var buildingBlueprint))
        {
            //int spawnLevel = buildingBlueprint.identityComponent.identity.spawnLevel;
            cachedCosts = buildingBlueprint.dataSO.buildingBuildCost;
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
        if (buildingToSpawn != null && buildingToSpawn != null)
        {
            tileUIPanel.PlaceBuilding(buildingToSpawn);

            // if (cachedCosts != null && prmInstance.HasResources(cachedCosts))
            // {
            //     costPanelManager.Hide();
            // }
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
                if (type == ScenarioBuildingType.OffenseBuilding) return GetMedievalUnitBuilding(data);
                if (type == ScenarioBuildingType.ResourceBuilding) return GetMedievalResourceBuilding(data);
                break;
            case FactionName.Present:
                if (type == ScenarioBuildingType.MainBuilding) return data.presentMainBuilding;
                if (type == ScenarioBuildingType.DefenseBuilding) return data.presentTurretBuilding;
                if (type == ScenarioBuildingType.OffenseBuilding) return data.presentInfantryBuilding;
                if (type == ScenarioBuildingType.ResourceBuilding) return GetPresentResourceBuilding(data);
                break;
            case FactionName.Futuristic:
                if (type == ScenarioBuildingType.MainBuilding) return data.futureMainBuilding;
                if (type == ScenarioBuildingType.DefenseBuilding) return data.futureTurretBuilding;
                if (type == ScenarioBuildingType.OffenseBuilding) return data.futureInfantryBuilding;
                if (type == ScenarioBuildingType.ResourceBuilding) return GetFuturisticResourceBuilding(data);
                break;
            case FactionName.Galvadore:
                if (type == ScenarioBuildingType.MainBuilding) return data.galvadoreMainBuilding;
                if (type == ScenarioBuildingType.DefenseBuilding) return data.galvadoreTurretBuilding;
                if (type == ScenarioBuildingType.OffenseBuilding) return data.galvadoreInfantryBuilding;
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
        costPanelManager.Hide();
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
            default:
                return null;
        }
    }

    private GameObject GetMedievalUnitBuilding(AllFactionsData factionData)
    {
        switch (offenseType)
        {
            case ScenarioUnitType.Air:
                return factionData.medievalAirBuilding;
            case ScenarioUnitType.Infantry:
                return factionData.medievalInfantryBuilding;
            case ScenarioUnitType.Melee:
                return factionData.medievalMeleeBuilding;
            case ScenarioUnitType.Tank:
                return factionData.medievalTankBuilding;
            default:
                return null;
        }
    }

    private GameObject GetPresentUnitBuilding(AllFactionsData factionData)
    {
        switch (offenseType)
        {
            case ScenarioUnitType.Air:
                return factionData.presentAirBuilding;
            case ScenarioUnitType.Infantry:
                return factionData.presentInfantryBuilding;
            case ScenarioUnitType.Tank:
                return factionData.presentTankBuilding;
            default:
                return null;
        }
    }

    private GameObject GetFuturisticUnitBuilding(AllFactionsData factionData)
    {
        switch (offenseType)
        {
            case ScenarioUnitType.Air:
                return factionData.futureAirBuilding;
            case ScenarioUnitType.Infantry:
                return factionData.futureInfantryBuilding;
            case ScenarioUnitType.Tank:
                return factionData.futureTankBuilding;
            default:
                return null;
        }
    }

    private GameObject GetGalvadoreUnitBuilding(AllFactionsData factionData)
    {
        switch (offenseType)
        {
            case ScenarioUnitType.Air:
                return factionData.galvadoreAirBuilding;
            case ScenarioUnitType.Infantry:
                return factionData.galvadoreInfantryBuilding;
            case ScenarioUnitType.Tank:
                return factionData.galvadoreTankBuilding;
            default:
                return null;
        }
    }

}
