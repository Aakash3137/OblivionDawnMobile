using System.Collections.Generic;
using UnityEngine;

public class UpgradePanelManager : MonoBehaviour
{
    [SerializeField] private List<UnitProduceStatsSO> unitScriptables;
    [SerializeField] private List<BuildingDataSO> buildingScriptables;

    [SerializeField] private CardsPanel buildingCardPanel;
    [SerializeField] private CardsPanel unitCardPanel;

    [SerializeField] private CardUpgradeData cardPrefab;

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
            if (buildingScriptable == null)
            {
                Debug.Log("<color=Green> [Upgrade Panel Manager]Building Scriptable is null</color>");
                continue;
            }
            buildingCardPanel.AddCards(cardPrefab, buildingScriptable);
        }
    }

    public void CreateUnitCards()
    {
        foreach (var unitScriptable in unitScriptables)
        {
            if (unitScriptable == null)
            {
                Debug.Log("<color=Green> [Upgrade Panel Manager]Unit Scriptable is null</color>");
                continue;
            }
            unitCardPanel.AddCards(cardPrefab, unitScriptable);
        }
    }
}
