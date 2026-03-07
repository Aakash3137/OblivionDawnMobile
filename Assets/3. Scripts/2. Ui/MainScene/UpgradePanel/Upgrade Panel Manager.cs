using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanelManager : MonoBehaviour
{
    public static UpgradePanelManager Instance;

    [SerializeField] private List<MainBuildingDataSO> cityCenterScriptables;
    [SerializeField] private List<UnitProduceStatsSO> unitScriptables;
    [SerializeField] private List<BuildingDataSO> buildingScriptables;
    public FactionCardPanel[] factionCardPanel;

    [Space(20)]
    [SerializeField] private UpgradeCard cardPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        CreateBuildingCards();
        CreateUnitCards();
        CreateCityCenterCards();
    }

    public void CreateCityCenterCards()
    {
        foreach (var scriptable in cityCenterScriptables)
        {
            if (scriptable == null) { LogNullScriptable("CityCenter"); continue; }

            factionCardPanel[(int)scriptable.buildingIdentity.faction].cardPanels[0].AddCard(cardPrefab, scriptable);
        }
    }

    public void CreateBuildingCards()
    {
        foreach (var scriptable in buildingScriptables)
        {
            if (scriptable == null) { LogNullScriptable("Building"); continue; }

            factionCardPanel[(int)scriptable.buildingIdentity.faction].cardPanels[2].AddCard(cardPrefab, scriptable);
        }
    }

    public void CreateUnitCards()
    {
        foreach (var scriptable in unitScriptables)
        {
            if (scriptable == null) { LogNullScriptable("Unit"); continue; }

            factionCardPanel[(int)scriptable.unitIdentity.faction].cardPanels[1].AddCard(cardPrefab, scriptable);
        }
    }

    private void LogNullScriptable(string context) =>
        Debug.Log($"<color=green>[UpgradePanelManager] {context} scriptable is null</color>");

    private void OnValidate()
    {
        var enumValues = Enum.GetValues(typeof(FactionName));

        if (factionCardPanel == null || factionCardPanel.Length != enumValues.Length)
        {
            var resized = new FactionCardPanel[enumValues.Length];

            if (factionCardPanel != null)
            {
                for (int i = 0; i < Mathf.Min(factionCardPanel.Length, resized.Length); i++)
                    resized[i] = factionCardPanel[i];
            }
            factionCardPanel = resized;
        }

        for (int i = 0; i < enumValues.Length; i++)
            factionCardPanel[i].factionName = (FactionName)enumValues.GetValue(i);
    }
}

[Serializable]
public class FactionCardPanel
{
    public FactionName factionName;

    [Header("cardPanels: 0 = CityCenter ; 1 = Units ; 2 = Buildings")]
    public CardsPanel[] cardPanels;
}