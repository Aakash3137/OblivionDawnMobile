using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePopUpPanel : MonoBehaviour
{
    [SerializeField] private Userdata userdata;

    [SerializeField] private TMP_Text cardName;
    [SerializeField] private Sprite cardImage;
    [SerializeField] private TMP_Text cardLevel;

    [SerializeField] private TMP_Text unitTypeText;
    [SerializeField] private TMP_Text buildingTypeText;

    [SerializeField] private List<StatBlock> statBlocks;
    [SerializeField] private int maxUpgradableStats;

    [Header("Button References")]
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button closeButton;

    private UnitProduceUpgrade unitUpgrade;
    private BuildingUpgrade buildingUpgradeData;


    public void Initialize(CardUpgradeData cardData)
    {
        if (statBlocks.Count != maxUpgradableStats)
        {
            Debug.Log("<color=green> [UpgradePopUpPanel] assign all stat blocks </color>");
            return;
        }


        if (cardData.buildingUpgradeData != null)
        {
            cardName.text = cardData.buildingUpgradeData.buildingIdentity.name;
            cardImage = cardData.buildingUpgradeData.buildingIcon;

            buildingTypeText.text = cardData.buildingUpgradeData.buildingType.ToString();
            cardLevel.SetText($"Level : {cardData.buildingUpgradeData.buildingIdentity.spawnLevel + 1}");
            InitializeStatBlocks(cardData.buildingUpgradeData);
        }
        else if (cardData.unitUpgradeData != null)
        {
            cardName.SetText(cardData.unitUpgradeData.unitIdentity.name);
            cardImage = cardData.unitUpgradeData.UnitIcon;

            unitTypeText.SetText(cardData.unitUpgradeData.unitType.ToString());
            cardLevel.SetText($"Level : {cardData.unitUpgradeData.unitIdentity.spawnLevel + 1}");
            InitializeStatBlocks(cardData.unitUpgradeData);
        }
        else
        {
            Debug.Log("<color=green> [UpgradePopUpPanel] Initialize failed Uint Stats and Building Stats are null</color>");
        }
    }

    private void InitializeStatBlocks(BuildingDataSO buildingData)
    {
        if (buildingData is OffenseBuildingDataSO offenseBuilding)
        {
            OffenseBuildingUpgradeData upgradeData = offenseBuilding.offenseBuildingUpgradeData[offenseBuilding.buildingIdentity.spawnLevel];

            statBlocks[0].EnableBlock();
            statBlocks[0].SetValues("Max Health", $"{upgradeData.buildingBasicStats.maxHealth}", "+500");

            statBlocks[1].EnableBlock();
            statBlocks[1].SetValues("Armor", $"{upgradeData.buildingBasicStats.armor}", "+50");

            statBlocks[2].EnableBlock();
            statBlocks[2].SetValues("Build Time", $"{upgradeData.buildingBuildTime}", "-0.1");

            statBlocks[3].DisableBlock();
            statBlocks[4].DisableBlock();
            statBlocks[5].DisableBlock();
        }
        else if (buildingData is MainBuildingDataSO mainBuilding)
        {
            MainBuildingUpgradeData upgradeData = mainBuilding.mainBuildingUpgradeData[mainBuilding.buildingIdentity.spawnLevel];

            statBlocks[0].EnableBlock();
            statBlocks[0].SetValues("Max Health", $"{upgradeData.buildingBasicStats.maxHealth}", "+500");

            statBlocks[1].EnableBlock();
            statBlocks[1].SetValues("Armor", $"{upgradeData.buildingBasicStats.armor}", "+50");

            statBlocks[2].EnableBlock();
            statBlocks[2].SetValues("Build Time", $"{upgradeData.buildingBuildTime}", "-0.1");

            statBlocks[3].DisableBlock();
            statBlocks[4].DisableBlock();
            statBlocks[5].DisableBlock();
        }
        else if (buildingData is DefenseBuildingDataSO defenseBuilding)
        {
            DefenseBuildingUpgradeData upgradeData = defenseBuilding.defenseBuildingUpgradeData[defenseBuilding.buildingIdentity.spawnLevel];

            statBlocks[0].EnableBlock();
            statBlocks[0].SetValues("Max Health", $"{upgradeData.buildingBasicStats.maxHealth}", "+500");

            statBlocks[1].EnableBlock();
            statBlocks[1].SetValues("Armor", $"{upgradeData.buildingBasicStats.armor}", "+50");

            statBlocks[2].EnableBlock();
            statBlocks[2].SetValues("Build Time", $"{upgradeData.buildingBuildTime}", "-0.1");

            statBlocks[3].EnableBlock();
            statBlocks[3].SetValues("Damage", $"{upgradeData.defenseAttackStats.damage}", "+5");

            statBlocks[4].EnableBlock();
            statBlocks[4].SetValues("Fire Rate", $"{upgradeData.defenseAttackStats.fireRate}", "-0.1");

            statBlocks[5].EnableBlock();
            statBlocks[5].SetValues("Attack Range", $"{upgradeData.defenseRangeStats.attackRange}", "+0.1");
        }
        else if (buildingData is ResourceBuildingDataSO resourceBuilding)
        {
            ResourceBuildingUpgradeData upgradeData = resourceBuilding.resourceBuildingUpgradeData[resourceBuilding.buildingIdentity.spawnLevel];

            statBlocks[0].EnableBlock();
            statBlocks[0].SetValues("Max Health", $"{upgradeData.buildingBasicStats.maxHealth}", "+500");

            statBlocks[1].EnableBlock();
            statBlocks[1].SetValues("Armor", $"{upgradeData.buildingBasicStats.armor}", "+50");

            statBlocks[2].EnableBlock();
            statBlocks[2].SetValues("Build Time", $"{upgradeData.buildingBuildTime}", "-0.1");

            statBlocks[3].EnableBlock();
            statBlocks[3].SetValues("Resource Per Tick", $"{upgradeData.resourceAmountPerBatch}", "+1");
            // statBlocks[3].OverrideIcon();

            statBlocks[4].EnableBlock();
            statBlocks[4].SetValues("Resource Capacity", $"{upgradeData.resourceAmountCapacity}", "+1");
            // statBlocks[4].OverrideIcon();

            statBlocks[5].DisableBlock();
        }
    }

    private void InitializeStatBlocks(UnitProduceStatsSO unitData)
    {

    }
}
