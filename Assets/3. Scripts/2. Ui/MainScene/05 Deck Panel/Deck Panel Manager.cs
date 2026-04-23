using System.Collections.Generic;
using UnityEngine;

public class DeckPanelManager : MonoBehaviour
{
    [Space(10)]
    [SerializeField] private AllBuildingData allBuildingData;
    [SerializeField] private AllUnitData allUnitData;

    [SerializeField, Space(10)] private DeckCard deckCardPrefab;

    [Space(10)] public FactionCardPanel[] factionCardPanels;
    [SerializeField, Space(10)] private FactionName defaultFaction = FactionName.Futuristic;

    private void Start()
    {
        var upgradePopUpPanel = OverlayPanelManager.Instance.upgradePopUpPanel;
        upgradePopUpPanel.OnCardPurchased += CreateDeckCard;

        foreach (var dataSO in allUnitData.AllUnitsSO)
            CreateDeckCard(dataSO);

        foreach (var dataSO in allBuildingData.AllDefenseBuildingSO)
            CreateDeckCard(dataSO);

        TryGetComponent(out DeckPanelNavigation deckPanelNavigation);
        deckPanelNavigation.SetDeckCardPanelToOpen(defaultFaction);
    }

    private void CreateDeckCard(ScriptableObject dataSO)
    {
        if (dataSO == null)
        {
            Debug.LogError($"<size=18>[Deck Panel Manager] dataSO is null</size>");
            return;
        }

        FactionName faction;
        CardsPanel cardPanel;
        bool inDeck;

        switch (dataSO)
        {
            case UnitProduceStatsSO unitSO:
                faction = unitSO.unitIdentity.faction;
                cardPanel = factionCardPanels[(int)faction].cardPanels[0];
                inDeck = cardPanel.scriptablesInDeck.Contains(unitSO);
                if (!inDeck && unitSO.cardDetails.cardState == CardState.Purchased)
                {
                    cardPanel.AddCard(deckCardPrefab, unitSO);
                    cardPanel.scriptablesInDeck.Add(unitSO);
                }
                break;
            case DefenseBuildingDataSO defenseSO:
                faction = defenseSO.buildingIdentity.faction;
                cardPanel = factionCardPanels[(int)faction].cardPanels[0];
                inDeck = cardPanel.scriptablesInDeck.Contains(defenseSO);
                if (!inDeck && defenseSO.cardDetails.cardState == CardState.Purchased)
                {
                    cardPanel.AddCard(deckCardPrefab, defenseSO);
                    cardPanel.scriptablesInDeck.Add(defenseSO);
                }

                break;
        }
    }

    public List<ScriptableObject> GetDefaultDeckCards(FactionName faction)
    {
        var defaultDeckCards = new List<ScriptableObject>
        {
            allUnitData.GetUnitsSO(faction ,ScenarioUnitType.Melee)[0],
            allUnitData.GetUnitsSO(faction ,ScenarioUnitType.Ranged)[0],
            allUnitData.GetUnitsSO(faction ,ScenarioUnitType.AOERanged)[0],
            allUnitData.GetUnitsSO(faction ,ScenarioUnitType.Air)[0],
            allBuildingData.GetDefenseBuildingsSO(faction, ScenarioDefenseType.Wall)[0],
            allBuildingData.GetDefenseBuildingsSO(faction, ScenarioDefenseType.Turret)[0],
            allBuildingData.GetDefenseBuildingsSO(faction, ScenarioDefenseType.AntiTank)[0],
            allBuildingData.GetDefenseBuildingsSO(faction, ScenarioDefenseType.AntiAir)[0]
        };

        return defaultDeckCards;
    }
    public MainBuildingUpgradeData GetMainBuildingUpgradeData(FactionName faction)
    {
        var cityCenterSO = allBuildingData.mainBuildingSO[(int)faction];
        return cityCenterSO.mainBuildingUpgradeData[cityCenterSO.buildingIdentity.spawnLevel];
    }

    private void OnDestroy()
    {
        var upgradePopUpPanel = OverlayPanelManager.Instance.upgradePopUpPanel;
        if (upgradePopUpPanel == null) return;
        upgradePopUpPanel.OnCardPurchased -= CreateDeckCard;
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