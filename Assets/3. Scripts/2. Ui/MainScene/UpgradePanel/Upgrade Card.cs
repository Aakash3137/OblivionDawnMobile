using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour
{
    [ReadOnly] public BuildingDataSO buildingUpgradeData;
    [ReadOnly] public UnitProduceStatsSO unitUpgradeData;

    [SerializeField] private Image cardImage;
    [SerializeField] private Image levelProgressBar;

    [SerializeField] private Button cardButton;

    [SerializeField] private TMP_Text populationCostText;
    [SerializeField] private GameObject populationCostRoot;

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

            if (buildingUpgradeData is DefenseBuildingDataSO defenseBuildingData)
            {
                populationCostText.SetText($"{defenseBuildingData.buildingIdentity.populationCost}");
            }
            else
            {
                populationCostRoot.SetActive(false);
            }
        }
        else if (unitUpgradeData != null)
        {
            cardImage.sprite = unitUpgradeData.UnitIcon;

            if (populationCostRoot == null && populationCostText == null)
                return;

            populationCostText.SetText($"{unitUpgradeData.unitIdentity.populationCost}");
        }
    }

    private void OnCardClick()
    {
        var panel = UpgradePopUpPanel.Instance;

        if (panel == null)
        {
            Debug.Log("<color=green>[UpgradeCard] UpgradePopUpPanel instance not found</color>");
            return;
        }

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
