using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePopUpPanel : MonoBehaviour
{
    [SerializeField] private Userdata userdata;

    [SerializeField] private TMP_Text cardName;
    [SerializeField] private Image cardImage;

    [SerializeField] private Image unitTypeIcon;
    [SerializeField] private TMP_Text unitTypeText;

    [SerializeField] private Image buildingTypeIcon;
    [SerializeField] private TMP_Text buildingTypeText;


    [SerializeField] private TMP_Text cardLevel;
    [SerializeField] private Image levelProgressBar;

    [SerializeField] private List<StatBlock> statBlocks;
    [SerializeField] private int maxUpgradableStats;

    [Header("Button References")]
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button closeButton;

    private UnitProduceUpgrade unitUpgrade;
    private BuildingUpgrade buildingUpgradeData;

    [Header("Sprite references")]
    [SerializeField] private Sprite healthIcon;
    [SerializeField] private Sprite armorIcon;
    [SerializeField] private Sprite buildTimeIcon;
    [SerializeField] private Sprite speedIcon;
    [SerializeField] private Sprite damageIcon;
    [SerializeField] private Sprite fireRateIcon;
    [SerializeField] private Sprite attackRangeIcon;
    [SerializeField] private Sprite resourceIcon;
    [SerializeField] private Sprite capacityIcon;

    public void OpenPopUpPanel(BuildingDataSO dataSO)
    {
        if (statBlocks.Count != maxUpgradableStats)
        {
            Debug.Log("<color=green> [UpgradePopUpPanel] assign all stat blocks </color>");
            return;
        }
        gameObject.SetActive(true);

        if (dataSO != null)
        {
            cardName.text = dataSO.buildingIdentity.name;
            cardImage.sprite = dataSO.buildingIcon;

            buildingTypeText.text = dataSO.buildingType.ToString();
            cardLevel.SetText($"Level : {dataSO.buildingIdentity.spawnLevel + 1}");
            InitializeStatBlocks(dataSO);
        }

        ToggleIconType(buildingTypeIcon.gameObject);
    }

    public void OpenPopUpPanel(UnitProduceStatsSO dataSO)
    {
        if (statBlocks.Count != maxUpgradableStats)
        {
            Debug.Log("<color=green> [UpgradePopUpPanel] assign all stat blocks </color>");
            return;
        }
        gameObject.SetActive(true);

        if (dataSO != null)
        {
            cardName.SetText(dataSO.unitIdentity.name);
            cardImage.sprite = dataSO.UnitIcon;

            unitTypeText.SetText(dataSO.unitType.ToString());
            cardLevel.SetText($"Level : {dataSO.unitIdentity.spawnLevel + 1}");
            InitializeStatBlocks(dataSO);
        }

        ToggleIconType(unitTypeIcon.gameObject);
    }

    private void OnClickClose()
    {
        gameObject.SetActive(false);
    }

    private void OnClickUpgrade()
    {

    }

    private void InitializeStatBlocks(BuildingDataSO buildingData)
    {
        if (buildingData is OffenseBuildingDataSO offenseBuilding)
        {
            OffenseBuildingUpgradeData upgradeData = offenseBuilding.offenseBuildingUpgradeData[offenseBuilding.buildingIdentity.spawnLevel];

            statBlocks[0].EnableBlock();
            statBlocks[0].SetValues("Max Health", $"{upgradeData.buildingBasicStats.maxHealth}", "+500");
            statBlocks[0].SetIcon(healthIcon);

            statBlocks[1].EnableBlock();
            statBlocks[1].SetValues("Armor", $"{upgradeData.buildingBasicStats.armor}", "+50");
            statBlocks[1].SetIcon(armorIcon);

            statBlocks[2].EnableBlock();
            statBlocks[2].SetValues("Build Time", $"{upgradeData.buildingBuildTime}", "-0.1");
            statBlocks[2].SetIcon(buildTimeIcon);

            statBlocks[3].DisableBlock();
            statBlocks[4].DisableBlock();
            statBlocks[5].DisableBlock();
            statBlocks[6].DisableBlock();
            statBlocks[7].DisableBlock();
        }
        else if (buildingData is MainBuildingDataSO mainBuilding)
        {
            MainBuildingUpgradeData upgradeData = mainBuilding.mainBuildingUpgradeData[mainBuilding.buildingIdentity.spawnLevel];

            statBlocks[0].EnableBlock();
            statBlocks[0].SetValues("Max Health", $"{upgradeData.buildingBasicStats.maxHealth}", "+500");
            statBlocks[0].SetIcon(healthIcon);

            statBlocks[1].EnableBlock();
            statBlocks[1].SetValues("Armor", $"{upgradeData.buildingBasicStats.armor}", "+50");
            statBlocks[1].SetIcon(armorIcon);

            statBlocks[2].EnableBlock();
            statBlocks[2].SetValues("Build Time", $"{upgradeData.buildingBuildTime}", "-0.1");
            statBlocks[2].SetIcon(buildTimeIcon);

            statBlocks[3].DisableBlock();
            statBlocks[4].DisableBlock();
            statBlocks[5].DisableBlock();
            statBlocks[6].DisableBlock();
            statBlocks[7].DisableBlock();

        }
        else if (buildingData is DefenseBuildingDataSO defenseBuilding)
        {
            DefenseBuildingUpgradeData upgradeData = defenseBuilding.defenseBuildingUpgradeData[defenseBuilding.buildingIdentity.spawnLevel];

            statBlocks[0].EnableBlock();
            statBlocks[0].SetValues("Max Health", $"{upgradeData.buildingBasicStats.maxHealth}", "+500");
            statBlocks[0].SetIcon(healthIcon);

            statBlocks[1].EnableBlock();
            statBlocks[1].SetValues("Armor", $"{upgradeData.buildingBasicStats.armor}", "+50");
            statBlocks[1].SetIcon(armorIcon);

            statBlocks[2].EnableBlock();
            statBlocks[2].SetValues("Build Time", $"{upgradeData.buildingBuildTime}", "-0.1");
            statBlocks[2].SetIcon(buildTimeIcon);

            statBlocks[3].EnableBlock();
            statBlocks[3].SetValues("Damage", $"{upgradeData.defenseAttackStats.damage}", "+5");
            statBlocks[3].SetIcon(damageIcon);

            statBlocks[4].EnableBlock();
            statBlocks[4].SetValues("Fire Rate", $"{upgradeData.defenseAttackStats.fireRate}", "-0.1");
            statBlocks[4].SetIcon(fireRateIcon);

            statBlocks[5].EnableBlock();
            statBlocks[5].SetValues("Attack Range", $"{upgradeData.defenseRangeStats.attackRange}", "+0.1");
            statBlocks[5].SetIcon(attackRangeIcon);

            statBlocks[6].DisableBlock();
            statBlocks[7].DisableBlock();
        }
        else if (buildingData is ResourceBuildingDataSO resourceBuilding)
        {
            ResourceBuildingUpgradeData upgradeData = resourceBuilding.resourceBuildingUpgradeData[resourceBuilding.buildingIdentity.spawnLevel];

            statBlocks[0].EnableBlock();
            statBlocks[0].SetValues("Max Health", $"{upgradeData.buildingBasicStats.maxHealth}", "+500");
            statBlocks[0].SetIcon(healthIcon);

            statBlocks[1].EnableBlock();
            statBlocks[1].SetValues("Armor", $"{upgradeData.buildingBasicStats.armor}", "+50");
            statBlocks[1].SetIcon(armorIcon);

            statBlocks[2].EnableBlock();
            statBlocks[2].SetValues("Build Time", $"{upgradeData.buildingBuildTime}", "-0.1");
            statBlocks[2].SetIcon(buildTimeIcon);

            statBlocks[3].EnableBlock();
            statBlocks[3].SetValues("Resource Per Tick", $"{upgradeData.resourceAmountPerBatch}", "+1");
            statBlocks[3].SetIcon(resourceBuilding.buildingIcon);

            statBlocks[4].EnableBlock();
            statBlocks[4].SetValues("Resource Capacity", $"{upgradeData.resourceAmountCapacity}", "+1");
            statBlocks[4].SetIcon(capacityIcon);

            statBlocks[5].DisableBlock();
            statBlocks[6].DisableBlock();
            statBlocks[7].DisableBlock();
        }
    }

    private void InitializeStatBlocks(UnitProduceStatsSO unitData)
    {
        UnitUpgradeData upgradeData = unitData.unitUpgradeData[unitData.unitIdentity.spawnLevel];

        statBlocks[0].EnableBlock();
        statBlocks[0].SetValues("Max Health", $"{upgradeData.unitBasicStats.maxHealth}", "+500");
        statBlocks[0].SetIcon(healthIcon);

        statBlocks[1].EnableBlock();
        statBlocks[1].SetValues("Armor", $"{upgradeData.unitBasicStats.armor}", "+50");
        statBlocks[1].SetIcon(armorIcon);

        statBlocks[2].EnableBlock();
        statBlocks[2].SetValues("Build Time", $"{upgradeData.unitBuildTime}", "-0.1");
        statBlocks[2].SetIcon(buildTimeIcon);

        statBlocks[3].EnableBlock();
        statBlocks[3].SetValues("Speed", $"{upgradeData.unitMobilityStats.moveSpeed}", "+5");
        statBlocks[3].SetIcon(speedIcon);

        statBlocks[4].EnableBlock();
        statBlocks[4].SetValues("Unit Damage", $"{upgradeData.unitAttackStats.damage}", "+5");
        statBlocks[4].SetIcon(damageIcon);

        statBlocks[5].EnableBlock();
        statBlocks[5].SetValues("Building Damage", $"{upgradeData.unitAttackStats.buildingDamage}", "+5");
        statBlocks[5].SetIcon(damageIcon);

        statBlocks[6].EnableBlock();
        statBlocks[6].SetValues("Fire Rate", $"{upgradeData.unitAttackStats.fireRate}", "-0.1");
        statBlocks[6].SetIcon(fireRateIcon);

        statBlocks[7].EnableBlock();
        statBlocks[7].SetValues("Attack Range", $"{upgradeData.unitRangeStats.attackRange}", "+0.1");
        statBlocks[7].SetIcon(attackRangeIcon);
    }

    private void ToggleIconType(GameObject icon)
    {
        buildingTypeIcon.gameObject.SetActive(false);
        unitTypeIcon.gameObject.SetActive(false);

        icon.SetActive(true);
    }

    private void OnEnable()
    {
        closeButton.onClick.AddListener(OnClickClose);
        upgradeButton.onClick.AddListener(OnClickUpgrade);
    }
    private void OnDisable()
    {
        closeButton.onClick.RemoveListener(OnClickClose);
        upgradeButton.onClick.RemoveListener(OnClickUpgrade);
    }
}
