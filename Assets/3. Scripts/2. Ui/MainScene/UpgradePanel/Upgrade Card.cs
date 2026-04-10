using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour
{
    [Space(10)]
    [ReadOnly] public ScriptableObject upgradeDataSO;
    [Space(10)]

    #region References to UI elements
    [SerializeField] private Userdata userData;
    [SerializeField] private Image cardImage;
    [SerializeField] private Image levelProgressBar;
    [SerializeField] private TMP_Text levelProgressText;

    [SerializeField] private Button cardButton;

    [SerializeField] private TMP_Text populationCostText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private GameObject populationCostRoot;
    #endregion

    [HideInInspector] public CardsPanel myPanel;

    public int cardUpgradeCost { get; private set; }
    public int cardFragmentCost { get; private set; }


    private FactionName cardFaction;
    private int currentFragments => userData.GetFragment(cardFaction);
    public bool isUpgradable => userData.Diamonds >= cardUpgradeCost && currentFragments >= cardFragmentCost;

    private void Awake()
    {
        AddListeners();
    }
    public virtual void RefreshAllCards()
    {
        foreach (var upgradeCard in myPanel.allCards)
        {
            upgradeCard.UpdateLevelUI();
        }
    }

    public virtual void InitializeCard()
    {
        SetPersistentVariables();
        RefreshCardCosts();
        UpdateLevelUI();
    }

    public void RefreshCardCosts()
    {
        if (upgradeDataSO is BuildingDataSO buildingDataSO)
        {
            cardUpgradeCost = StatUpgrade.GetUpgradeCost(buildingDataSO.buildingIdentity.spawnLevel, buildingDataSO.cardDetails.upgradeCostMultiplier);
            cardFragmentCost = StatUpgrade.GetFragmentCost(buildingDataSO.buildingIdentity.spawnLevel, buildingDataSO.cardDetails.fragmentCostMultiplier);
        }
        else if (upgradeDataSO is UnitProduceStatsSO unitDataSO)
        {
            cardUpgradeCost = StatUpgrade.GetUpgradeCost(unitDataSO.unitIdentity.spawnLevel, unitDataSO.cardDetails.upgradeCostMultiplier);
            cardFragmentCost = StatUpgrade.GetFragmentCost(unitDataSO.unitIdentity.spawnLevel, unitDataSO.cardDetails.fragmentCostMultiplier);
        }
    }

    internal void SetPersistentVariables()
    {
        if (upgradeDataSO is BuildingDataSO buildingDataSO)
        {
            cardFaction = buildingDataSO.buildingIdentity.faction;
            Sprite icon = buildingDataSO.buildingIcon;
            cardImage.sprite = icon;
        }
        else if (upgradeDataSO is UnitProduceStatsSO unitDataSO)
        {
            cardFaction = unitDataSO.unitIdentity.faction;
            Sprite icon = unitDataSO.unitIcon;
            cardImage.sprite = icon;
        }
    }

    internal void UpdateLevelUI()
    {
        if (upgradeDataSO is BuildingDataSO buildingDataSO)
        {
            levelText.SetText($"Level {buildingDataSO.buildingIdentity.spawnLevel + 1}");

            if (buildingDataSO is DefenseBuildingDataSO defenseBuildingData)
            {
                populationCostText.SetText($"{defenseBuildingData.populationCost}");
            }
            else
            {
                populationCostRoot.SetActive(false);
            }

        }
        else if (upgradeDataSO is UnitProduceStatsSO unitDataSO)
        {
            populationCostText.SetText($"{unitDataSO.populationCost}");

            levelText.SetText($"Level {unitDataSO.unitIdentity.spawnLevel + 1}");
        }

        UpdateProgressBar();
    }

    internal void UpdateProgressBar()
    {
        levelProgressBar.fillAmount = (float)currentFragments / cardFragmentCost;
        levelProgressText.SetText($"{currentFragments}/{cardFragmentCost}");
    }

    private void OnCardClick()
    {
        var panel = UpgradePopUpPanel.Instance;

        if (panel == null)
        {
            Debug.Log("<color=green>[UpgradeCard] UpgradePopUpPanel instance not found</color>");
            return;
        }

        panel.SetSelectedUpgradeCard(this);

        if (upgradeDataSO is BuildingDataSO buildingDataSO)
        {
            Debug.Log($"<color=green> Clicked on {buildingDataSO.name} card</color>");
            panel.OpenActionPanel(buildingDataSO);
        }
        else if (upgradeDataSO is UnitProduceStatsSO unitDataSO)
        {
            Debug.Log($"<color=green> Clicked on {unitDataSO.name} card</color>");
            panel.OpenActionPanel(unitDataSO);
        }
        else
        {
            Debug.Log("<color=green> [CardUpgradeData]Imposter Scriptable Object not Unit SO or Building SO </color>");
        }
    }

    internal virtual void AddListeners()
    {
        cardButton.onClick.AddListener(OnCardClick);
    }
    internal virtual void RemoveListeners()
    {
        cardButton.onClick.RemoveListener(OnCardClick);
    }

    private void OnDestroy()
    {
        RemoveListeners();
    }
}
