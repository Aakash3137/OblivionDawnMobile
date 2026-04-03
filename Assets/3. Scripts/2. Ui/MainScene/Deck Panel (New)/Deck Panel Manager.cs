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
    private List<DefenseBuildingDataSO> defenseScriptables;


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

        cityCenterScriptables = allBuildingData.mainBuildingSO;
        defenseScriptables = allBuildingData.defenseBuildingsSO;
        unitScriptables = allUnitData.allUnitsSO;
    }

    private void OnEnable()
    {
        CreateUnitCards();
        CreateDefenseCards();
    }

    public void CreateUnitCards()
    {
        foreach (var unitSO in unitScriptables)
        {
            if (unitSO == null) { LogNullScriptable("Unit"); continue; }
            var cardPanel = factionCardPanels[(int)unitSO.unitIdentity.faction].cardPanels[0];

            if (unitSO.cardDetails.cardState == CardState.Purchased && !cardPanel.scriptablesInDeck.Contains(unitSO))
            {
                cardPanel.AddCard(deckCardPrefab, unitSO);
                cardPanel.scriptablesInDeck.Add(unitSO);
            }
        }
    }
    public void CreateDefenseCards()
    {
        if (defenseScriptables == null) { LogNullScriptable("Defense Building List"); return; }
        foreach (var defenseSO in defenseScriptables)
        {
            if (defenseSO == null) { LogNullScriptable("Defense Building"); continue; }
            var cardPanel = factionCardPanels[(int)defenseSO.buildingIdentity.faction].cardPanels[0];

            if (defenseSO.cardDetails.cardState == CardState.Purchased && !cardPanel.scriptablesInDeck.Contains(defenseSO))
            {
                cardPanel.AddCard(deckCardPrefab, defenseSO);
                cardPanel.scriptablesInDeck.Add(defenseSO);
            }
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
            allUnitData.allUnits[(int)faction].aoeRangedUnits[0],
            allUnitData.allUnits[(int)faction].airUnits[0],
            allBuildingData.defenseBuildings[(int)faction].wallBuildings[0],
            allBuildingData.defenseBuildings[(int)faction].turretBuildings[0],
            allBuildingData.defenseBuildings[(int)faction].antiTankBuildings[0],
            allBuildingData.defenseBuildings[(int)faction].antiAirBuildings[0]
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