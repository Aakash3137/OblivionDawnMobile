using UnityEngine;
using UnityEngine.UI;

public class BuildButton : MonoBehaviour
{
    [SerializeField] private BuildingType buildingType;
    [SerializeField] private TileUIPanel tileUIPanel;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        var slot = GetSlot(GameData.SelectedFaction, buildingType);
        if (slot != null && slot.prefab != null)
        {
            tileUIPanel.PlaceBuilding(slot);
        }
    }

    AllFactionsData.BuildingSlot GetSlot(FactionName faction, BuildingType type)
    {
        var data = GameData.AllFactionsData;
        switch (faction)
        {
            case FactionName.Past:
                if (type == BuildingType.MainBuilding) return data.pastMainBuilding;
                if (type == BuildingType.DefenceBuilding) return data.pastTurretBuilding;
                if (type == BuildingType.UnitBuilding) return data.pastUnitBuilding;
                if (type == BuildingType.ResourceBuilding) return data.pastGoldMine;
                break;
            case FactionName.Present:
                if (type == BuildingType.MainBuilding) return data.presentMainBuilding;
                if (type == BuildingType.DefenceBuilding) return data.presentTurretBuilding;
                if (type == BuildingType.UnitBuilding) return data.presentUnitBuilding;
                if (type == BuildingType.ResourceBuilding) return data.presentGoldMine;
                break;
            case FactionName.Future:
                if (type == BuildingType.MainBuilding) return data.futureMainBuilding;
                if (type == BuildingType.DefenceBuilding) return data.futureTurretBuilding;
                if (type == BuildingType.UnitBuilding) return data.futureUnitBuilding;
                if (type == BuildingType.ResourceBuilding) return data.futureGoldMine;
                break;
            case FactionName.Monster:
                if (type == BuildingType.MainBuilding) return data.monsterMainBuilding;
                if (type == BuildingType.DefenceBuilding) return data.monsterTurretBuilding;
                if (type == BuildingType.UnitBuilding) return data.monsterUnitBuilding;
                if (type == BuildingType.ResourceBuilding) return data.monsterGoldMine;
                break;
        }
        return null;
    }
}
