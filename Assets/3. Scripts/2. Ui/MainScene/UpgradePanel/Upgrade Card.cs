using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour
{
    [ReadOnly] public BuildingDataSO buildingUpgradeData;
    [ReadOnly] public UnitProduceStatsSO unitUpgradeData;

    [SerializeField] private Userdata userdata;
    [SerializeField] private Image cardImage;
    [SerializeField] private Image levelProgressBar;
    [SerializeField] private TMP_Text levelProgressText;

    [SerializeField] private Button cardButton;

    [SerializeField] private TMP_Text populationCostText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private GameObject populationCostRoot;

    [HideInInspector] public CardsPanel myPanel;

    private void Start()
    {
        SetCardSprite();
        cardButton.onClick.AddListener(OnCardClick);
    }

    private void SetCardSprite()
    {
        if (buildingUpgradeData != null)
        {
            cardImage.sprite = buildingUpgradeData.buildingIcon;

            if (populationCostRoot == null && populationCostText == null)
                return;

            levelText.SetText($"Level {buildingUpgradeData.buildingIdentity.spawnLevel + 1}");

            if (buildingUpgradeData is DefenseBuildingDataSO defenseBuildingData)
            {
                populationCostText.SetText($"{defenseBuildingData.buildingIdentity.populationCost}");
            }
            else
            {
                populationCostRoot.SetActive(false);
            }

            float progress = (float)userdata.fragments[(int)buildingUpgradeData.buildingIdentity.faction] / StatUpgrade.FragmentCost(buildingUpgradeData.buildingIdentity.spawnLevel + 1);

            levelProgressBar.fillAmount = progress;
            levelProgressText.SetText($"{userdata.fragments[(int)buildingUpgradeData.buildingIdentity.faction]}/{StatUpgrade.FragmentCost(buildingUpgradeData.buildingIdentity.spawnLevel + 1)}");
        }
        else if (unitUpgradeData != null)
        {
            cardImage.sprite = unitUpgradeData.UnitIcon;

            if (populationCostRoot == null && populationCostText == null)
                return;

            populationCostText.SetText($"{unitUpgradeData.unitIdentity.populationCost}");

            levelText.SetText($"Level {unitUpgradeData.unitIdentity.spawnLevel + 1}");

            float progress = (float)userdata.fragments[(int)unitUpgradeData.unitIdentity.faction] / StatUpgrade.FragmentCost(unitUpgradeData.unitIdentity.spawnLevel + 1);

            levelProgressBar.fillAmount = progress;
            levelProgressText.SetText($"{userdata.fragments[(int)unitUpgradeData.unitIdentity.faction]}/{StatUpgrade.FragmentCost(unitUpgradeData.unitIdentity.spawnLevel + 1)}");
        }
    }
    public void RefreshAllCards()
    {
        foreach (var card in myPanel.allCards)
            card.SetCardSprite();
    }

    public void RefreshCards()
    {
        SetCardSprite();
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

        if (buildingUpgradeData != null)
        {
            Debug.Log($"<color=green> Clicked on {buildingUpgradeData.name} card</color>");
            panel.OpenActionPanel(buildingUpgradeData);
        }
        else if (unitUpgradeData != null)
        {
            Debug.Log($"<color=green> Clicked on {unitUpgradeData.name} card</color>");
            panel.OpenActionPanel(unitUpgradeData);
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
