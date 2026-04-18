using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;

public class BuildingButtonsManager : MonoBehaviour
{
    [SerializeField] private DecSelectionData deckSelectionData;
    [SerializeField] private AllBuildingData allBuildingData;
    [SerializeField] private BuildButton buttonPrefab;
    [SerializeField] private CostPanelManager costPanelManager;
    [SerializeField] private TileUIPanel tileUIPanel;


    private List<BuildButton> buttons = new();
    private FactionName playerFaction;

    private CharacterDatabase characterDatabase => CharacterDatabase.Instance;

    [SerializeField] private Transform offenseBuildingsRoot;
    [SerializeField] private Transform defenseBuildingsRoot;
    [SerializeField] private Transform resourceBuildingsRoot;


    private void Start()
    {
        SpawnAndInitializeButtons();
    }

    private void SpawnAndInitializeButtons()
    {
        if (deckSelectionData == null)
            GameDebug.Log("<color=#000000>[BuildingButtonManager] DecSelectionData is null</color>");

        playerFaction = GameData.playerFaction;

        GameDebug.Log($"<color=#40E0D0> [BuildingButtonsManager] Selected Faction : {playerFaction} </color>");

        var currentDeck = deckSelectionData.allFactionsDeckData[(int)playerFaction].decks[deckSelectionData.deckIndex].deckCardsSO;

        int resourceBuildingsCount = ScenarioDataTypes._resourceEnumValues.Length;

        GameDebug.Log($"<color=#40E0D0> [BuildingButtonsManager] Deck Buildings Count : {currentDeck.Count}, Resource Buildings Count : {resourceBuildingsCount} </color>");

        var UnitsSO = deckSelectionData.GetUnitsSOInDeck(playerFaction);
        var defenseSO = deckSelectionData.GetDefensesSOInDeck(playerFaction);
        var resourcesSO = allBuildingData.GetResourceBuildingsSO(playerFaction);

        for (int i = 0; i < UnitsSO.Count; i++)
        {
            var button = Instantiate(buttonPrefab, offenseBuildingsRoot);
            buttons.Add(button);

            var prefab = characterDatabase.GetSpawnerBuilding(UnitsSO[i]);
            var sprite = UnitsSO[i].unitIcon;

            button.Initialize(prefab, sprite);
            button.unitSO = UnitsSO[i];
        }

        for (int i = 0; i < defenseSO.Count; i++)
        {
            var button = Instantiate(buttonPrefab, defenseBuildingsRoot);
            buttons.Add(button);

            var prefab = characterDatabase.GetDefenseBuildingPrefab(defenseSO[i]);
            var sprite = defenseSO[i].buildingIcon;

            button.Initialize(prefab, sprite);
        }

        for (int i = 0; i < resourcesSO.Count; i++)
        {
            var button = Instantiate(buttonPrefab, resourceBuildingsRoot);
            buttons.Add(button);

            var prefab = characterDatabase.GetResourceBuildingPrefab(resourcesSO[i]);
            var sprite = resourcesSO[i].buildingIcon;

            button.Initialize(prefab, sprite);
        }


        if (tileUIPanel == null || costPanelManager == null)
            return;

        foreach (var button in buttons)
        {
            button.SetManagers(tileUIPanel, costPanelManager);
        }
    }
}
