using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BuildButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ScenarioBuildingType buildingType;
    [SerializeField] private TileUIPanel tileUIPanel;
    [SerializeField] private ScenarioDefenseType defenseType;
    [SerializeField] private ScenarioResourceType resourceType;
    [SerializeField] private ScenarioUnitType unitType;
    private Button button;

    [SerializeField] private GameObject costPanel;
    [SerializeField] private TMP_Text foodText;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text metalText;
    [SerializeField] private TMP_Text powerText;

    private void OnEnable()
    {
        PlayerResourceManager.Instance.OnResourcesChanged.AddListener(UpdateButtonInteractivity);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnClick);
        PlayerResourceManager.Instance.OnResourcesChanged.RemoveListener(UpdateButtonInteractivity);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        var slot = GetSlot(GameData.SelectedFaction, buildingType);
        BuildingUpgradeCost[] buildingUpgradeData = slot.prefab.GetComponent<BuildingStats>().buildingStats.buildingLevelData[0].buildingUpgradeCosts;

        foodText.text = buildingUpgradeData[0].resourceCost.ToString();
        goldText.text = buildingUpgradeData[1].resourceCost.ToString();
        metalText.text = buildingUpgradeData[2].resourceCost.ToString();
        powerText.text = buildingUpgradeData[3].resourceCost.ToString();

        if (buildingUpgradeData[0].resourceCost == 0)
            foodText.transform.parent.gameObject.SetActive(false);
        if (buildingUpgradeData[1].resourceCost == 0)
            goldText.transform.parent.gameObject.SetActive(false);
        if (buildingUpgradeData[2].resourceCost == 0)
            metalText.transform.parent.gameObject.SetActive(false);
        if (buildingUpgradeData[3].resourceCost == 0)
            powerText.transform.parent.gameObject.SetActive(false);

        costPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        costPanel.SetActive(false);

        foodText.transform.parent.gameObject.SetActive(true);
        goldText.transform.parent.gameObject.SetActive(true);
        metalText.transform.parent.gameObject.SetActive(true);
        powerText.transform.parent.gameObject.SetActive(true);
    }
    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        UpdateButtonInteractivity();
    }

    private void UpdateButtonInteractivity()
    {
        var slot = GetSlot(GameData.SelectedFaction, buildingType);
        BuildingUpgradeCost[] buildingUpgradeData = slot.prefab.GetComponent<BuildingStats>().buildingStats.buildingLevelData[0].buildingUpgradeCosts;

        if (buildingUpgradeData == null || !PlayerResourceManager.Instance.HasResources(buildingUpgradeData))
            button.interactable = false;
        else
            button.interactable = true;
    }

    void OnClick()
    {
        var slot = GetSlot(GameData.SelectedFaction, buildingType);
        if (slot != null && slot.prefab != null)
        {
            tileUIPanel.PlaceBuilding(slot);
        }
    }

    AllFactionsData.BuildingSlot GetSlot(FactionName faction, ScenarioBuildingType type)
    {
        var data = GameData.AllFactionsData;
        switch (faction)
        {
            case FactionName.Medieval:
                if (type == ScenarioBuildingType.MainBuilding) return data.pastMainBuilding;
                if (type == ScenarioBuildingType.DefenseBuilding) return data.pastTurretBuilding;
                if (type == ScenarioBuildingType.UnitBuilding) return data.pastInfantryBuilding;
                if (type == ScenarioBuildingType.ResourceBuilding) return GetMedievalResourceBuilding(data);
                break;
            case FactionName.Present:
                if (type == ScenarioBuildingType.MainBuilding) return data.presentMainBuilding;
                if (type == ScenarioBuildingType.DefenseBuilding) return data.presentTurretBuilding;
                if (type == ScenarioBuildingType.UnitBuilding) return data.presentInfantryBuilding;
                if (type == ScenarioBuildingType.ResourceBuilding) return GetPresentResourceBuilding(data);
                break;
            case FactionName.Futuristic:
                if (type == ScenarioBuildingType.MainBuilding) return data.futureMainBuilding;
                if (type == ScenarioBuildingType.DefenseBuilding) return data.futureTurretBuilding;
                if (type == ScenarioBuildingType.UnitBuilding) return data.futureInfantryBuilding;
                if (type == ScenarioBuildingType.ResourceBuilding) return GetFuturisticResourceBuilding(data);
                break;
            case FactionName.Galvadore:
                if (type == ScenarioBuildingType.MainBuilding) return data.monsterMainBuilding;
                if (type == ScenarioBuildingType.DefenseBuilding) return data.monsterTurretBuilding;
                if (type == ScenarioBuildingType.UnitBuilding) return data.monsterInfantryBuilding;
                if (type == ScenarioBuildingType.ResourceBuilding) return GetGalvadoreResourceBuilding(data);
                break;
        }
        return null;
    }

    private AllFactionsData.BuildingSlot GetMedievalResourceBuilding(AllFactionsData factionData)
    {
        switch (resourceType)
        {
            case ScenarioResourceType.Food:
                return factionData.pastFoodBuilding;
            case ScenarioResourceType.Gold:
                return factionData.pastGoldBuilding;
            case ScenarioResourceType.Metal:
                return factionData.pastMetalBuilding;
            case ScenarioResourceType.Power:
                return factionData.pastPowerBuilding;
            default:
                return null;
        }
    }
    private AllFactionsData.BuildingSlot GetPresentResourceBuilding(AllFactionsData factionData)
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

    private AllFactionsData.BuildingSlot GetFuturisticResourceBuilding(AllFactionsData factionData)
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

    private AllFactionsData.BuildingSlot GetGalvadoreResourceBuilding(AllFactionsData factionData)
    {
        switch (resourceType)
        {
            case ScenarioResourceType.Food:
                return factionData.monsterFoodBuilding;
            case ScenarioResourceType.Gold:
                return factionData.monsterGoldBuilding;
            case ScenarioResourceType.Metal:
                return factionData.monsterMetalBuilding;
            case ScenarioResourceType.Power:
                return factionData.monsterPowerBuilding;
            default:
                return null;
        }
    }

    private AllFactionsData.BuildingSlot GetMedievalDefenseBuilding(AllFactionsData factionData)
    {
        switch (defenseType)
        {
            case ScenarioDefenseType.AntiAir:
                return factionData.pastAntiAirBuilding;
            case ScenarioDefenseType.AntiTank:
                return factionData.pastAntiTankBuilding;
            case ScenarioDefenseType.Turret:
                return factionData.pastTurretBuilding;
            default:
                return null;
        }
    }

    private AllFactionsData.BuildingSlot GetPresentDefenseBuilding(AllFactionsData factionData)
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

    private AllFactionsData.BuildingSlot GetFuturisticDefenseBuilding(AllFactionsData factionData)
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

    private AllFactionsData.BuildingSlot GetGalvadoreDefenseBuilding(AllFactionsData factionData)
    {
        switch (defenseType)
        {
            case ScenarioDefenseType.AntiAir:
                return factionData.monsterAntiAirBuilding;
            case ScenarioDefenseType.AntiTank:
                return factionData.monsterAntiTankBuilding;
            case ScenarioDefenseType.Turret:
                return factionData.monsterTurretBuilding;
            default:
                return null;
        }
    }

    private AllFactionsData.BuildingSlot GetMedievalUnitBuilding(AllFactionsData factionData)
    {
        switch (unitType)
        {
            case ScenarioUnitType.Air:
                return factionData.pastAirBuilding;
            case ScenarioUnitType.Infantry:
                return factionData.pastInfantryBuilding;
            case ScenarioUnitType.Tank:
                return factionData.pastTankBuilding;
            default:
                return null;
        }
    }

    private AllFactionsData.BuildingSlot GetPresentUnitBuilding(AllFactionsData factionData)
    {
        switch (unitType)
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

    private AllFactionsData.BuildingSlot GetFuturisticUnitBuilding(AllFactionsData factionData)
    {
        switch (unitType)
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

    private AllFactionsData.BuildingSlot GetGalvadoreUnitBuilding(AllFactionsData factionData)
    {
        switch (unitType)
        {
            case ScenarioUnitType.Air:
                return factionData.monsterAirBuilding;
            case ScenarioUnitType.Infantry:
                return factionData.monsterInfantryBuilding;
            case ScenarioUnitType.Tank:
                return factionData.monsterTankBuilding;
            default:
                return null;
        }
    }

}
