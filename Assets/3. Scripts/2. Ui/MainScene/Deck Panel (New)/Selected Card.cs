using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedCard : MonoBehaviour
{
    [ReadOnly] public ScriptableObject upgradeDataSO;
    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text populationCostText;

    public bool isActive { get; private set; }

    private void Awake()
    {
        cardImage.enabled = false;
        populationCostText.transform.parent.gameObject.SetActive(false);
    }

    public void EnablePopulationPanel()
    {
        if (populationCostText == null)
            return;

        var populationPanel = populationCostText.transform.parent.gameObject;
        populationPanel.SetActive(true);
    }

    public void SetSelectedCard(ScriptableObject dataSO)
    {
        upgradeDataSO = dataSO;

        RefreshSelectedCardVisuals();
        cardImage.enabled = true;
    }

    public void RefreshSelectedCardVisuals()
    {
        if (upgradeDataSO == null)
            return;

        Sprite icon = upgradeDataSO switch
        {
            UnitProduceStatsSO unit => unit.unitIcon,
            BuildingDataSO building => building.buildingIcon,
            _ => null
        };

        if (icon != null && cardImage != null)
            cardImage.sprite = icon;

        int popCost = upgradeDataSO switch
        {
            UnitProduceStatsSO unit => unit.populationCost,
            DefenseBuildingDataSO building => building.populationCost,
            _ => 99
        };

        if (populationCostText != null)
            populationCostText.SetText($"{popCost}");
    }

    public void UnsetSelectedCard()
    {
        upgradeDataSO = null;
        cardImage.enabled = false;
    }

    public void ShowCard()
    {
        gameObject.SetActive(true);
        isActive = true;
    }

    public void HideCard()
    {
        gameObject.SetActive(false);
        isActive = false;
    }
}