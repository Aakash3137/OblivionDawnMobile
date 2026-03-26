using System.Collections.Generic;
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
        // SpawnAndSetButtonData();
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
        var resourcesSO = allBuildingData.GetFactionResourceBuildingsSO(playerFaction);

        for (int i = 0; i < UnitsSO.Count; i++)
        {
            var button = Instantiate(buttonPrefab, offenseBuildingsRoot);
            buttons.Add(button);

            var prefab = characterDatabase.GetSpawnerBuilding(UnitsSO[i]);
            var sprite = UnitsSO[i].unitIcon;

            button.Initialize(prefab, sprite);

            prefab.SetUnitPrefab(characterDatabase.GetUnitPrefab(UnitsSO[i]));
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

    private void SpawnAndSetButtonData()
    {
        // if (decSelectionData == null)
        //     GameDebug.Log("<color=#000000>[BuildingButtonManager] DecSelectionData is null</color>");

        // playerFaction = decSelectionData.CurrentFaction;

        // GameDebug.Log($"<color=#40E0D0> [BuildingButtonsManager] Selected Faction : {playerFaction} </color>");

        // int offenseBuildingsCount = decSelectionData.AllFactionDecData[(int)playerFaction].SelectedUnitDeck.Count;
        // int defenseBuildingsCount = decSelectionData.AllFactionDecData[(int)playerFaction].SelectedDefenseDec.Count;
        // int resourceBuildingsCount = decSelectionData.AllFactionDecData[(int)playerFaction].SelectedResourceDeck.Count;

        // GameDebug.Log($"<color=#40E0D0> [BuildingButtonsManager] Offense Buildings Count : {offenseBuildingsCount} </color>");

        // for (int i = 0; i < offenseBuildingsCount; i++)
        // {
        //     var button = Instantiate(buttonPrefab, offenseBuildingsRoot);
        //     buttons.Add(button);

        //     var buildingPrefab = characterDatabase.GetSpawnerBuilding(decSelectionData.AllFactionDecData[(int)playerFaction].SelectedUnitDeck[i]);
        //     var unitSprite = decSelectionData.AllFactionDecData[(int)playerFaction].SelectedUnitDeck[i].unitIcon;

        //     button.Initialize(buildingPrefab, unitSprite);

        //     if (buildingPrefab is OffenseBuildingStats offenseBuildingPrefab)
        //         offenseBuildingPrefab.SetUnitPrefab(characterDatabase.GetUnitPrefab(decSelectionData.AllFactionDecData[(int)playerFaction].SelectedUnitDeck[i]));

        //     GameDebug.Log($"<color=#40E0D0> [BuildingButtonsManager] Offense Building : {buildingPrefab.name} with spawning unit {characterDatabase.GetUnitPrefab(decSelectionData.AllFactionDecData[(int)playerFaction].SelectedUnitDeck[i]).name}</color>");
        // }

        // for (int i = 0; i < defenseBuildingsCount; i++)
        // {
        //     var button = Instantiate(buttonPrefab, defenseBuildingsRoot);
        //     buttons.Add(button);

        //     var buildingPrefab = characterDatabase.GetDefenseBuildingPrefab(decSelectionData.AllFactionDecData[(int)playerFaction].SelectedDefenseDec[i]);
        //     var buildingSprite = decSelectionData.AllFactionDecData[(int)playerFaction].SelectedDefenseDec[i].buildingIcon;

        //     button.Initialize(buildingPrefab, buildingSprite);
        // }

        // for (int i = 0; i < resourceBuildingsCount; i++)
        // {
        //     var button = Instantiate(buttonPrefab, resourceBuildingsRoot);
        //     buttons.Add(button);

        //     var buildingPrefab = characterDatabase.GetResourceBuildingPrefab(decSelectionData.AllFactionDecData[(int)playerFaction].SelectedResourceDeck[i]);
        //     var unitSprite = decSelectionData.AllFactionDecData[(int)playerFaction].SelectedResourceDeck[i].buildingIcon;

        //     button.Initialize(buildingPrefab, unitSprite);
        // }

        // if (tileUIPanel == null || costPanelManager == null)
        //     return;

        // foreach (var button in buttons)
        // {
        //     button.SetManagers(tileUIPanel, costPanelManager);
        // }
    }


}
