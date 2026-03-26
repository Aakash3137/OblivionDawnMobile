using System;
using System.Collections.Generic;
using UnityEngine;

public class DeckPanelManager : MonoBehaviour
{
    public static DeckPanelManager Instance { get; private set; }
    [Space(10)]
    [SerializeField] private AllBuildingData allBuildingData;
    [SerializeField] private AllUnitData allUnitData;
    [Space(10)]
    [SerializeField] private DeckCard deckCardPrefab;
    [Space(10)]
    public FactionCardPanel[] factionCardPanels;

    public List<MainBuildingDataSO> cityCenterScriptables { get; private set; }
    private List<UnitProduceStatsSO> unitScriptables;
    private List<BuildingDataSO> buildingScriptables;

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

        cityCenterScriptables = allBuildingData.cityCenterBuildingsSO;
        buildingScriptables = allBuildingData.GetDefenseBuildingsSO();
        unitScriptables = allUnitData.allUnitsSO;

        CreateUnitCards();
        CreateDefenseCards();
    }

    public void CreateUnitCards()
    {
        foreach (var scriptable in unitScriptables)
        {
            if (scriptable == null) { LogNullScriptable("Unit"); continue; }

            if (scriptable.cardDetails.isUnlocked)
                factionCardPanels[(int)scriptable.unitIdentity.faction].cardPanels[0].AddCard(deckCardPrefab, scriptable);

        }
    }
    public void CreateDefenseCards()
    {
        foreach (var scriptable in buildingScriptables)
        {
            if (scriptable == null) { LogNullScriptable("Defense Building"); continue; }

            if (scriptable.cardDetails.isUnlocked)
                factionCardPanels[(int)scriptable.buildingIdentity.faction].cardPanels[0].AddCard(deckCardPrefab, scriptable);
        }
    }

    private void LogNullScriptable(string context) =>
        Debug.Log($"<color=green>[Deck Panel Manager] {context} scriptable is null</color>");

    public List<ScriptableObject> GetDefaultDeckCards(FactionName faction)
    {
        var defaultDeckCards = new List<ScriptableObject>
        {
            allUnitData.allUnits[(int)faction].meleeUnits[0],
            allUnitData.allUnits[(int)faction].rangedUnits[0],
            allBuildingData.defenseBuildings[(int)faction].wallBuildings[0],
            allBuildingData.defenseBuildings[(int)faction].turretBuildings[0]
        };

        return defaultDeckCards;
    }

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