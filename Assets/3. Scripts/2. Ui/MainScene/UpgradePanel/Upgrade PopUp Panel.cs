using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePopUpPanel : MonoBehaviour
{
    public static UpgradePopUpPanel Instance { get; protected set; }

    [SerializeField] protected Userdata userData;

    [SerializeField] protected TMP_Text cardName;
    [SerializeField] protected Image cardImage;

    [SerializeField] protected Image unitTypeIcon;
    [SerializeField] protected TMP_Text unitTypeText;

    [SerializeField] protected Image buildingTypeIcon;
    [SerializeField] protected TMP_Text buildingTypeText;

    [SerializeField] protected TMP_Text cardLevel;
    [SerializeField] protected Image levelProgressFill;
    [SerializeField] protected TMP_Text levelProgressText;

    [SerializeField] protected List<StatBlock> statBlocks;
    [SerializeField] protected int maxUpgradableStats;

    [Header("Button References")]
    [SerializeField] protected Button actionButton;
    private TMP_Text buttonLabel;
    [SerializeField] protected TMP_Text actionGemCostText;
    [SerializeField] protected Button closeButton;

    protected UnitProduceStatsSO currentUnitData;
    protected BuildingDataSO currentBuildingData;

    [Header("Sprite references")]
    [SerializeField] protected Sprite healthIcon;
    [SerializeField] protected Sprite armorIcon;
    [SerializeField] protected Sprite buildTimeIcon;
    [SerializeField] protected Sprite speedIcon;
    [SerializeField] protected Sprite damageIcon;
    [SerializeField] protected Sprite fireRateIcon;
    [SerializeField] protected Sprite attackRangeIcon;
    [SerializeField] protected Sprite capacityIcon;
    [SerializeField] protected Sprite deckIcon;
    [SerializeField] protected Sprite populationIcon;
    [SerializeField] protected Sprite resourceIcon;

    protected CanvasGroup canvasGroup;
    private UpgradeCard currentUpgradeCard;

    // private BuildingUpgrade buildingUpgrade;
    private UnitProduceUpgrade unitProduceUpgrade;

    protected void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // buildingUpgrade = new BuildingUpgrade();
        unitProduceUpgrade = new UnitProduceUpgrade();
    }

    protected void Start()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        HidePanel();

        buttonLabel = actionButton.GetComponentInChildren<TMP_Text>();

        closeButton.onClick.AddListener(OnClickClose);
        actionButton.onClick.AddListener(OnClickAction);
    }

    protected void OnDestroy()
    {
        closeButton.onClick.RemoveListener(OnClickClose);
        actionButton.onClick.RemoveListener(OnClickAction);
    }

    public void ShowPanel()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    public void HidePanel()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    protected void OnClickAction()
    {
        if (currentBuildingData != null)
        {
            if (!currentBuildingData.cardDetails.purchased)
                OnClickPurchase(currentBuildingData);
            else
                OnClickUpgrade(currentBuildingData);
        }
        else if (currentUnitData != null)
        {
            if (!currentUnitData.cardDetails.purchased)
                OnClickPurchase(currentUnitData);
            else
                OnClickUpgrade(currentUnitData);
        }
    }

    protected void OnClickClose()
    {
        HidePanel();

        if (currentUpgradeCard != null)
            currentUpgradeCard.RefreshAllCards();

        currentUpgradeCard = null;
        currentBuildingData = null;
        currentUnitData = null;
    }

    #region Helper Functions
    private void RefreshButtonState(CardDetails cardDetails, int spawnLevel, FactionName faction)
    {
        if (!cardDetails.isUnlocked)
        {
            actionButton.gameObject.SetActive(false);
            return;
        }

        actionButton.gameObject.SetActive(true);

        if (cardDetails.purchased)
        {
            int cost = StatUpgrade.UpgradeCost(spawnLevel + 1);
            int fragmentCost = StatUpgrade.FragmentCost(spawnLevel + 1);
            int currentFragments = userData.GetFragment((int)faction);

            UpdateCostDisplay(cost);
            buttonLabel.SetText("Upgrade");

            actionButton.interactable = userData.Diamonds >= cost && currentFragments >= fragmentCost;
        }
        else
        {
            int cost = StatUpgrade.UpgradeCost(spawnLevel);

            UpdateCostDisplay(cost);
            buttonLabel.SetText("Purchase");

            actionButton.interactable = userData.Diamonds >= cost;
        }
    }

    private void UpdateProgressBar(int spawnLevel, FactionName faction)
    {
        int fragmentCost = StatUpgrade.FragmentCost(spawnLevel + 1);
        int currentFragments = userData.GetFragment((int)faction);

        levelProgressFill.fillAmount = (float)currentFragments / fragmentCost;
        levelProgressText.SetText($"{currentFragments}/{fragmentCost}");
    }

    private void UpdateCostDisplay(int cost)
    {
        actionGemCostText.SetText(cost.ToString());
    }
    #endregion

    #region OnClickPurchase
    private void OnClickPurchase(BuildingDataSO data)
    {
        int cost = StatUpgrade.UpgradeCost(data.buildingIdentity.spawnLevel);

        if (userData.Diamonds < cost)
            return;

        userData.Diamonds -= cost;
        data.cardDetails.purchased = true;

        RefreshButtonState(data.cardDetails, data.buildingIdentity.spawnLevel, data.buildingIdentity.faction);
        UpdateProgressBar(data.buildingIdentity.spawnLevel, data.buildingIdentity.faction);

        InitializeStatBlocks(data);
    }

    private void OnClickPurchase(UnitProduceStatsSO data)
    {
        int cost = StatUpgrade.UpgradeCost(data.unitIdentity.spawnLevel);

        if (userData.Diamonds < cost)
            return;

        userData.Diamonds -= cost;
        data.cardDetails.purchased = true;

        RefreshButtonState(data.cardDetails, data.unitIdentity.spawnLevel, data.unitIdentity.faction);
        UpdateProgressBar(data.unitIdentity.spawnLevel, data.unitIdentity.faction);

        InitializeStatBlocks(data);
    }
    #endregion

    #region OnClickUpgrade
    private void OnClickUpgrade(BuildingDataSO data)
    {
        var identity = data.buildingIdentity;
        int cost = StatUpgrade.UpgradeCost(identity.spawnLevel + 1);
        int fragmentCost = StatUpgrade.FragmentCost(identity.spawnLevel + 1);

        if (userData.Diamonds < cost || userData.GetFragment((int)identity.faction) < fragmentCost)
            return;

        userData.Diamonds -= cost;
        userData.ConsumeFragments(identity.faction, fragmentCost);

        // buildingUpgrade.UpgradeNext(data);
        InitializeStatBlocks(data);

        RefreshButtonState(data.cardDetails, identity.spawnLevel, identity.faction);
        UpdateProgressBar(identity.spawnLevel, identity.faction);
    }

    private void OnClickUpgrade(UnitProduceStatsSO data)
    {
        var identity = data.unitIdentity;
        int cost = StatUpgrade.UpgradeCost(identity.spawnLevel + 1);
        int fragmentCost = StatUpgrade.FragmentCost(identity.spawnLevel + 1);

        if (userData.Diamonds < cost || userData.GetFragment((int)identity.faction) < fragmentCost)
            return;

        userData.Diamonds -= cost;
        userData.ConsumeFragments(identity.faction, fragmentCost);

        unitProduceUpgrade.UpgradeNext(data);
        InitializeStatBlocks(data);

        RefreshButtonState(data.cardDetails, identity.spawnLevel, identity.faction);
        UpdateProgressBar(identity.spawnLevel, identity.faction);
    }
    #endregion

    #region OpenActionPanel
    public void OpenActionPanel(BuildingDataSO dataSO)
    {
        if (statBlocks.Count != maxUpgradableStats)
        {
            Debug.LogError("[UpgradePopUpPanel] assign all stat blocks");
            return;
        }

        currentBuildingData = dataSO;
        currentUnitData = null;

        ShowPanel();

        cardName.text = dataSO.buildingIdentity.name;
        cardImage.sprite = dataSO.buildingIcon;
        buildingTypeText.text = dataSO.buildingType.ToString();

        ToggleTypeIcon(buildingTypeIcon.gameObject);
        InitializeStatBlocks(dataSO);

        RefreshButtonState(dataSO.cardDetails, dataSO.buildingIdentity.spawnLevel, dataSO.buildingIdentity.faction);
        UpdateProgressBar(dataSO.buildingIdentity.spawnLevel, dataSO.buildingIdentity.faction);
    }

    public void OpenActionPanel(UnitProduceStatsSO dataSO)
    {
        if (statBlocks.Count != maxUpgradableStats)
        {
            Debug.LogError("[UpgradePopUpPanel] assign all stat blocks");
            return;
        }

        currentUnitData = dataSO;
        currentBuildingData = null;

        ShowPanel();

        cardName.SetText(dataSO.unitIdentity.name);
        cardImage.sprite = dataSO.unitIcon;
        unitTypeText.SetText(dataSO.unitType.ToString());

        ToggleTypeIcon(unitTypeIcon.gameObject);
        InitializeStatBlocks(dataSO);

        RefreshButtonState(dataSO.cardDetails, dataSO.unitIdentity.spawnLevel, dataSO.unitIdentity.faction);
        UpdateProgressBar(dataSO.unitIdentity.spawnLevel, dataSO.unitIdentity.faction);
    }
    #endregion

    #region InitializeStatBlocks

    private void SetCommonBuildingStats(bool purchased, BuildingUpgradeData stats)
    {
        statBlocks[0].EnableBlock(purchased);
        statBlocks[0].SetValues("Max Health", $"{stats.buildingBasicStats.maxHealth}", "+500");
        statBlocks[0].SetIcon(healthIcon);

        statBlocks[1].EnableBlock(purchased);
        statBlocks[1].SetValues("Armor", $"{stats.buildingBasicStats.armor}", "+50");
        statBlocks[1].SetIcon(armorIcon);

        statBlocks[2].EnableBlock(purchased);
        statBlocks[2].SetValues("Build Time", $"{stats.buildingBuildTime}", "-0.1");
        statBlocks[2].SetIcon(buildTimeIcon);
    }

    private void DisableBlocksFrom(int startIndex)
    {
        for (int i = startIndex; i < statBlocks.Count; i++)
            statBlocks[i].DisableBlock();
    }

    internal void InitializeStatBlocks(BuildingDataSO buildingData)
    {
        int spawnLevel = buildingData.buildingIdentity.spawnLevel;
        cardLevel.SetText($"Level : {spawnLevel + 1}");

        bool purchased = buildingData.cardDetails.purchased;

        if (buildingData is OffenseBuildingDataSO offenseBuilding)
        {
            OffenseBuildingUpgradeData upgradeData = offenseBuilding.offenseBuildingUpgradeData[spawnLevel];

            SetCommonBuildingStats(purchased, upgradeData);
            DisableBlocksFrom(3);
        }
        else if (buildingData is MainBuildingDataSO mainBuilding)
        {
            MainBuildingUpgradeData upgradeData = mainBuilding.mainBuildingUpgradeData[spawnLevel];

            SetCommonBuildingStats(purchased, upgradeData);

            statBlocks[2].SetValues("Deck Size", $"{upgradeData.maxDeckEquipCount}", "+1");
            statBlocks[2].SetIcon(deckIcon);

            statBlocks[3].EnableBlock(purchased);
            statBlocks[3].SetValues("Max Population", $"{upgradeData.maxPopulation}", "+5");
            statBlocks[3].SetIcon(populationIcon);

            statBlocks[4].EnableBlock(purchased);
            statBlocks[4].SetValues("Start Resources", $"{upgradeData.starterResources}", "+100");
            statBlocks[4].SetIcon(resourceIcon);

            DisableBlocksFrom(5);
        }
        else if (buildingData is DefenseBuildingDataSO defenseBuilding)
        {
            DefenseBuildingUpgradeData upgradeData = defenseBuilding.defenseBuildingUpgradeData[spawnLevel];

            SetCommonBuildingStats(purchased, upgradeData);

            if (defenseBuilding.defenseType == ScenarioDefenseType.Wall)
            {
                DisableBlocksFrom(3);
                return;
            }

            statBlocks[3].EnableBlock(purchased);
            statBlocks[3].SetValues("Damage", $"{upgradeData.defenseAttackStats.damage}", "+5");
            statBlocks[3].SetIcon(damageIcon);

            statBlocks[4].EnableBlock(purchased);
            statBlocks[4].SetValues("Fire Rate", $"{upgradeData.defenseAttackStats.fireRate}", "-0.1");
            statBlocks[4].SetIcon(fireRateIcon);

            statBlocks[5].EnableBlock(purchased);
            statBlocks[5].SetValues("Attack Range", $"{upgradeData.defenseRangeStats.attackRange}", "+0.1");
            statBlocks[5].SetIcon(attackRangeIcon);

            DisableBlocksFrom(6);
        }
        else if (buildingData is ResourceBuildingDataSO resourceBuilding)
        {
            ResourceBuildingUpgradeData upgradeData = resourceBuilding.resourceBuildingUpgradeData[spawnLevel];

            SetCommonBuildingStats(purchased, upgradeData);

            statBlocks[3].EnableBlock(purchased);
            statBlocks[3].SetValues("Resource Per Tick", $"{upgradeData.resourceAmountPerBatch}", "+1");
            statBlocks[3].SetIcon(resourceBuilding.buildingIcon);

            statBlocks[4].EnableBlock(purchased);
            statBlocks[4].SetValues("Resource Capacity", $"{upgradeData.resourceAmountCapacity}", "+1");
            statBlocks[4].SetIcon(capacityIcon);

            DisableBlocksFrom(5);
        }
    }

    internal void InitializeStatBlocks(UnitProduceStatsSO unitData)
    {
        int spawnLevel = unitData.unitIdentity.spawnLevel;
        cardLevel.SetText($"Level : {spawnLevel + 1}");

        bool purchased = unitData.cardDetails.purchased;
        UnitUpgradeData upgradeData = unitData.unitUpgradeData[spawnLevel];

        statBlocks[0].EnableBlock(purchased);
        statBlocks[0].SetValues("Max Health", $"{upgradeData.unitBasicStats.maxHealth}", "+500");
        statBlocks[0].SetIcon(healthIcon);

        statBlocks[1].EnableBlock(purchased);
        statBlocks[1].SetValues("Armor", $"{upgradeData.unitBasicStats.armor}", "+50");
        statBlocks[1].SetIcon(armorIcon);

        statBlocks[2].EnableBlock(purchased);
        statBlocks[2].SetValues("Build Time", $"{upgradeData.unitBuildTime}", "-0.1");
        statBlocks[2].SetIcon(buildTimeIcon);

        statBlocks[3].EnableBlock(purchased);
        statBlocks[3].SetValues("Speed", $"{upgradeData.unitMobilityStats.moveSpeed}", "+5");
        statBlocks[3].SetIcon(speedIcon);

        statBlocks[4].EnableBlock(purchased);
        statBlocks[4].SetValues("Unit Damage", $"{upgradeData.unitAttackStats.damage}", "+5");
        statBlocks[4].SetIcon(damageIcon);

        statBlocks[5].EnableBlock(purchased);
        statBlocks[5].SetValues("Building Damage", $"{upgradeData.unitAttackStats.buildingDamage}", "+5");
        statBlocks[5].SetIcon(damageIcon);

        statBlocks[6].EnableBlock(purchased);
        statBlocks[6].SetValues("Fire Rate", $"{upgradeData.unitAttackStats.fireRate}", "-0.1");
        statBlocks[6].SetIcon(fireRateIcon);

        statBlocks[7].EnableBlock(purchased);
        statBlocks[7].SetValues("Attack Range", $"{upgradeData.unitRangeStats.attackRange}", "+0.1");
        statBlocks[7].SetIcon(attackRangeIcon);
    }
    #endregion

    protected void ToggleTypeIcon(GameObject icon)
    {
        buildingTypeIcon.gameObject.SetActive(false);
        unitTypeIcon.gameObject.SetActive(false);
        icon.SetActive(true);
    }

    public void SetSelectedUpgradeCard(UpgradeCard card)
    {
        currentUpgradeCard = card;
    }
}