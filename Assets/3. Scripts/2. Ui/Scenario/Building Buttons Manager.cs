using System.Collections.Generic;
using UnityEngine;

public class BuildingButtonsManager : MonoBehaviour
{
    [SerializeField] private DecSelectionData decSelectionData;
    [SerializeField] private BuildButton buttonPrefab;
    public CostPanelManager costPanelManager;
    public TileUIPanel tileUIPanel;
    private List<BuildButton> buttons;
    private int offenseBuildingsCount = 0;
    private int defenseBuildingsCount = 0;
    private int resourceBuildingsCount = 0;
    private int totalBuildingsCount = 0;
    private FactionName selectedFaction;

    private CharacterDatabase characterDatabase => CharacterDatabase.Instance;

    [SerializeField] private Transform offenseBuildingsRoot;
    [SerializeField] private Transform defenseBuildingsRoot;
    [SerializeField] private Transform resourceBuildingsRoot;


    private void Start()
    {
        if (decSelectionData == null)
            GameDebug.Log("<color=#000000>[BuildingButtonManager] DecSelectionData is null</color>");

        selectedFaction = decSelectionData.CurrentFaction;

        GameDebug.Log($"<color=#40E0D0> [BuildingButtonsManager] Selected Faction : {selectedFaction} </color>");

        offenseBuildingsCount = decSelectionData.AllFactionDecData[(int)selectedFaction].SelectedUnitDeck.Count;
        defenseBuildingsCount = decSelectionData.AllFactionDecData[(int)selectedFaction].SelectedDefenseDec.Count;
        resourceBuildingsCount = decSelectionData.AllFactionDecData[(int)selectedFaction].SelectedResourceDeck.Count;

        totalBuildingsCount = offenseBuildingsCount + defenseBuildingsCount + resourceBuildingsCount;

        buttons = new List<BuildButton>(totalBuildingsCount);

        GameDebug.Log($"<color=#40E0D0> [BuildingButtonsManager] Offense Buildings Count : {offenseBuildingsCount} </color>");

        for (int i = 0; i < offenseBuildingsCount; i++)
        {
            buttons.Add(Instantiate(buttonPrefab, offenseBuildingsRoot));
            buttons[i].buildingPrefab = characterDatabase.GetSpawnerBuilding(decSelectionData.AllFactionDecData[(int)selectedFaction].SelectedUnitDeck[i]);

            buttons[i].iconImage.sprite =
                decSelectionData.AllFactionDecData[(int)selectedFaction].SelectedUnitDeck[i].UnitIcon;
            
            if (buttons[i].buildingPrefab is OffenseBuildingStats offenseBuildingPrefab)
                offenseBuildingPrefab.SetUnitPrefab(characterDatabase.GetUnitPrefab(decSelectionData.AllFactionDecData[(int)selectedFaction].SelectedUnitDeck[i]));

            GameDebug.Log($"<color=#40E0D0> [BuildingButtonsManager] Offense Building : {buttons[i].buildingPrefab.name} with spawning unit {characterDatabase.GetUnitPrefab(decSelectionData.AllFactionDecData[(int)selectedFaction].SelectedUnitDeck[i]).name}</color>");
        }

        for (int i = offenseBuildingsCount; i < defenseBuildingsCount + offenseBuildingsCount; i++)
        {
            buttons.Add(Instantiate(buttonPrefab, defenseBuildingsRoot));
            buttons[i].buildingPrefab = characterDatabase.GetDefenseBuildingPrefab(decSelectionData.AllFactionDecData[(int)selectedFaction].SelectedDefenseDec[i - offenseBuildingsCount]);
            buttons[i].iconImage.sprite = decSelectionData.AllFactionDecData[(int)selectedFaction].SelectedDefenseDec[i - offenseBuildingsCount].buildingIcon;
            
        }

        for (int i = defenseBuildingsCount + offenseBuildingsCount; i < totalBuildingsCount; i++)
        {
            buttons.Add(Instantiate(buttonPrefab, resourceBuildingsRoot));
            buttons[i].buildingPrefab = characterDatabase.GetResourceBuildingPrefab(decSelectionData.AllFactionDecData[(int)selectedFaction].SelectedResourceDeck[i - (defenseBuildingsCount + offenseBuildingsCount)]);
            buttons[i].iconImage.sprite = decSelectionData.AllFactionDecData[(int)selectedFaction].SelectedResourceDeck[i - (defenseBuildingsCount + offenseBuildingsCount)].buildingIcon;
        }

        if (tileUIPanel == null || costPanelManager == null)
            return;

        foreach (BuildButton button in buttons)
        {
            button.costPanelManager = costPanelManager;
            button.tileUIPanel = tileUIPanel;
        }

    }
}
