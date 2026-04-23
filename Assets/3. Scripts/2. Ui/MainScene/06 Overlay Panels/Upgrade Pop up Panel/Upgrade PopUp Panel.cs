using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePopUpPanel : MonoBehaviour
{
    #region  References to UI elements
    [SerializeField] private Userdata userData;
    [SerializeField] private AllBuildingData allBuildingData;

    [SerializeField] private TMP_Text cardName;
    [SerializeField] private Image cardImage;

    [SerializeField] private Image unitTypeIcon;
    [SerializeField] private TMP_Text unitTypeText;

    [SerializeField] private Image buildingTypeIcon;
    [SerializeField] private TMP_Text buildingTypeText;

    [SerializeField] private TMP_Text cardLevelText;
    [SerializeField] private Image levelProgressFill;
    [SerializeField] private TMP_Text levelProgressText;

    [SerializeField] private List<StatBlock> statBlocks;
    [SerializeField] private int maxUpgradableStats;

    [Header("Button References")]
    [SerializeField] private Button actionButton;
    private TMP_Text buttonLabel;
    [SerializeField] private TMP_Text actionGemCostText;
    [SerializeField] private Button closeButton;

    [Header("Sprite references")]
    [SerializeField] private Sprite healthIcon;
    [SerializeField] private Sprite armorIcon;
    [SerializeField] private Sprite buildTimeIcon;
    [SerializeField] private Sprite speedIcon;
    [SerializeField] private Sprite damageIcon;
    [SerializeField] private Sprite fireRateIcon;
    [SerializeField] private Sprite attackRangeIcon;
    [SerializeField] private Sprite capacityIcon;
    [SerializeField] private Sprite deckIcon;
    [SerializeField] private Sprite populationIcon;
    [SerializeField] private Sprite resourceIcon;
    #endregion

    private UnitProduceStatsSO currentUnitSO;
    private BuildingDataSO currentBuildingSO;

    private CanvasGroup canvasGroup;
    private UpgradeCard currentUpgradeCard;

    private FactionName currentCardFaction;
    private CardState currentCardState;
    private int cardSpawnLevel;
    private int cardDeltaLevel;
    private int purchaseCost;

    private int currentFragments => userData.GetFragment(currentCardFaction);
    MainBuildingDataSO mainBuildingSO => allBuildingData.mainBuildingSO[(int)currentCardFaction];

    public Action<ScriptableObject> OnCardPurchased;

    private void Start()
    {
        buttonLabel = actionButton.GetComponentInChildren<TMP_Text>();

        closeButton.onClick.AddListener(OnClickClose);
        actionButton.onClick.AddListener(OnClickAction);
    }

    private void OnDestroy()
    {
        closeButton.onClick.RemoveListener(OnClickClose);
        actionButton.onClick.RemoveListener(OnClickAction);
    }
    private void OnClickAction()
    {
        if (currentBuildingSO == null && currentUnitSO == null)
            return;

        var cardDetails = currentBuildingSO == null ? currentUnitSO.cardDetails : currentBuildingSO.cardDetails;

        var cardState = cardDetails.cardState;

        switch (cardState)
        {
            case CardState.Unlocked:
                OnClickPurchase(purchaseCost);
                break;
            case CardState.Purchased:
                OnClickUpgrade();
                break;
        }
    }

    private void OnClickClose()
    {
        HidePanel();

        if (currentUpgradeCard != null)
        {
            currentUpgradeCard.RefreshAllCards();
            currentUpgradeCard.RefreshCardCosts();
        }

        currentUpgradeCard = null;
        currentBuildingSO = null;
        currentUnitSO = null;
        cardDeltaLevel = 0;
    }

    #region OnClickPurchase
    private void OnClickPurchase(int purchaseCost)
    {
        if (currentCardState != CardState.Unlocked)
        {
            Debug.LogError("Card is not unlocked");
        }

        if (userData.Diamonds < purchaseCost)
            return;

        userData.Diamonds -= purchaseCost;

        currentCardState = CardState.Purchased;

        if (currentBuildingSO != null)
        {
            currentBuildingSO.cardDetails.cardState = currentCardState;
            InitializeStatBlocks(currentBuildingSO);
            OnCardPurchased?.Invoke(currentBuildingSO);
        }
        else
        {
            currentUnitSO.cardDetails.cardState = currentCardState;
            InitializeStatBlocks(currentUnitSO);
            OnCardPurchased?.Invoke(currentUnitSO);
        }


#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(currentUnitSO);
        UnityEditor.AssetDatabase.SaveAssets();
#endif

        RefreshButtonState();
        UpdateProgressBar();
    }
    #endregion

    #region OnClickUpgrade
    private void OnClickUpgrade()
    {
        if (userData.Diamonds < currentUpgradeCard.cardUpgradeCost || currentFragments < currentUpgradeCard.cardFragmentCost)
            return;

        var mainSpawnLevel = mainBuildingSO.buildingIdentity.spawnLevel;

        if (currentBuildingSO != null && currentBuildingSO is MainBuildingDataSO currentMainSO)
        {
            // compare with player level for main buildings
            // var playerLevel = ;

            // if (mainSpawnLevel >= playerLevel + cardDeltaLevel )
            // {
            //     Debug.Log("<font=18>Main building level is too low");
            //     return;
            // }
        }
        else
        {
            if (cardSpawnLevel >= mainSpawnLevel + cardDeltaLevel)
            {
                Debug.Log($"<color=green>Cannot upgrade {currentUpgradeCard.upgradeDataSO.name} anymore. {currentCardFaction} Main building level is too low");
                return;
            }
        }

        userData.Diamonds -= currentUpgradeCard.cardUpgradeCost;
        userData.ConsumeFragments(currentCardFaction, currentUpgradeCard.cardFragmentCost);

        cardSpawnLevel++;
        cardSpawnLevel = Mathf.Clamp(cardSpawnLevel, 0, GameData.GameMaxObjectLevel - 1);

        if (currentBuildingSO != null)
        {
            currentBuildingSO.buildingIdentity.spawnLevel = cardSpawnLevel;
            InitializeStatBlocks(currentBuildingSO);
        }
        else
        {
            currentUnitSO.unitIdentity.spawnLevel = cardSpawnLevel;
            InitializeStatBlocks(currentUnitSO);
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(currentUnitSO);
        UnityEditor.AssetDatabase.SaveAssets();
#endif

        currentUpgradeCard.RefreshCardCosts();
        RefreshButtonState();
        UpdateProgressBar();
    }
    #endregion

    #region OpenActionPanel
    public void OpenActionPanel(BuildingDataSO dataSO)
    {
        ShowPanel();

        cardName.SetText($"{dataSO.buildingIdentity.name} ");
        cardImage.sprite = dataSO.buildingIcon;
        buildingTypeText.SetText($"{dataSO.buildingType}");

        currentBuildingSO = dataSO;
        currentUnitSO = null;

        currentCardFaction = dataSO.buildingIdentity.faction;
        currentCardState = dataSO.cardDetails.cardState;
        cardSpawnLevel = dataSO.buildingIdentity.spawnLevel;
        cardDeltaLevel = dataSO.cardDetails.deltaLevel;

        if (currentCardState == CardState.Unlocked)
        {
            var purchaseCost = dataSO.cardDetails.purchaseCost;
            UpdateCostDisplay(purchaseCost);
            actionButton.interactable = userData.Diamonds >= purchaseCost;
        }

        ToggleTypeIcon(buildingTypeIcon.gameObject);
        InitializeStatBlocks(dataSO);

        RefreshButtonState();
        UpdateProgressBar();
    }

    public void OpenActionPanel(UnitProduceStatsSO dataSO)
    {
        ShowPanel();

        cardName.SetText(dataSO.unitIdentity.name);
        cardImage.sprite = dataSO.unitIcon;
        unitTypeText.SetText(dataSO.unitType.ToString());

        currentUnitSO = dataSO;
        currentBuildingSO = null;

        currentCardFaction = dataSO.unitIdentity.faction;
        currentCardState = dataSO.cardDetails.cardState;
        cardSpawnLevel = dataSO.unitIdentity.spawnLevel;
        cardDeltaLevel = dataSO.cardDetails.deltaLevel;

        if (currentCardState == CardState.Unlocked)
            purchaseCost = dataSO.cardDetails.purchaseCost;

        ToggleTypeIcon(unitTypeIcon.gameObject);
        InitializeStatBlocks(dataSO);

        RefreshButtonState();
        UpdateProgressBar();
    }
    #endregion

    #region InitializeStatBlocks
    private void SetCommonBuildingStats(bool canUpgrade, BuildingUpgradeData currentStats, BuildingUpgradeData nextStats)
    {
        var currentHealth = currentStats.buildingBasicStats.maxHealth;
        var healthChange = Mathf.Abs(nextStats.buildingBasicStats.maxHealth - currentStats.buildingBasicStats.maxHealth);
        statBlocks[0].EnableBlock(canUpgrade);
        statBlocks[0].SetValues("Max Health", currentHealth, healthChange);
        statBlocks[0].SetIcon(healthIcon);

        var currentArmor = currentStats.buildingBasicStats.armor;
        var armorChange = Mathf.Abs(nextStats.buildingBasicStats.armor - currentStats.buildingBasicStats.armor);
        statBlocks[1].EnableBlock(canUpgrade);
        statBlocks[1].SetValues("Armor", currentArmor, armorChange);
        statBlocks[1].SetIcon(armorIcon);

        var currentBuildTime = currentStats.buildingBuildTime;
        var buildTimeChange = Mathf.Abs(nextStats.buildingBuildTime - currentStats.buildingBuildTime);
        statBlocks[2].EnableBlock(canUpgrade);
        statBlocks[2].SetValues("Build Time", currentBuildTime, buildTimeChange);
        statBlocks[2].SetIcon(buildTimeIcon);
    }

    private void DisableBlocksFrom(int startIndex)
    {
        for (int i = startIndex; i < statBlocks.Count; i++)
            statBlocks[i].DisableBlock();
    }

    internal void InitializeStatBlocks(BuildingDataSO buildingData)
    {
        cardLevelText.SetText($"Level {cardSpawnLevel + 1}");

        bool hasUpgrade = buildingData.cardDetails.cardState == CardState.Purchased && cardSpawnLevel < GameData.GameMaxObjectLevel;

        switch (buildingData)
        {
            case OffenseBuildingDataSO offenseBuilding:
                {
                    var currentLevelData = offenseBuilding.offenseBuildingUpgradeData[cardSpawnLevel];
                    OffenseBuildingUpgradeData nextLevelData = new();

                    if (cardSpawnLevel + 1 < offenseBuilding.offenseBuildingUpgradeData.Count)
                        nextLevelData = offenseBuilding.offenseBuildingUpgradeData[cardSpawnLevel + 1];

                    SetCommonBuildingStats(hasUpgrade, currentLevelData, nextLevelData);

                    var currentMaxUnits = currentLevelData.maxSpawnableUnits;
                    var maxUnitsChange = Mathf.Abs(nextLevelData.maxSpawnableUnits - currentLevelData.maxSpawnableUnits);
                    statBlocks[3].EnableBlock(hasUpgrade);
                    statBlocks[3].SetValues("Max Units", currentMaxUnits, maxUnitsChange);
                    statBlocks[3].SetIcon(populationIcon);

                    DisableBlocksFrom(4);
                }
                break;
            case MainBuildingDataSO mainBuilding:
                {
                    var currentLevelData = mainBuilding.mainBuildingUpgradeData[cardSpawnLevel];
                    MainBuildingUpgradeData nextLevelData = new();

                    if (cardSpawnLevel + 1 < mainBuilding.mainBuildingUpgradeData.Count)
                        nextLevelData = mainBuilding.mainBuildingUpgradeData[cardSpawnLevel + 1];

                    SetCommonBuildingStats(hasUpgrade, currentLevelData, nextLevelData);

                    var currentDeckSize = currentLevelData.maxDeckEquipCount;
                    var deckSizeChange = Mathf.Abs(nextLevelData.maxDeckEquipCount - currentLevelData.maxDeckEquipCount);
                    statBlocks[2].EnableBlock(hasUpgrade);
                    statBlocks[2].SetValues("Deck Size", currentDeckSize, deckSizeChange);
                    statBlocks[2].SetIcon(deckIcon);

                    var currentMaxPopulation = currentLevelData.maxPopulation;
                    var maxPopulationChange = Mathf.Abs(nextLevelData.maxPopulation - currentLevelData.maxPopulation);
                    statBlocks[3].EnableBlock(hasUpgrade);
                    statBlocks[3].SetValues("Max Population", currentMaxPopulation, maxPopulationChange);
                    statBlocks[3].SetIcon(populationIcon);

                    var currentStartResources = currentLevelData.starterResources;
                    var startResourcesChange = Mathf.Abs(nextLevelData.starterResources - currentLevelData.starterResources);
                    statBlocks[4].EnableBlock(hasUpgrade);
                    statBlocks[4].SetValues("Start Resources", currentStartResources, startResourcesChange);
                    statBlocks[4].SetIcon(resourceIcon);

                    DisableBlocksFrom(5);
                }
                break;

            case DefenseBuildingDataSO defenseBuilding:
                {
                    var currentLevelData = defenseBuilding.defenseBuildingUpgradeData[cardSpawnLevel];
                    DefenseBuildingUpgradeData nextLevelData = new();

                    if (cardSpawnLevel + 1 < defenseBuilding.defenseBuildingUpgradeData.Count)
                        nextLevelData = defenseBuilding.defenseBuildingUpgradeData[cardSpawnLevel + 1];

                    SetCommonBuildingStats(hasUpgrade, currentLevelData, nextLevelData);

                    if (defenseBuilding.defenseType == ScenarioDefenseType.Wall)
                    {
                        DisableBlocksFrom(3);
                        break;
                    }

                    var currentUnitDamage = currentLevelData.defenseAttackStats.damage;
                    var unitDamageChange = Mathf.Abs(nextLevelData.defenseAttackStats.damage - currentLevelData.defenseAttackStats.damage);
                    statBlocks[3].EnableBlock(hasUpgrade);
                    statBlocks[3].SetValues("Damage", currentUnitDamage, unitDamageChange);
                    statBlocks[3].SetIcon(damageIcon);

                    var currentBuildingDamage = currentLevelData.defenseAttackStats.buildingDamage;
                    var buildingDamageChange = Mathf.Abs(nextLevelData.defenseAttackStats.buildingDamage - currentLevelData.defenseAttackStats.buildingDamage);
                    statBlocks[4].EnableBlock(hasUpgrade);
                    statBlocks[4].SetValues("Building Damage", currentBuildingDamage, buildingDamageChange);
                    statBlocks[4].SetIcon(damageIcon);

                    var currentFireRate = currentLevelData.defenseAttackStats.fireRate;
                    var fireRateChange = Mathf.Abs(nextLevelData.defenseAttackStats.fireRate - currentLevelData.defenseAttackStats.fireRate);
                    statBlocks[5].EnableBlock(hasUpgrade);
                    statBlocks[5].SetValues("Fire Rate", currentFireRate, fireRateChange);
                    statBlocks[5].SetIcon(fireRateIcon);

                    var currentRange = currentLevelData.defenseRangeStats.attackRange;
                    var rangeChange = Mathf.Abs(nextLevelData.defenseRangeStats.attackRange - currentLevelData.defenseRangeStats.attackRange);
                    statBlocks[6].EnableBlock(hasUpgrade);
                    statBlocks[6].SetValues("Attack Range", currentRange, rangeChange);
                    statBlocks[6].SetIcon(attackRangeIcon);

                    DisableBlocksFrom(7);
                }
                break;

            case ResourceBuildingDataSO resourceBuilding:
                {
                    var currentLevelData = resourceBuilding.resourceBuildingUpgradeData[cardSpawnLevel];
                    ResourceBuildingUpgradeData nextLevelData = new();

                    if (cardSpawnLevel + 1 < resourceBuilding.resourceBuildingUpgradeData.Count)
                        nextLevelData = resourceBuilding.resourceBuildingUpgradeData[cardSpawnLevel + 1];

                    SetCommonBuildingStats(hasUpgrade, currentLevelData, nextLevelData);

                    var currentResourceGenerationAmount = currentLevelData.resourceAmountPerBatch;
                    var resourceGenerationAmountChange = Mathf.Abs(nextLevelData.resourceAmountPerBatch - currentLevelData.resourceAmountPerBatch);
                    statBlocks[3].EnableBlock(hasUpgrade);
                    statBlocks[3].SetValues("Amount", currentResourceGenerationAmount, resourceGenerationAmountChange);
                    statBlocks[3].SetIcon(resourceBuilding.buildingIcon);

                    var currentResourceCapacity = currentLevelData.resourceAmountCapacity;
                    var resourceCapacityChange = Mathf.Abs(nextLevelData.resourceAmountCapacity - currentLevelData.resourceAmountCapacity);
                    statBlocks[4].EnableBlock(hasUpgrade);
                    statBlocks[4].SetValues("Capacity", currentResourceCapacity, resourceCapacityChange);
                    statBlocks[4].SetIcon(capacityIcon);

                    DisableBlocksFrom(5);
                }
                break;
        }
    }

    internal void InitializeStatBlocks(UnitProduceStatsSO unitData)
    {
        cardLevelText.SetText($"Level {cardSpawnLevel + 1}");

        UnitUpgradeData currentLevelData = unitData.unitUpgradeData[cardSpawnLevel];
        UnitUpgradeData nextLevelData = new();

        bool hasUpgrade = unitData.cardDetails.cardState == CardState.Purchased && cardSpawnLevel < GameData.GameMaxObjectLevel;

        if (cardSpawnLevel + 1 < unitData.unitUpgradeData.Length)
            nextLevelData = unitData.unitUpgradeData[cardSpawnLevel + 1];

        var currentHealth = currentLevelData.unitBasicStats.maxHealth;
        var healthChange = Mathf.Abs(nextLevelData.unitBasicStats.maxHealth - currentLevelData.unitBasicStats.maxHealth);
        statBlocks[0].EnableBlock(hasUpgrade);
        statBlocks[0].SetValues("Max Health", currentHealth, healthChange);
        statBlocks[0].SetIcon(healthIcon);

        var currentArmor = currentLevelData.unitBasicStats.armor;
        var armorChange = Mathf.Abs(nextLevelData.unitBasicStats.armor - currentLevelData.unitBasicStats.armor);
        statBlocks[1].EnableBlock(hasUpgrade);
        statBlocks[1].SetValues("Armor", currentArmor, armorChange);
        statBlocks[1].SetIcon(armorIcon);

        var currentBuildTime = currentLevelData.unitSpawnTime;
        var buildTimeChange = Mathf.Abs(nextLevelData.unitSpawnTime - currentLevelData.unitSpawnTime);
        statBlocks[2].EnableBlock(hasUpgrade);
        statBlocks[2].SetValues("Spawn Time", currentBuildTime, buildTimeChange);
        statBlocks[2].SetIcon(buildTimeIcon);

        var currentMoveSpeed = currentLevelData.unitMobilityStats.moveSpeed;
        var moveSpeedChange = Mathf.Abs(nextLevelData.unitMobilityStats.moveSpeed - currentLevelData.unitMobilityStats.moveSpeed);
        statBlocks[3].EnableBlock(hasUpgrade);
        statBlocks[3].SetValues("Speed", currentMoveSpeed, moveSpeedChange);
        statBlocks[3].SetIcon(speedIcon);

        var currentUnitDamage = currentLevelData.unitAttackStats.damage;
        var unitDamageChange = Mathf.Abs(nextLevelData.unitAttackStats.damage - currentLevelData.unitAttackStats.damage);
        statBlocks[4].EnableBlock(hasUpgrade);
        statBlocks[4].SetValues("Unit Damage", currentUnitDamage, unitDamageChange);
        statBlocks[4].SetIcon(damageIcon);

        var currentBuildingDamage = currentLevelData.unitAttackStats.buildingDamage;
        var buildingDamageChange = Mathf.Abs(nextLevelData.unitAttackStats.buildingDamage - currentLevelData.unitAttackStats.buildingDamage);
        statBlocks[5].EnableBlock(hasUpgrade);
        statBlocks[5].SetValues("Building Damage", currentBuildingDamage, buildingDamageChange);
        statBlocks[5].SetIcon(damageIcon);

        var currentFireRate = currentLevelData.unitAttackStats.fireRate;
        var fireRateChange = Mathf.Abs(nextLevelData.unitAttackStats.fireRate - currentLevelData.unitAttackStats.fireRate);
        statBlocks[6].EnableBlock(hasUpgrade);
        statBlocks[6].SetValues("Fire Rate", currentFireRate, fireRateChange);
        statBlocks[6].SetIcon(fireRateIcon);

        var currentRange = currentLevelData.unitRangeStats.attackRange;
        var rangeChange = Mathf.Abs(nextLevelData.unitRangeStats.attackRange - currentLevelData.unitRangeStats.attackRange);
        statBlocks[7].EnableBlock(hasUpgrade);
        statBlocks[7].SetValues("Attack Range", currentRange, rangeChange);
        statBlocks[7].SetIcon(attackRangeIcon);
    }
    #endregion

    #region Helper Functions
    private void RefreshButtonState()
    {
        switch (currentCardState)
        {
            case CardState.Locked:
                buttonLabel.SetText("Locked");
                actionButton.interactable = false;
                actionGemCostText.transform.parent.gameObject.SetActive(false);
                return;
            case CardState.Unlocked:
                buttonLabel.SetText("Purchase");
                UpdateCostDisplay(purchaseCost);
                actionButton.interactable = userData.Diamonds >= purchaseCost;
                break;
            case CardState.Purchased:
                if (cardSpawnLevel + 1 == GameData.GameMaxObjectLevel)
                {
                    buttonLabel.SetText("Max Level");
                    actionButton.interactable = false;
                    actionGemCostText.transform.parent.gameObject.SetActive(false);
                    break;
                }
                buttonLabel.SetText("Upgrade");
                UpdateCostDisplay(currentUpgradeCard.cardUpgradeCost);
                actionButton.interactable = currentUpgradeCard.isUpgradable;
                break;
        }
    }

    private void UpdateProgressBar()
    {
        levelProgressFill.fillAmount = (float)currentFragments / currentUpgradeCard.cardFragmentCost;
        levelProgressText.SetText($"{currentFragments}/{currentUpgradeCard.cardFragmentCost}");
    }

    private void UpdateCostDisplay(int cost)
    {
        actionGemCostText.SetText($"{cost}");
        actionGemCostText.transform.parent.gameObject.SetActive(true);
    }
    private void ToggleTypeIcon(GameObject icon)
    {
        buildingTypeIcon.gameObject.SetActive(false);
        unitTypeIcon.gameObject.SetActive(false);
        icon.SetActive(true);
    }
    public void SetSelectedUpgradeCard(UpgradeCard card)
    {
        currentUpgradeCard = card;
    }

    private void ShowPanel()
    {
        if (canvasGroup == null)
            TryGetComponent(out canvasGroup);

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
    public void HidePanel()
    {
        if (canvasGroup == null)
            TryGetComponent(out canvasGroup);

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    #endregion
}