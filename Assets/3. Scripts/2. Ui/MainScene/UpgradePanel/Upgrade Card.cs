using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour
{
    [Space(10)]
    [ReadOnly] public ScriptableObject upgradeDataSO;
    [Space(10)]
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
        AddListeners();
    }
    public virtual void RefreshAllCards()
    {
        foreach (var upgradeCard in myPanel.allCards)
        {
            upgradeCard.UpdateLevelUI();
        }
    }

    public virtual void RefreshCard()
    {
        SetCardSprite();
        UpdateLevelUI();
    }

    private void SetCardSprite()
    {
        Sprite icon = upgradeDataSO switch
        {
            UnitProduceStatsSO unit => unit.unitIcon,
            BuildingDataSO building => building.buildingIcon,
            _ => null
        };

        if (icon != null)
            cardImage.sprite = icon;
    }

    private void UpdateLevelUI()
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

            UpdateProgressBar(buildingDataSO.buildingIdentity);
        }
        else if (upgradeDataSO is UnitProduceStatsSO unitDataSO)
        {
            populationCostText.SetText($"{unitDataSO.populationCost}");

            levelText.SetText($"Level {unitDataSO.unitIdentity.spawnLevel + 1}");

            UpdateProgressBar(unitDataSO.unitIdentity);
        }
    }

    internal void UpdateProgressBar(Identity identity)
    {
        int spawnLevel = identity.spawnLevel;
        FactionName faction = identity.faction;

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
