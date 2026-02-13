using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class CardUpgradeData : MonoBehaviour
{
    [ReadOnly] public BuildingDataSO buildingUpgradeData;
    [ReadOnly] public UnitProduceStatsSO unitUpgradeData;

    [SerializeField] private Image cardImage;
    [SerializeField] private Image levelProgressBar;

    [SerializeField] private Button cardButton;

    private UpgradePanelManager upgradePanelManager;

    private UpgradePopUpPanel upgradePopUpPanel;

    private void Start()
    {
        InitializeCard();
        upgradePanelManager = UpgradePanelManager.Instance;
        upgradePopUpPanel = upgradePanelManager.upgradePopUpPanel;
    }

    private void InitializeCard()
    {
        if (buildingUpgradeData != null)
        {
            cardImage.sprite = buildingUpgradeData.buildingIcon;
        }
        else if (unitUpgradeData != null)
        {
            cardImage.sprite = unitUpgradeData.UnitIcon;
        }
    }

    private void OnCardClick()
    {
        if (upgradePopUpPanel == null)
        {
            Debug.Log("<color=red>UpgradePopUpPanel is null</color>");
            return;
        }

        if (buildingUpgradeData != null)
        {
            Debug.Log($"<color=green> Clicked on {buildingUpgradeData.name} card</color>");
            upgradePopUpPanel.OpenPopUpPanel(buildingUpgradeData);
        }
        else if (unitUpgradeData != null)
        {
            Debug.Log($"<color=green> Clicked on {unitUpgradeData.name} card</color>");
            upgradePopUpPanel.OpenPopUpPanel(unitUpgradeData);
        }
        else
        {
            Debug.Log("<color=green> [CardUpgradeData] Initialize failed Uint Stats and Building Stats are null</color>");
        }

    }
    private void OnEnable()
    {
        cardButton.onClick.AddListener(OnCardClick);
    }
    private void OnDisable()
    {
        cardButton.onClick.RemoveListener(OnCardClick);
    }
}
