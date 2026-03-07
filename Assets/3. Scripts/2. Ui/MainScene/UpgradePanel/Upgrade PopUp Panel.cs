using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePopUpPanel : MonoBehaviour
{
    public static UpgradePopUpPanel Instance { get; protected set; }

    [SerializeField] protected Userdata userdata;

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
    protected CanvasGroup canvasGroup;

    private UpgradeCard currentUpgradeCard;

    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected void Start()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

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

        UpgradePanelNavigation.Instance.UpdateFragmentsCount();

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
        }
        else
        {
            int cost = StatUpgrade.UpgradeCost(spawnLevel);

            actionButton.gameObject.SetActive(true);

            if (cardDetails.purchased)
            {
                cost = StatUpgrade.UpgradeCost(spawnLevel + 1);
                int fragmentCost = StatUpgrade.FragmentCost(spawnLevel + 1);

                UpdateCostDisplay(cost);
                buttonLabel.SetText("Upgrade");

                if (userdata.Diamonds < cost || userdata.fragments[(int)faction] < fragmentCost)
                    actionButton.interactable = false;
                else
                    actionButton.interactable = true;
            }
            else
            {
                UpdateCostDisplay(cost);
                buttonLabel.SetText("Purchase");

                if (userdata.Diamonds < cost)
                    actionButton.interactable = false;
                else
                    actionButton.interactable = true;
            }
        }
    }

    private void UpdateProgressBar(int spawnLevel, FactionName faction)
    {
        float progress = (float)userdata.fragments[(int)faction] / StatUpgrade.FragmentCost(spawnLevel + 1);
        levelProgressFill.fillAmount = progress;
        levelProgressText.SetText($"{userdata.fragments[(int)faction]}/{StatUpgrade.FragmentCost(spawnLevel + 1)}");
    }
    private void UpdateCostDisplay(int cost)
    {
        actionGemCostText.text = cost.ToString();
    }
    #endregion

    #region OnClickPurchase
    private void OnClickPurchase(BuildingDataSO data)
    {
        int cost = StatUpgrade.UpgradeCost(data.buildingIdentity.spawnLevel);

        if (userdata.Diamonds < cost)
            return;

        userdata.Diamonds -= cost;

        data.cardDetails.purchased = true;

        RefreshButtonState(data.cardDetails, data.buildingIdentity.spawnLevel, data.buildingIdentity.faction);
        UpdateProgressBar(data.buildingIdentity.spawnLevel, data.buildingIdentity.faction);

        InitializeStatBlocks(data);
    }
    // using polymorphism
    private void OnClickPurchase(UnitProduceStatsSO data)
    {
        int cost = StatUpgrade.UpgradeCost(data.unitIdentity.spawnLevel);

        if (userdata.Diamonds < cost)
            return;

        userdata.Diamonds -= cost;

        data.cardDetails.purchased = true;

        RefreshButtonState(data.cardDetails, data.unitIdentity.spawnLevel, data.unitIdentity.faction);
        UpdateProgressBar(data.unitIdentity.spawnLevel, data.unitIdentity.faction);

        InitializeStatBlocks(data);
    }
    #endregion

    #region OnClickUpgrade
    private void OnClickUpgrade(BuildingDataSO data)
    {
        int cost = StatUpgrade.UpgradeCost(data.buildingIdentity.spawnLevel + 1);
        int fragmentCost = StatUpgrade.FragmentCost(data.buildingIdentity.spawnLevel + 1);

        if (userdata.Diamonds < cost || userdata.fragments[(int)data.buildingIdentity.faction] < fragmentCost)
            return;

        userdata.Diamonds -= cost;
        userdata.fragments[(int)data.buildingIdentity.faction] -= fragmentCost;

        new BuildingUpgrade().UpgradeNext(data);
        InitializeStatBlocks(data);

        RefreshButtonState(data.cardDetails, data.buildingIdentity.spawnLevel, data.buildingIdentity.faction);
        UpdateProgressBar(data.buildingIdentity.spawnLevel, data.buildingIdentity.faction);
    }
    // using polymorphism
    private void OnClickUpgrade(UnitProduceStatsSO data)
    {
        int cost = StatUpgrade.UpgradeCost(data.unitIdentity.spawnLevel + 1);
        int fragmentCost = StatUpgrade.FragmentCost(data.unitIdentity.spawnLevel + 1);

        if (userdata.Diamonds < cost || userdata.fragments[(int)data.unitIdentity.faction] < fragmentCost)
            return;

        userdata.Diamonds -= cost;
        userdata.fragments[(int)data.unitIdentity.faction] -= fragmentCost;

        new UnitProduceUpgrade().UpgradeNext(data);
        InitializeStatBlocks(data);

        RefreshButtonState(data.cardDetails, data.unitIdentity.spawnLevel, data.unitIdentity.faction);
        UpdateProgressBar(data.unitIdentity.spawnLevel, data.unitIdentity.faction);
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

        // clear previous unit data
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

        // clear previous building data
        currentBuildingData = null;

        ShowPanel();

        cardName.SetText(dataSO.unitIdentity.name);
        cardImage.sprite = dataSO.UnitIcon;
        unitTypeText.SetText(dataSO.unitType.ToString());

        ToggleTypeIcon(unitTypeIcon.gameObject);
        InitializeStatBlocks(dataSO);

        RefreshButtonState(dataSO.cardDetails, dataSO.unitIdentity.spawnLevel, dataSO.unitIdentity.faction);
        UpdateProgressBar(dataSO.unitIdentity.spawnLevel, dataSO.unitIdentity.faction);
    }
    #endregion

    #region InitializeStatBlocks
    internal void InitializeStatBlocks(BuildingDataSO buildingData)
    {
        cardLevel.SetText($"Level : {buildingData.buildingIdentity.spawnLevel + 1}");
        bool purchased = buildingData.cardDetails.purchased;

        if (buildingData is OffenseBuildingDataSO offenseBuilding)
        {
            OffenseBuildingUpgradeData upgradeData = offenseBuilding.offenseBuildingUpgradeData[offenseBuilding.buildingIdentity.spawnLevel];

            statBlocks[0].EnableBlock(purchased);
            statBlocks[0].SetValues("Max Health", $"{upgradeData.buildingBasicStats.maxHealth}", "+500");
            statBlocks[0].SetIcon(healthIcon);

            statBlocks[1].EnableBlock(purchased);
            statBlocks[1].SetValues("Armor", $"{upgradeData.buildingBasicStats.armor}", "+50");
            statBlocks[1].SetIcon(armorIcon);

            statBlocks[2].EnableBlock(purchased);
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

            statBlocks[0].EnableBlock(purchased);
            statBlocks[0].SetValues("Max Health", $"{upgradeData.buildingBasicStats.maxHealth}", "+500");
            statBlocks[0].SetIcon(healthIcon);

            statBlocks[1].EnableBlock(purchased);
            statBlocks[1].SetValues("Armor", $"{upgradeData.buildingBasicStats.armor}", "+50");
            statBlocks[1].SetIcon(armorIcon);

            statBlocks[2].EnableBlock(purchased);
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

            statBlocks[0].EnableBlock(purchased);
            statBlocks[0].SetValues("Max Health", $"{upgradeData.buildingBasicStats.maxHealth}", "+500");
            statBlocks[0].SetIcon(healthIcon);

            statBlocks[1].EnableBlock(purchased);
            statBlocks[1].SetValues("Armor", $"{upgradeData.buildingBasicStats.armor}", "+50");
            statBlocks[1].SetIcon(armorIcon);

            statBlocks[2].EnableBlock(purchased);
            statBlocks[2].SetValues("Build Time", $"{upgradeData.buildingBuildTime}", "-0.1");
            statBlocks[2].SetIcon(buildTimeIcon);

            statBlocks[3].EnableBlock(purchased);
            statBlocks[3].SetValues("Damage", $"{upgradeData.defenseAttackStats.damage}", "+5");
            statBlocks[3].SetIcon(damageIcon);

            statBlocks[4].EnableBlock(purchased);
            statBlocks[4].SetValues("Fire Rate", $"{upgradeData.defenseAttackStats.fireRate}", "-0.1");
            statBlocks[4].SetIcon(fireRateIcon);

            statBlocks[5].EnableBlock(purchased);
            statBlocks[5].SetValues("Attack Range", $"{upgradeData.defenseRangeStats.attackRange}", "+0.1");
            statBlocks[5].SetIcon(attackRangeIcon);

            statBlocks[6].DisableBlock();
            statBlocks[7].DisableBlock();
        }
        else if (buildingData is ResourceBuildingDataSO resourceBuilding)
        {
            ResourceBuildingUpgradeData upgradeData = resourceBuilding.resourceBuildingUpgradeData[resourceBuilding.buildingIdentity.spawnLevel];

            statBlocks[0].EnableBlock(purchased);
            statBlocks[0].SetValues("Max Health", $"{upgradeData.buildingBasicStats.maxHealth}", "+500");
            statBlocks[0].SetIcon(healthIcon);

            statBlocks[1].EnableBlock(purchased);
            statBlocks[1].SetValues("Armor", $"{upgradeData.buildingBasicStats.armor}", "+50");
            statBlocks[1].SetIcon(armorIcon);

            statBlocks[2].EnableBlock(purchased);
            statBlocks[2].SetValues("Build Time", $"{upgradeData.buildingBuildTime}", "-0.1");
            statBlocks[2].SetIcon(buildTimeIcon);

            statBlocks[3].EnableBlock(purchased);
            statBlocks[3].SetValues("Resource Per Tick", $"{upgradeData.resourceAmountPerBatch}", "+1");
            statBlocks[3].SetIcon(resourceBuilding.buildingIcon);

            statBlocks[4].EnableBlock(purchased);
            statBlocks[4].SetValues("Resource Capacity", $"{upgradeData.resourceAmountCapacity}", "+1");
            statBlocks[4].SetIcon(capacityIcon);

            statBlocks[5].DisableBlock();
            statBlocks[6].DisableBlock();
            statBlocks[7].DisableBlock();
        }
    }

    internal void InitializeStatBlocks(UnitProduceStatsSO unitData)
    {
        cardLevel.SetText($"Level : {unitData.unitIdentity.spawnLevel + 1}");
        bool purchased = unitData.cardDetails.purchased;

        UnitUpgradeData upgradeData = unitData.unitUpgradeData[unitData.unitIdentity.spawnLevel];

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