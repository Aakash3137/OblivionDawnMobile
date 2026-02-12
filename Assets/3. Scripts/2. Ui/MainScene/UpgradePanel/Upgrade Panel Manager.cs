using System.Collections.Generic;
using UnityEngine;

public class UpgradePanelManager : MonoBehaviour
{
    [SerializeField] private List<UnitProduceStatsSO> unitScriptables;
    [SerializeField] private List<BuildingDataSO> buildingScriptables;

    [SerializeField] private CardsPanel buildingCardPanel;
    [SerializeField] private CardsPanel unitCardPanel;

    [SerializeField] private CardUpgrade cardPrefab;

    private void Start()
    {
        if (!buildingCardPanel.initializedBuildings)
            CreateBuildingCards();

        if (!unitCardPanel.initializedUnits)
            CreateUnitCards();
    }

    public void CreateBuildingCards()
    {
        foreach (var buildingScriptable in buildingScriptables)
        {
            buildingCardPanel.AddCards(cardPrefab, buildingScriptable);
        }
    }

    public void CreateUnitCards()
    {
        foreach (var unitScriptable in unitScriptables)
        {
            unitCardPanel.AddCards(cardPrefab, unitScriptable);
        }
    }
}
