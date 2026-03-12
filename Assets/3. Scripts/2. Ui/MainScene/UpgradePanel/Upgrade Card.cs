using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour
{
    [ReadOnly] public ScriptableObject upgradeDataSO;

    [SerializeField] private Userdata userData;
    [SerializeField] private Image cardImage;
    [SerializeField] private Image levelProgressBar;
    [SerializeField] private TMP_Text levelProgressText;

    [SerializeField] private Button cardButton;

    [SerializeField] private TMP_Text populationCostText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private GameObject populationCostRoot;

    [HideInInspector] public CardsPanel myPanel;

    private void Awake()
    {
        cardButton.onClick.AddListener(OnCardClick);
    }

    private void SetCardSprite()
    {
        if (upgradeDataSO is BuildingDataSO buildingDataSO)
        {
            cardImage.sprite = buildingDataSO.buildingIcon;

            levelText.SetText($"Level {buildingDataSO.buildingIdentity.spawnLevel + 1}");

            if (buildingDataSO is DefenseBuildingDataSO defenseBuildingData)
            {
                populationCostText.SetText($"{defenseBuildingData.populationCost}");
            }
            else
            {
                populationCostRoot.SetActive(false);
            }

            UpdateProgressBar(buildingDataSO.buildingIdentity.spawnLevel, buildingDataSO.buildingIdentity.faction);
        }
        else if (upgradeDataSO is UnitProduceStatsSO unitDataSO)
        {
            cardImage.sprite = unitDataSO.unitIcon;

            populationCostText.SetText($"{unitDataSO.populationCost}");

            levelText.SetText($"Level {unitDataSO.unitIdentity.spawnLevel + 1}");

            UpdateProgressBar(unitDataSO.unitIdentity.spawnLevel, unitDataSO.unitIdentity.faction);
        }
    }
    public void RefreshAllCards()
    {
        foreach (var card in myPanel.allCards)
            card.SetCardSprite();
    }

    public void RefreshCard()
    {
        SetCardSprite();
    }
    private void UpdateProgressBar(int spawnLevel, FactionName faction)
    {
        int fragmentCost = StatUpgrade.FragmentCost(spawnLevel + 1);
        int currentFragments = userData.fragments[(int)faction];
        levelProgressBar.fillAmount = (float)currentFragments / fragmentCost;
        levelProgressText.SetText($"{currentFragments}/{fragmentCost}");
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
            Debug.Log("<color=green> [CardUpgradeData] Initialize failed Uint Stats and Building Stats are null</color>");
        }
    }

    private void OnDestroy()
    {
        cardButton.onClick.RemoveListener(OnCardClick);
    }
}
