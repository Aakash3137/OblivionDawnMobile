using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePanelManager : MonoBehaviour
{
    public static UpgradePanelManager Instance;

    [SerializeField] private AllBuildingData allBuildingData;
    [SerializeField] private AllUnitData allUnitData;
    [Space(10)]
    [SerializeField] private UpgradeCard upgradeCardPrefab;
    [Space(10)]
    public FactionCardPanel[] factionCardPanels;

    private List<MainBuildingDataSO> mainBuildingScriptables;
    private List<UnitProduceStatsSO> unitScriptables;
    private List<BuildingDataSO> buildingScriptables;

    public UpgradePanelNavigation upgradePanelNavigation { get; private set; }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        upgradePanelNavigation = GetComponent<UpgradePanelNavigation>();

        mainBuildingScriptables = allBuildingData.mainBuildingSO;
        buildingScriptables = allBuildingData.allBuildingsSO;
        unitScriptables = allUnitData.allUnitsSO;

        CreateBuildingCards();
        CreateUnitCards();
        CreateCityCenterCards();
    }

    public void CreateCityCenterCards()
    {
        foreach (var scriptable in mainBuildingScriptables)
        {
            if (scriptable == null) { LogNullScriptable("CityCenter"); continue; }

            factionCardPanels[(int)scriptable.buildingIdentity.faction].cardPanels[0].AddCard(upgradeCardPrefab, scriptable);
        }
    }

    public void CreateBuildingCards()
    {
        foreach (var scriptable in buildingScriptables)
        {
            if (scriptable == null) { LogNullScriptable("Building"); continue; }

            factionCardPanels[(int)scriptable.buildingIdentity.faction].cardPanels[2].AddCard(upgradeCardPrefab, scriptable);
        }
    }

    public void CreateUnitCards()
    {
        foreach (var scriptable in unitScriptables)
        {
            if (scriptable == null) { LogNullScriptable("Unit"); continue; }

            factionCardPanels[(int)scriptable.unitIdentity.faction].cardPanels[1].AddCard(upgradeCardPrefab, scriptable);
        }
    }

    private void LogNullScriptable(string context) =>
        Debug.Log($"<color=green>[Upgrade Panel Manager] {context} scriptable is null</color>");

    private void OnValidate()
    {
        var enumValues = ScenarioDataTypes._factionEnumValues;

        if (factionCardPanels == null || factionCardPanels.Length != enumValues.Length)
        {
            var resized = new FactionCardPanel[enumValues.Length];

            if (factionCardPanels != null)
            {
                for (int i = 0; i < Mathf.Min(factionCardPanels.Length, resized.Length); i++)
                    resized[i] = factionCardPanels[i];
            }
            factionCardPanels = resized;
        }

        for (int i = 0; i < enumValues.Length; i++)
        {
            factionCardPanels[i].factionName = enumValues[i];

            if (factionCardPanels[i].panelParent != null)
            {
                factionCardPanels[i].cardPanels = factionCardPanels[i].panelParent.GetComponentsInChildren<CardsPanel>();
            }
        }

    }
}

[Serializable]
public class FactionCardPanel
{
    public FactionName factionName;
    public GameObject panelParent;
    // [Header("cardPanels: 0 = CityCenter ; 1 = Units ; 2 = Buildings")]
    public CardsPanel[] cardPanels;
}